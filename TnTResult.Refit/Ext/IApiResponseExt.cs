// <copyright file="IApiResponseExt.cs" company="TnT">
//     Copyright (c) TnT. All rights reserved.
// </copyright>

using Refit;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace TnTResult.Refit.Ext;

/// <summary>
///     Simple ProblemDetails class for RFC 7807 problem details parsing. Used to deserialize error responses that follow the Problem Details for HTTP APIs standard.
/// </summary>
/// <remarks>
///     This class implements a subset of RFC 7807 Problem Details for HTTP APIs specification.
///     See: https://tools.ietf.org/html/rfc7807
/// </remarks>
[ExcludeFromCodeCoverage]
internal class ProblemDetails {

    /// <summary>
    ///     Gets or sets a human-readable explanation specific to this occurrence of the problem.
    /// </summary>
    public string? Detail { get; set; }

    /// <summary>
    ///     Gets or sets the HTTP status code for this occurrence of the problem.
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    ///     Gets or sets a short, human-readable summary of the problem type.
    /// </summary>
    public string? Title { get; set; }
}

/// <summary>
///     Extension methods for converting Refit's <see cref="IApiResponse" /> types to TnTResult types. Provides seamless integration between Refit HTTP client library and the TnTResult pattern.
/// </summary>
/// <remarks>
///     <para>
///         These extension methods convert Refit's <see cref="IApiResponse" /> objects into <see cref="ITnTResult" /> / <see cref="ITnTResult{TSuccess}" /> instances, enabling a functional approach
///         to handling HTTP API responses with consistent error handling.
///     </para>
///     <para>Supported scenarios:</para>
///     <list type="bullet">
///         <item>
///             <description>Success responses (with and without content)</description>
///         </item>
///         <item>
///             <description>Error responses including RFC 7807 (problem+json) parsing</description>
///         </item>
///         <item>
///             <description>File download responses (stream and <see cref="TnTFileDownload" />)</description>
///         </item>
///         <item>
///             <description>Automatic extraction of Location header for 201 Created and redirect responses</description>
///         </item>
///         <item>
///             <description>Timeout responses mapped to <see cref="TimeoutException" /></description>
///         </item>
///         <item>
///             <description>Graceful handling of cancellation ( <see cref="OperationCanceledException" />) in async variants</description>
///         </item>
///     </list>
///     <para>
///         Most overloads dispose the underlying <see cref="IApiResponse" /> immediately (via <c>using</c>).
///         However, when the content type implements <see cref="IDisposable" />, the <see cref="IApiResponse" /> is NOT disposed automatically,
///         and the caller is responsible for disposing both the content and the response.
///     </para>
/// </remarks>
public static class IApiResponseExt {
    private const string ProblemJsonContentType = "application/problem+json";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    /// <summary>
    ///     Converts a non-generic <see cref="IApiResponse" /> into an <see cref="ITnTResult" />.
    /// </summary>
    /// <param name="apiResponse">The API response. If <c>null</c>, a failed result is returned.</param>
    /// <returns>
    ///     <see cref="TnTResult.Successful" /> when <see cref="IApiResponse.IsSuccessStatusCode" /> is <c>true</c>; otherwise a failed result containing an appropriate <see cref="Exception" />.
    /// </returns>
    /// <remarks>This method never throws for a <c>null</c> response; instead it returns a failed result with a descriptive error message.</remarks>
    /// <example>
    ///     <code>
    ///IApiResponse response = await api.DeleteUser(id);
    ///ITnTResult result = response.ToTnTResult();
    ///if (result.IsSuccessful) { /* success */ } else { Console.WriteLine(result.ErrorMessage); }
    ///     </code>
    /// </example>
    public static ITnTResult ToTnTResult(this IApiResponse apiResponse) {
        if (apiResponse is null) {
            return TnTResult.Failure(new Exception("Failed with empty response from the server"));
        }

        using (apiResponse) {
            if (apiResponse.IsSuccessStatusCode) {
                return TnTResult.Successful;
            }
            else {
                return HandleErrorResponse<Empty>(apiResponse);
            }
        }
    }

