// <copyright file="IApiResponseExt.cs" company="TnT">
//     Copyright (c) TnT. All rights reserved.
// </copyright>

using Refit;
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
///     Extension methods for converting Refit's IApiResponse types to TnTResult types. Provides seamless integration between Refit HTTP client library and TnTResult pattern.
/// </summary>
/// <remarks>
///     <para>
///         This class provides extension methods that convert Refit's IApiResponse objects into TnTResult objects, enabling a functional approach to handling HTTP API responses with proper error handling.
///     </para>
///     <para>
///         The extension methods handle various scenarios including:
///         - Success responses with and without content
///         - Error responses with RFC 7807 Problem Details parsing
///         - File download responses with proper filename extraction
///         - Timeout handling for all response types
///         - Cancellation token support for async operations
///     </para>
///     <para>
///         Special handling is provided for:
///         - HTTP 201 Created responses with Location headers
///         - Stream responses for file downloads
///         - Problem Details JSON error responses
///         - Request timeout scenarios
///     </para>
/// </remarks>
/// <example>
///     <code>
///IApiResponse response = await apiClient.GetData();
///ITnTResult result = response.ToTnTResult();
///
///IApiResponse&lt;User&gt; userResponse = await apiClient.GetUser(id);
///ITnTResult&lt;User&gt; userResult = userResponse.ToTnTResult();
///
///ITnTResult&lt;User&gt; userResult = await apiClient.GetUser(id).ToTnTResultAsync();
///
///IApiResponse&lt;Stream&gt; fileResponse = await apiClient.DownloadFile(id);
///ITnTResult&lt;TnTFileDownload&gt; fileResult = fileResponse.ToTnTResult();
///     </code>
/// </example>
public static class IApiResponseExt {

    /// <summary>
    ///     The content type for RFC 7807 Problem Details responses.
    /// </summary>
    private const string ProblemJsonContentType = "application/problem+json";

