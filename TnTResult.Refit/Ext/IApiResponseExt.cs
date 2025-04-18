﻿using Refit;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace TnTResult.Refit.Ext;

/// <summary>
///     Extension methods for IApiResponse.
/// </summary>
public static class IApiResponseExt {

    /// <summary>
    ///     Converts an IApiResponse to an ITnTResult.
    /// </summary>
    /// <param name="apiResponse">The API response.</param>
    /// <returns>The result of the operation.</returns>
    public static ITnTResult ToTnTResult(this IApiResponse apiResponse) => new PromotingApiResponse(apiResponse).ToTnTResult();

    /// <summary>
    ///     Converts an IApiResponse of type TSuccess to an ITnTResult of type TSuccess.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="apiResponse">The API response.</param>
    /// <returns>The result of the operation.</returns>
    public static ITnTResult<TSuccess> ToTnTResult<TSuccess>(this IApiResponse<TSuccess> apiResponse) {
        try {
            if (apiResponse is null) {
                return TnTResult.Failure<TSuccess>(new Exception("Failed with empty response from the server"));
            }
            else {
                if (apiResponse.IsSuccessStatusCode) {
                    if (apiResponse.StatusCode == HttpStatusCode.Created && apiResponse.Headers.Location is not null && typeof(TSuccess) == typeof(string)) {
                        return TnTResult.Success((TSuccess)Convert.ChangeType(apiResponse.Headers.Location.ToString(), typeof(TSuccess)));
                    }
                    else {
                        return TnTResult.Success(apiResponse.Content!);
                    }
                }
                else {
                    if(apiResponse.ContentHeaders?.ContentType?.ToString().Equals("application/problem+json", StringComparison.OrdinalIgnoreCase) == true && apiResponse.Error.Content is not null) {
                        var awaitable = apiResponse.Error.GetContentAsAsync<ProblemDetails>();
                        awaitable.Wait();
                        var problemDetails = awaitable.Result;
                        if(problemDetails is not null) {
                            return TnTResult.Failure<TSuccess>(new Exception(problemDetails.Detail ?? $"Failed with status code {apiResponse.StatusCode} {apiResponse.ReasonPhrase}"));
                        }
                    }

                    return apiResponse.StatusCode == HttpStatusCode.InternalServerError
                        ? TnTResult.Failure<TSuccess>(new Exception(apiResponse.Error.ReasonPhrase?.Trim('"') ?? $"Failed with status code {apiResponse.StatusCode} {apiResponse.ReasonPhrase}"))
                        : TnTResult.Failure<TSuccess>(new Exception(apiResponse.Error.Content?.Trim('"') ?? $"Failed with status code {apiResponse.StatusCode} {apiResponse.ReasonPhrase}"));
                }
            }
        }
        finally {
            apiResponse?.Dispose();
        }
    }

    /// <summary>
    ///     Converts an IApiResponse of type Stream to an ITnTResult of type TnTFileDownload.
    /// </summary>
    /// <param name="apiResponse">The API response.</param>
    /// <returns>The result of the operation.</returns>
    public static ITnTResult<TnTFileDownload> ToTnTResult(this IApiResponse<Stream> apiResponse) => new StreamApiResponseWrapper(apiResponse).ToTnTResult<TnTFileDownload>();

    /// <summary>
    ///     Asynchronously converts a Task of IApiResponse to an ITnTResult.
    /// </summary>
    /// <param name="task">The task representing the API response.</param>
    /// <returns>A task representing the result of the operation.</returns>
    public static async Task<ITnTResult> ToTnTResultAsync(this Task<IApiResponse> task) => await task.ContinueWith(t => {
        if (t.IsCanceled || t.Result.StatusCode == HttpStatusCode.RequestTimeout) {
            return TnTResult.Failure(new TaskCanceledException("The operation was cancelled"));
        }
        else {
            return t.Result.ToTnTResult();
        }
    }, TaskContinuationOptions.None);

    /// <summary>
    ///     Asynchronously converts a Task of IApiResponse of type TSuccess to an ITnTResult of type TSuccess.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="task">The task representing the API response.</param>
    /// <returns>A task representing the result of the operation.</returns>
    public static async Task<ITnTResult<TSuccess>> ToTnTResultAsync<TSuccess>(this Task<IApiResponse<TSuccess>> task) => await task.ContinueWith(t => {
        if (t.IsCanceled || t.Result.StatusCode == HttpStatusCode.RequestTimeout) {
            return TnTResult.Failure<TSuccess>(new TaskCanceledException("The operation was cancelled"));
        }
        else {
            return t.Result.ToTnTResult();
        }
    }, TaskContinuationOptions.None);

/// <summary>
///     Asynchronously converts a Task of IApiResponse of type Stream to an ITnTResult of type TnTFileDownload.
/// </summary>
/// <param name="task">The task representing the API response.</param>
/// <returns>A task representing the result of the operation.</returns>
public static async Task<ITnTResult<TnTFileDownload>> ToTnTResultAsync(this Task<IApiResponse<Stream>> task) => await task.ContinueWith(t => {
        if (t.IsCanceled || t.Result.StatusCode == HttpStatusCode.RequestTimeout) {
            return TnTResult.Failure<TnTFileDownload>(new TaskCanceledException("The operation was cancelled"));
        }
        else {
            return t.Result.ToTnTResult();
        }
    }, TaskContinuationOptions.None);

    /// <summary>
    ///     Internal implementation of IApiResponse for object type.
    /// </summary>
    /// <param name="apiResponse">The original API response.</param>
    private class PromotingApiResponse(IApiResponse apiResponse) : IApiResponse<int> {
        public int Content => 0;
        public HttpContentHeaders? ContentHeaders => apiResponse.ContentHeaders;
        public ApiException? Error => apiResponse.Error;
        public HttpResponseHeaders Headers => apiResponse.Headers;
        public bool IsSuccessful => apiResponse.IsSuccessful;
        public bool IsSuccessStatusCode => apiResponse.IsSuccessStatusCode;
        public string? ReasonPhrase => apiResponse.ReasonPhrase;
        public HttpRequestMessage? RequestMessage => apiResponse.RequestMessage;
        public HttpStatusCode StatusCode => apiResponse.StatusCode;
        public Version Version => apiResponse.Version;

        public void Dispose() => apiResponse.Dispose();
    }

    /// <summary>
    ///     Internal implementation of IApiResponse for TnTFileDownload type.
    /// </summary>
    /// <param name="apiResponse">The original API response.</param>
    private class StreamApiResponseWrapper(IApiResponse<Stream> apiResponse) : IApiResponse<TnTFileDownload> {

        public TnTFileDownload? Content { get; } = new() {
            Contents = apiResponse.Content!,
            Filename = apiResponse.ContentHeaders!.ContentDisposition?.FileNameStar ?? apiResponse.ContentHeaders.ContentDisposition?.FileName!,
            ContentType = apiResponse.ContentHeaders.ContentType?.MediaType!
        };

        public HttpContentHeaders? ContentHeaders => apiResponse.ContentHeaders;
        public ApiException? Error => apiResponse.Error;
        public HttpResponseHeaders Headers => apiResponse.Headers;
        public bool IsSuccessful => apiResponse.IsSuccessful;
        public bool IsSuccessStatusCode => apiResponse.IsSuccessStatusCode;
        public string? ReasonPhrase => apiResponse.ReasonPhrase;
        public HttpRequestMessage? RequestMessage => apiResponse.RequestMessage;
        public HttpStatusCode StatusCode => apiResponse.StatusCode;
        public Version Version => apiResponse.Version;
        public void Dispose() { }
    }
}