    /// <summary>
    ///     Converts a typed <see cref="IApiResponse{TSuccess}" /> into an <see cref="ITnTResult{TSuccess}" />.
    /// </summary>
    /// <typeparam name="TSuccess">The expected success value type.</typeparam>
    /// <param name="apiResponse">The typed API response. If <c>null</c>, a failed result is returned.</param>
    /// <returns>A successful result containing the deserialized content (including special Location header handling) or a failed result with an error.</returns>
    /// <remarks>
    ///     <para>Special handling includes:</para>
    ///     <list type="bullet">
    ///         <item>
    ///             <description>201 Created + Location header + <typeparamref name="TSuccess" /> is <see cref="string" /> ⇒ returns Location URI string.</description>
    ///         </item>
    ///         <item>
    ///             <description>301/302/307 redirect + Location header + <typeparamref name="TSuccess" /> is <see cref="string" /> or <see cref="Uri" /> ⇒ returns Location value.</description>
    ///         </item>
    ///         <item>
    ///             <description>Error responses attempt RFC 7807 (problem+json) parsing before falling back to content or reason phrase.</description>
    ///         </item>
    ///         <item>
    ///             <description>When content is <see cref="IDisposable"/>, the response is NOT disposed automatically - the caller owns the lifecycle.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    /// <example>
    ///     <code>
    ///IApiResponse&lt;User&gt; response = await api.GetUser(id);
    ///ITnTResult&lt;User&gt; result = response.ToTnTResult();
    ///if (result.IsSuccessful)
    ///{
    ///Console.WriteLine(result.Value.Name);
    ///}
    ///else
    ///{
    ///Console.WriteLine(result.ErrorMessage);
    ///}
    ///     </code>
    /// </example>
    public static ITnTResult<TSuccess> ToTnTResult<TSuccess>(this IApiResponse<TSuccess> apiResponse) {
        if (apiResponse is null) {
            return TnTResult.Failure<TSuccess>(new Exception("Failed with empty response from the server"));
        }

        // Determine if we should dispose the response
        // Only skip disposal if content is disposable AND we're returning it successfully
        var contentIsDisposable = typeof(IDisposable).IsAssignableFrom(typeof(TSuccess));
        var shouldDispose = true;
        
        try {
            if (apiResponse.IsSuccessStatusCode) {
                // Handle special case for Created status with Location header
                if (apiResponse.StatusCode == HttpStatusCode.Created &&
                    apiResponse.Headers.Location is not null &&
                    typeof(TSuccess) == typeof(string)) {
                    var locationValue = (TSuccess)Convert.ChangeType(apiResponse.Headers.Location.ToString(), typeof(TSuccess));
                    return TnTResult.Success(locationValue);
                }

                // Redirect handling
                if (apiResponse.StatusCode is HttpStatusCode.Found or HttpStatusCode.TemporaryRedirect or HttpStatusCode.MovedPermanently && apiResponse.Headers.Location is not null) {
                    if (typeof(TSuccess) == typeof(string)) {
                        var locationValue = (TSuccess)Convert.ChangeType(apiResponse.Headers.Location.ToString(), typeof(TSuccess));
                        return TnTResult.Success(locationValue);
                    }
                    else if (typeof(TSuccess) == typeof(Uri)) {
                        var locationValue = (TSuccess)Convert.ChangeType(apiResponse.Headers.Location, typeof(TSuccess));
                        return TnTResult.Success(locationValue);
                    }
                }

                // If content is disposable, don't dispose the response - caller owns it
                shouldDispose = !contentIsDisposable;
                return TnTResult.Success(apiResponse.Content!);
            }
            else {
                return HandleErrorResponse<TSuccess>(apiResponse);
            }
        }
        finally {
            if (shouldDispose) {
                apiResponse.Dispose();
            }
        }
    }