    /// <summary>
    ///     JSON serializer options for parsing Problem Details responses. Configured with case-insensitive property matching.
    /// </summary>
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    ///     Converts an IApiResponse to an ITnTResult. This method handles non-generic API responses that don't return content.
    /// </summary>
    /// <param name="apiResponse">The API response to convert. Cannot be null.</param>
    /// <returns>An ITnTResult indicating success or failure. Returns TnTResult.Successful for successful responses, or TnTResult.Failure with appropriate exception for error responses.</returns>
    /// <exception cref="ArgumentNullException">Thrown when apiResponse is null.</exception>
    /// <remarks>
    ///     This method automatically disposes the API response using a using statement. Success is determined by the IsSuccessStatusCode property. Error responses are processed through
    ///     HandleErrorResponse for consistent error handling.
    /// </remarks>
    /// <example>
    ///     <code>
    ///IApiResponse response = await apiClient.DeleteUser(userId);
    ///ITnTResult result = response.ToTnTResult();
    ///
    ///if (result.IsSuccess) {
    ///Console.WriteLine("User deleted successfully");
    ///} else {
    ///Console.WriteLine($"Error: {result.Error.Message}");
    ///}
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
                return HandleErrorResponse<object>(apiResponse);
            }
        }
    }

    /// <summary>
    ///     Converts an IApiResponse of type TSuccess to an ITnTResult of type TSuccess. This method handles generic API responses that return typed content.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value expected from the API response. This type should match the expected response content type.</typeparam>
    /// <param name="apiResponse">The typed API response to convert. Cannot be null.</param>
    /// <returns>An ITnTResult&lt;TSuccess&gt; containing either the response content on success or an appropriate error on failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when apiResponse is null.</exception>
    /// <remarks>
    ///     <para>
    ///         This method includes special handling for HTTP 201 Created responses: When the response has a Created status code and a Location header, and TSuccess is string, the Location header
    ///         value is returned as the result.
    ///     </para>
    ///     <para>
    ///         The method automatically disposes the API response using a using statement. Success is determined by the IsSuccessStatusCode property. Error responses are processed through
    ///         HandleErrorResponse for consistent error handling.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code>
    /// // Basic usage IApiResponse&lt;User&gt; response = await apiClient.GetUser(userId); ITnTResult&lt;User&gt; result = response.ToTnTResult();
    ///
    ///if (result.IsSuccess) { User user = result.Value; Console.WriteLine($"Retrieved user: {user.Name}"); }
    ///
    ///createResponse.ToTnTResult(); // If successful and Created, returns the Location header value
    ///     </code>
    /// </example>
    public static ITnTResult<TSuccess> ToTnTResult<TSuccess>(this IApiResponse<TSuccess> apiResponse) {
        if (apiResponse is null) {
            return TnTResult.Failure<TSuccess>(new Exception("Failed with empty response from the server"));
        }
        using (apiResponse) {
            if (apiResponse.IsSuccessStatusCode) {
                // Handle special case for Created status with Location header This is commonly used in REST APIs where POST operations return the location of the created resource
                if (apiResponse.StatusCode == HttpStatusCode.Created &&
                    apiResponse.Headers.Location is not null &&
                    typeof(TSuccess) == typeof(string)) {
                    var locationValue = (TSuccess)Convert.ChangeType(apiResponse.Headers.Location.ToString(), typeof(TSuccess));
                    return TnTResult.Success(locationValue);
                }

                return TnTResult.Success(apiResponse.Content!);
            }
            else {
                return HandleErrorResponse<TSuccess>(apiResponse);
            }
        }
    }

    /// <summary>
    ///     Converts an IApiResponse of type Stream to an ITnTResult of type TnTFileDownload. This method is specifically designed for handling file download responses.
    /// </summary>
    /// <param name="apiResponse">The stream API response containing file data. Cannot be null.</param>
    /// <returns>An ITnTResult&lt;TnTFileDownload&gt; containing the file download information on success or an appropriate error on failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when apiResponse is null.</exception>
    /// <remarks>
    ///     <para>
    ///         This method extracts file information from HTTP headers:
    ///         - Filename: Extracted from Content-Disposition header (FileNameStar preferred over FileName)
    ///         - Content-Type: Extracted from Content-Type header, defaults to "application/octet-stream"
    ///     </para>
    ///     <para>The method automatically disposes the API response using a using statement. The Stream content is implicitly converted to FileContents in the TnTFileDownload object.</para>
    ///     <para>If the response content is null, the method returns a failure result. Error responses are processed through HandleErrorResponse for consistent error handling.</para>
    /// </remarks>
    /// <example>
    ///     <code>
    /// IApiResponse&lt;Stream&gt; response = await apiClient.DownloadFile(fileId);
    ///ITnTResult&lt;TnTFileDownload&gt; result = response.ToTnTResult();
    ///
    ///if (result.IsSuccess) { TnTFileDownload download = result.Value; Console.WriteLine($"Downloaded: {download.Filename}"); Console.WriteLine($"Content-Type: {download.ContentType}"); // Use
    ///download.Contents to access the file stream }
    ///     </code>
    /// </example>
    public static ITnTResult<TnTFileDownload> ToTnTResult(this IApiResponse<Stream> apiResponse) {
        if (apiResponse is null) {
            return TnTResult.Failure<TnTFileDownload>(new Exception("Failed with empty response from the server"));
        }
        using (apiResponse) {
            if (apiResponse.IsSuccessStatusCode) {
                if (apiResponse.Content is null) {
                    return TnTResult.Failure<TnTFileDownload>(new Exception("No content in the response"));
                }

                // Extract filename from Content-Disposition header with proper fallback logic
                var filename = apiResponse.ContentHeaders?.ContentDisposition?.FileNameStar ??
                    apiResponse.ContentHeaders?.ContentDisposition?.FileName ??
                    "download";

                // Extract content type with sensible default for binary files
                var contentType = apiResponse.ContentHeaders?.ContentType?.MediaType ?? "application/octet-stream";

                return TnTResult.Success(new TnTFileDownload {
                    Contents = apiResponse.Content!, // Implicit conversion from Stream to FileContents
                    Filename = filename,
                    ContentType = contentType
                });
            }
            else {
                return HandleErrorResponse<TnTFileDownload>(apiResponse);
            }
        }
    }

    /// <summary>
    ///     Asynchronously converts a Task of IApiResponse to an ITnTResult. This method provides async/await support for non-generic API responses.
    /// </summary>
    /// <param name="task">The task representing the API response operation. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous conversion operation. The task result contains an ITnTResult indicating success or failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when task is null.</exception>
    /// <remarks>
    ///     <para>This method awaits the provided task and then converts the resulting IApiResponse to an ITnTResult using the synchronous ToTnTResult method.</para>
    ///     <para>
    ///         The method includes special handling for:
    ///         - OperationCanceledException: Wrapped and returned as a failure result
    ///         - Request timeouts: Detected and converted to TimeoutException
    ///         - ConfigureAwait(false): Used to avoid deadlocks in synchronization contexts
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code>
    ///// Direct async conversion ITnTResult result = await apiClient.UpdateUserAsync(user).ToTnTResultAsync();
    ///     </code>
    /// </example>
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
    ///     Asynchronously converts a Task of IApiResponse of type TSuccess to an ITnTResult of type TSuccess. This method provides async/await support for generic API responses with typed content.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value expected from the API response. This type should match the expected response content type.</typeparam>
    /// <param name="task">The task representing the typed API response operation. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous conversion operation. The task result contains an ITnTResult&lt;TSuccess&gt; with either the response content or an error.</returns>
    /// <exception cref="ArgumentNullException">Thrown when task is null.</exception>
    /// <remarks>
    ///     <para>This method awaits the provided task and then converts the resulting IApiResponse&lt;TSuccess&gt; to an ITnTResult&lt;TSuccess&gt; using the synchronous ToTnTResult method.</para>
    ///     <para>
    ///         The method includes the same special handling as the non-generic version:
    ///         - OperationCanceledException: Wrapped and returned as a failure result
    ///         - Request timeouts: Detected and converted to TimeoutException
    ///         - ConfigureAwait(false): Used to avoid deadlocks in synchronization contexts
    ///         - HTTP 201 Created responses with Location headers (when TSuccess is string)
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code>
    /// // Direct async conversion ITnTResult&lt;User&gt; result = await apiClient.GetUserAsync(userId).ToTnTResultAsync();
    ///
    ///
    ///if (result.IsSuccess) { UserList users = result.Value; Console.WriteLine($"Retrieved {users.Count} users"); }
    ///     </code>
    /// </example>
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
    ///     Asynchronously converts a Task of IApiResponse of type Stream to an ITnTResult of type TnTFileDownload. This method provides async/await support for file download operations.
    /// </summary>
    /// <param name="task">The task representing the stream API response operation. Cannot be null.</param>
    /// <returns>
    ///     A task that represents the asynchronous file download conversion operation. The task result contains an ITnTResult&lt;TnTFileDownload&gt; with either the file download information or an error.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when task is null.</exception>
    /// <remarks>
    ///     <para>This method awaits the provided task and then converts the resulting IApiResponse&lt;Stream&gt; to an ITnTResult&lt;TnTFileDownload&gt; using the synchronous ToTnTResult method.</para>
    ///     <para>
    ///         The method includes the same special handling as other async methods:
    ///         - OperationCanceledException: Wrapped and returned as a failure result
    ///         - Request timeouts: Detected and converted to TimeoutException
    ///         - ConfigureAwait(false): Used to avoid deadlocks in synchronization contexts
    ///     </para>
    ///     <para>
    ///         File-specific handling includes:
    ///         - Filename extraction from Content-Disposition headers
    ///         - Content-Type detection with fallback to "application/octet-stream"
    ///         - Stream to FileContents implicit conversion
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code>
    /// // Direct async file download ITnTResult&lt;TnTFileDownload&gt; result = await apiClient.DownloadFileAsync(fileId).ToTnTResultAsync();
    ///
    ///if (result.IsSuccess) { TnTFileDownload download = result.Value; await File.WriteAllBytesAsync(download.Filename, download.Contents.ToByteArray()); Console.WriteLine($"Downloaded
    ///{download.Filename} ({download.ContentType})"); }
    ///
    ///.DownloadLargeFileAsync(fileId, cts.Token) .ToTnTResultAsync();
    ///     </code>
    /// </example>
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
    ///     Gets a default error message for API responses when no specific error content is available.
    /// </summary>
    /// <param name="apiResponse">The API response to generate an error message for.</param>
    /// <returns>A formatted error message containing the status code and reason phrase.</returns>
    private static string GetDefaultErrorMessage(IApiResponse apiResponse) =>
        $"Failed with status code {apiResponse.StatusCode} {apiResponse.ReasonPhrase}";

    /// <summary>
    ///     Helper method to handle timeout checking for non-generic API responses. Provides a consistent way to detect and handle request timeout scenarios.
    /// </summary>
    /// <param name="apiResponse">The API response to check for timeout.</param>
    /// <param name="converter">  Function to convert the response if it's not a timeout.</param>
    /// <returns>A failure result with TimeoutException if the response indicates a timeout, otherwise the result of calling the converter function.</returns>
    private static ITnTResult HandleApiResponseWithTimeout(IApiResponse apiResponse, Func<IApiResponse, ITnTResult> converter) =>
        apiResponse.StatusCode == HttpStatusCode.RequestTimeout
            ? TnTResult.Failure(new TimeoutException("The request timed out"))
            : converter(apiResponse);

    /// <summary>
    ///     Helper method to handle timeout checking for generic API responses. Provides a consistent way to detect and handle request timeout scenarios for typed responses.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="apiResponse">The typed API response to check for timeout.</param>
    /// <param name="converter">  Function to convert the response if it's not a timeout.</param>
    /// <returns>A failure result with TimeoutException if the response indicates a timeout, otherwise the result of calling the converter function.</returns>
    private static ITnTResult<TSuccess> HandleApiResponseWithTimeout<TSuccess>(IApiResponse<TSuccess> apiResponse, Func<IApiResponse<TSuccess>, ITnTResult<TSuccess>> converter) =>
        apiResponse.StatusCode == HttpStatusCode.RequestTimeout
            ? TnTResult.Failure<TSuccess>(new TimeoutException("The request timed out"))
            : converter(apiResponse);

    /// <summary>
    ///     Handles error responses by attempting to parse Problem Details and providing appropriate error messages. This method centralizes error handling logic for all API response types.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value for the result.</typeparam>
    /// <param name="apiResponse">The API response containing error information.</param>
    /// <returns>A failure result with an appropriate error message.</returns>
    /// <remarks>
    ///     <para>This method implements a two-tier error handling strategy:</para>
    ///     <list type="number">
    ///         <item>
    ///             <description>
    ///                 <strong>RFC 7807 Problem Details:</strong> If the response has a Problem Details content type and valid Problem Details JSON, uses the Detail property for the error message.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <strong>Fallback Error Handling:</strong> For Internal Server Errors (500), uses the ReasonPhrase. For other errors, uses the error content. Falls back to a default message if
    ///                 neither is available.
    ///             </description>
    ///         </item>
    ///     </list>
    ///     <para>Error messages are cleaned by trimming surrounding quotes which may be present in some API responses.</para>
    /// </remarks>
    private static ITnTResult<TSuccess> HandleErrorResponse<TSuccess>(IApiResponse apiResponse) {
        // Try to parse problem details for better error messages
        if (IsProblemJsonContent(apiResponse) &&
            apiResponse.Error?.Content is not null &&
            TryParseProblemDetails(apiResponse.Error.Content, out var problemDetails) &&
            !string.IsNullOrWhiteSpace(problemDetails?.Detail)) {
            return TnTResult.Failure<TSuccess>(new Exception(problemDetails.Detail));
        }

        // Fallback to standard error handling
        var errorMessage = apiResponse.StatusCode == HttpStatusCode.InternalServerError
            ? apiResponse.Error?.ReasonPhrase?.Trim('"') ?? GetDefaultErrorMessage(apiResponse)
            : apiResponse.Error?.Content?.Trim('"') ?? GetDefaultErrorMessage(apiResponse);

        return TnTResult.Failure<TSuccess>(new Exception(errorMessage));
    }

    /// <summary>
    ///     Helper method to handle timeout checking for Stream API responses converting to TnTFileDownload. Provides specialized timeout handling for file download operations.
    /// </summary>
    /// <param name="apiResponse">The stream API response to check for timeout.</param>
    /// <param name="converter">  Function to convert the response if it's not a timeout.</param>
    /// <returns>A failure result with TimeoutException if the response indicates a timeout, otherwise the result of calling the converter function.</returns>
    private static ITnTResult<TnTFileDownload> HandleStreamApiResponseWithTimeout(IApiResponse<Stream> apiResponse, Func<IApiResponse<Stream>, ITnTResult<TnTFileDownload>> converter) =>
        apiResponse.StatusCode == HttpStatusCode.RequestTimeout
            ? TnTResult.Failure<TnTFileDownload>(new TimeoutException("The request timed out")) : converter(apiResponse);

    /// <summary>
    ///     Determines whether the API response contains Problem Details JSON content. Checks the Content-Type header for the RFC 7807 Problem Details media type.
    /// </summary>
    /// <param name="apiResponse">The API response to check.</param>
    /// <returns><c>true</c> if the response has a Content-Type of "application/problem+json" (case-insensitive); otherwise, <c>false</c>.</returns>
    private static bool IsProblemJsonContent(IApiResponse apiResponse) =>
        apiResponse.ContentHeaders?.ContentType?.ToString()
            .Equals(ProblemJsonContentType, StringComparison.OrdinalIgnoreCase) == true;

    /// <summary>
    ///     Attempts to parse JSON content as RFC 7807 Problem Details. Uses case-insensitive property matching for better compatibility.
    /// </summary>
    /// <param name="content">       The JSON content string to parse.</param>
    /// <param name="problemDetails">When this method returns, contains the parsed Problem Details object if successful; otherwise, <c>null</c>.</param>
    /// <returns><c>true</c> if the content was successfully parsed as Problem Details; otherwise, <c>false</c>.</returns>
    /// <remarks>This method only catches JsonException to avoid masking other types of exceptions. The method validates that the deserialized object is not null before returning success.</remarks>
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
}