    /// <summary>
    ///     Converts an <see cref="IApiResponse{Stream}" /> into an <see cref="ITnTResult{TnTFileDownload}" /> (file download helper).
    /// </summary>
    /// <param name="apiResponse">The stream response. If <c>null</c>, a failed result is returned.</param>
    /// <returns>A successful file download result or a failed result with an error.</returns>
    /// <remarks>
    ///     Header extraction:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>Filename: <c>Content-Disposition.FileNameStar</c> then <c>FileName</c> else "download".</description>
    ///         </item>
    ///         <item>
    ///             <description>ContentType: <c>Content-Type</c> else <c>application/octet-stream</c>.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public static ITnTResult<TnTFileDownload> ToTnTResult(this IApiResponse<Stream> apiResponse) {
        if (apiResponse is null) {
            return TnTResult.Failure<TnTFileDownload>(new Exception("Failed with empty response from the server"));
        }
        if (apiResponse.IsSuccessStatusCode) {
            if (apiResponse.Content is null) {
                // Dispose the response since we're returning an error and TnTFileDownload won't take ownership
                using (apiResponse) {
                    return TnTResult.Failure<TnTFileDownload>(new Exception("No content in the response"));
                }
            }

            var filename = apiResponse.ContentHeaders?.ContentDisposition?.FileNameStar ??
                apiResponse.ContentHeaders?.ContentDisposition?.FileName ??
                "download";

            var contentType = apiResponse.ContentHeaders?.ContentType?.MediaType ?? "application/octet-stream";

            // Note: We do NOT dispose apiResponse here because TnTFileDownload takes ownership
            // through DisposableContent and will dispose it when the TnTFileDownload is disposed
            return TnTResult.Success(new TnTFileDownload {
                Contents = apiResponse.Content!,
                Filename = filename,
                ContentType = contentType,
                DisposableContent = apiResponse
            });
        }
        else {
            // On error, we need to dispose the response since TnTFileDownload won't take ownership
            using (apiResponse) {
                return HandleErrorResponse<TnTFileDownload>(apiResponse);
            }
        }
    }

    /// <summary>
    ///     Converts an <see cref="IApiResponse{TnTFileDownload}" /> into an <see cref="ITnTResult{TnTFileDownload}" />.
    /// </summary>
    /// <param name="apiResponse">The file download response. If <c>null</c>, a failed result is returned.</param>
    /// <returns>A successful result wrapping the provided <see cref="TnTFileDownload" /> or a failure.</returns>
    /// <remarks>
    ///     If <see cref="TnTFileDownload.Contents" /> is a <see cref="Stream" /> a guiding <see cref="InvalidOperationException" /> is thrown, instructing consumers to use the stream overload instead.
    /// </remarks>
    public static ITnTResult<TnTFileDownload> ToTnTResult(this IApiResponse<TnTFileDownload> apiResponse) {
        if (apiResponse is null) {
            return TnTResult.Failure<TnTFileDownload>(new Exception("Failed with empty response from the server"));
        }
        using (apiResponse) {
            if (apiResponse.IsSuccessStatusCode) {
                if (apiResponse.Content is null) {
                    return TnTResult.Failure<TnTFileDownload>(new Exception("No content in the response"));
                }

                var data = apiResponse.Content.Contents;
                if (data.IsStream) {
                    throw new InvalidOperationException($"The {nameof(TnTFileDownload)}.{nameof(TnTFileDownload.Contents)} is a {nameof(Stream)}. Use the {nameof(IApiResponse)}<{nameof(Stream)}> overload instead.");
                }

                return TnTResult.Success(apiResponse.Content);
            }
            else {
                return HandleErrorResponse<TnTFileDownload>(apiResponse);
            }
        }
    }

    /// <summary>
    ///     Asynchronously converts a <see cref="Task{TResult}" /> of <see cref="IApiResponse" /> into an <see cref="ITnTResult" />.
    /// </summary>
    /// <param name="task">The pending response task.</param>
    /// <returns>A task producing a success or failure result.</returns>
    /// <remarks>Cancellation is wrapped and returned as a failed result (no exception is thrown to the caller).</remarks>
    public static async Task<ITnTResult> ToTnTResultAsync(this Task<IApiResponse> task) {
        try {
            var apiResponse = await task.ConfigureAwait(false);
            return HandleApiResponseWithTimeout(apiResponse, response => response.ToTnTResult());
        }
        catch (OperationCanceledException ex) {
            return TnTResult.Failure(new OperationCanceledException("The operation was cancelled", ex));
        }
    }

    /// <summary>
    ///     Asynchronously converts a <see cref="Task{TResult}" /> of <see cref="IApiResponse{TSuccess}" /> into an <see cref="ITnTResult{TSuccess}" />.
    /// </summary>
    /// <typeparam name="TSuccess">The success value type.</typeparam>
    /// <param name="task">The pending typed response task.</param>
    /// <returns>A task producing a typed success or failure result.</returns>
    /// <remarks>Applies the same Created/Redirect special cases as the synchronous overload.</remarks>
    public static async Task<ITnTResult<TSuccess>> ToTnTResultAsync<TSuccess>(this Task<IApiResponse<TSuccess>> task) {
        try {
            var apiResponse = await task.ConfigureAwait(false);
            return HandleApiResponseWithTimeout(apiResponse, response => response.ToTnTResult());
        }
        catch (OperationCanceledException ex) {
            return TnTResult.Failure<TSuccess>(new OperationCanceledException("The operation was cancelled", ex));
        }
    }

    /// <summary>
    ///     Asynchronously converts a <see cref="Task{TResult}" /> of <see cref="IApiResponse{Stream}" /> into an <see cref="ITnTResult{TnTFileDownload}" />.
    /// </summary>
    /// <param name="task">The pending stream response task.</param>
    /// <returns>A task producing a file download result or an error result.</returns>
    public static async Task<ITnTResult<TnTFileDownload>> ToTnTResultAsync(this Task<IApiResponse<Stream>> task) {
        try {
            var apiResponse = await task.ConfigureAwait(false);
            return HandleStreamApiResponseWithTimeout(apiResponse, response => response.ToTnTResult());
        }
        catch (OperationCanceledException ex) {
            return TnTResult.Failure<TnTFileDownload>(new OperationCanceledException("The operation was cancelled", ex));
        }
    }

    /// <summary>
    ///     Asynchronously converts a <see cref="Task{TResult}" /> of <see cref="IApiResponse{TnTFileDownload}" /> into an <see cref="ITnTResult{TnTFileDownload}" />.
    /// </summary>
    /// <param name="task">The pending file download response task.</param>
    /// <returns>A task producing a file download result or an error result.</returns>
    public static async Task<ITnTResult<TnTFileDownload>> ToTnTResultAsync(this Task<IApiResponse<TnTFileDownload>> task) {
        try {
            var apiResponse = await task.ConfigureAwait(false);
            return HandleTnTFileDownloadResponseWithTimeout(apiResponse, response => response.ToTnTResult());
        }
        catch (OperationCanceledException ex) {
            return TnTResult.Failure<TnTFileDownload>(new OperationCanceledException("The operation was cancelled", ex));
        }
    }

    // Internal helpers -------------------------------------------------------------------------

    private static string GetDefaultErrorMessage(IApiResponse apiResponse) =>
        $"Failed with status code {apiResponse.StatusCode} {apiResponse.ReasonPhrase}";

    private static ITnTResult HandleApiResponseWithTimeout(IApiResponse apiResponse, Func<IApiResponse, ITnTResult> converter) =>
        apiResponse.StatusCode == HttpStatusCode.RequestTimeout
            ? TnTResult.Failure(new TimeoutException("The request timed out"))
            : converter(apiResponse);

    private static ITnTResult<TSuccess> HandleApiResponseWithTimeout<TSuccess>(IApiResponse<TSuccess> apiResponse, Func<IApiResponse<TSuccess>, ITnTResult<TSuccess>> converter) =>
        apiResponse.StatusCode == HttpStatusCode.RequestTimeout
            ? TnTResult.Failure<TSuccess>(new TimeoutException("The request timed out"))
            : converter(apiResponse);

    /// <summary>
    ///     Centralized error handling including RFC 7807 parsing and fallback strategies.
    /// </summary>
    private static ITnTResult<TSuccess> HandleErrorResponse<TSuccess>(IApiResponse apiResponse) {
        if (IsProblemJsonContent(apiResponse) &&
            apiResponse.Error?.Content is not null &&
            TryParseProblemDetails(apiResponse.Error.Content, out var problemDetails) &&
            !string.IsNullOrWhiteSpace(problemDetails?.Detail)) {
            return TnTResult.Failure<TSuccess>(new Exception(problemDetails.Detail));
        }

        var errorMessage = apiResponse.StatusCode == HttpStatusCode.InternalServerError
            ? apiResponse.Error?.ReasonPhrase?.Trim('"') ?? GetDefaultErrorMessage(apiResponse)
            : apiResponse.Error?.Content?.Trim('"') ?? GetDefaultErrorMessage(apiResponse);

        return TnTResult.Failure<TSuccess>(new Exception(errorMessage));
    }

    private static ITnTResult<TnTFileDownload> HandleStreamApiResponseWithTimeout(IApiResponse<Stream> apiResponse, Func<IApiResponse<Stream>, ITnTResult<TnTFileDownload>> converter) =>
        apiResponse.StatusCode == HttpStatusCode.RequestTimeout
            ? TnTResult.Failure<TnTFileDownload>(new TimeoutException("The request timed out")) : converter(apiResponse);

    /// <summary>
    ///     Timeout handler for <see cref="IApiResponse{TnTFileDownload}" /> responses.
    /// </summary>
    private static ITnTResult<TnTFileDownload> HandleTnTFileDownloadResponseWithTimeout(IApiResponse<TnTFileDownload> apiResponse, Func<IApiResponse<TnTFileDownload>, ITnTResult<TnTFileDownload>> converter) =>
        apiResponse.StatusCode == HttpStatusCode.RequestTimeout
            ? TnTResult.Failure<TnTFileDownload>(new TimeoutException("The request timed out")) : converter(apiResponse);

    private static bool IsProblemJsonContent(IApiResponse apiResponse) =>
        apiResponse.ContentHeaders?.ContentType?.ToString()
            .Equals(ProblemJsonContentType, StringComparison.OrdinalIgnoreCase) == true;

    private static bool TryParseProblemDetails(string content, out ProblemDetails? problemDetails) {
        try {
            problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content, JsonSerializerOptions);
            return problemDetails is not null;
        }
        catch (JsonException) {
            problemDetails = null;
            return false;
        }
    }

    /// <summary>
    ///     Placeholder empty success type used for non-generic response conversions.
    /// </summary>
    private readonly struct Empty { }
}