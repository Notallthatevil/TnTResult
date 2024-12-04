using Refit;
using System.Net;

namespace TnTResult.Refit.Ext;

public static class IApiResponseExt {

    /// <summary>
    /// Converts an <see cref="IApiResponse" /> to an <see cref="ITnTResult" />.
    /// </summary>
    /// <param name="apiResponse">The <see cref="IApiResponse" /> to convert.</param>
    /// <returns>An <see cref="ITnTResult" /> representing the conversion result.</returns>
    public static ITnTResult ToTnTResult(this IApiResponse apiResponse) {
        if (apiResponse == null) {
            return ITnTResult.Failure("Failed with empty response from the server");
        }
        else {
            if (!apiResponse.IsSuccessStatusCode) {
                if (apiResponse.StatusCode == HttpStatusCode.InternalServerError) {
                    return ITnTResult.Failure(apiResponse.Error.ReasonPhrase?.Trim('"') ?? $"Failed with status code {apiResponse.StatusCode} {apiResponse.ReasonPhrase}");
                }
                else {
                    return ITnTResult.Failure(apiResponse.Error.Content?.Trim('"') ?? $"Failed with status code {apiResponse.StatusCode} {apiResponse.ReasonPhrase}");
                }
            }

            return ITnTResult.Successful;
        }
    }

    /// <summary>
    /// Converts an <see cref="IApiResponse{TSuccess}" /> to an <see cref="ITnTResult{TSuccess}" />.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success content.</typeparam>
    /// <param name="apiResponse">The <see cref="IApiResponse{TSuccess}" /> to convert.</param>
    /// <returns>An <see cref="ITnTResult{TSuccess}" /> representing the conversion result.</returns>
    public static ITnTResult<TSuccess> ToTnTResult<TSuccess>(this IApiResponse<TSuccess> apiResponse) {
        if (apiResponse?.IsSuccessStatusCode == true) {
            if (apiResponse.StatusCode == HttpStatusCode.Created && apiResponse.Headers.Location is not null && typeof(TSuccess) == typeof(string)) {
                return ITnTResult<TSuccess>.Success((TSuccess)Convert.ChangeType(apiResponse.Headers.Location.ToString(), typeof(TSuccess)));
            }

            return ITnTResult<TSuccess>.Success(apiResponse.Content);
        }
        else {
            var result = ((IApiResponse?)apiResponse)?.ToTnTResult();
            return ITnTResult<TSuccess>.Failure(result?.Error ?? new Exception("An unknown error occurred"));
        }
    }

    /// <summary>
    /// Converts an <see cref="IApiResponse{Stream}" /> to an <see cref="ITnTResult{TnTFileStream}" />.
    /// </summary>
    /// <param name="apiResponse">The <see cref="IApiResponse{Stream}" /> to convert.</param>
    /// <returns>An <see cref="ITnTResult{TnTFileStream}" /> representing the conversion result.</returns>
    public static ITnTResult<TnTFileDownload> ToTnTResult(this IApiResponse<Stream> apiResponse) {
        if (apiResponse is not null && apiResponse.IsSuccessStatusCode == true) {
            return ITnTResult<TnTFileDownload>.Success(new TnTFileDownload {
                Contents = apiResponse.Content!,
                Filename = apiResponse.ContentHeaders.ContentDisposition?.FileNameStar ?? apiResponse.ContentHeaders.ContentDisposition?.FileName!,
                ContentType = apiResponse.ContentHeaders.ContentType?.MediaType!
            });
        }
        else {
            var result = ((IApiResponse?)apiResponse)?.ToTnTResult();
            return ITnTResult<TnTFileDownload>.Failure(result?.Error ?? new Exception("An unknown error occurred"));
        }
    }

    /// <summary>
    /// Asynchronously converts a <see cref="Task{IApiResponse}" /> to an <see cref="ITnTResult" />.
    /// </summary>
    /// <param name="task">The <see cref="Task{IApiResponse}" /> to convert.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns an <see cref="ITnTResult" />.
    /// </returns>
    public static async Task<ITnTResult> ToTnTResultAsync(this Task<IApiResponse> task) => (await task).ToTnTResult();

    /// <summary>
    /// Asynchronously converts a <see cref="Task{IApiResponse{TSuccess}}" /> to an <see
    /// cref="ITnTResult{TSuccess}" />.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success content.</typeparam>
    /// <param name="task">The <see cref="Task{IApiResponse{TSuccess}}" /> to convert.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns an <see
    /// cref="ITnTResult{TSuccess}" />.
    /// </returns>
    public static async Task<ITnTResult<TSuccess>> ToTnTResultAsync<TSuccess>(this Task<IApiResponse<TSuccess>> task) => (await task).ToTnTResult();

    /// <summary>
    /// Asynchronously converts a <see cref="Task{IApiResponse{Stream}}" /> to an <see
    /// cref="ITnTResult{TnTFileStream}" />.
    /// </summary>
    /// <param name="task">The <see cref="Task{IApiResponse{Stream}}" /> to convert.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns an <see
    /// cref="ITnTResult{TnTFileStream}" />.
    /// </returns>
    public static async Task<ITnTResult<TnTFileDownload>> ToTnTResultAsync(this Task<IApiResponse<Stream>> task) => (await task).ToTnTResult();

    /// <summary>
    /// Asynchronously converts a <see cref="Task{ApiResponse{TSuccess}}" /> to an <see
    /// cref="ITnTResult{TSuccess}" />.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success content.</typeparam>
    /// <param name="task">The <see cref="Task{ApiResponse{TSuccess}}" /> to convert.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns an <see
    /// cref="ITnTResult{TSuccess}" />.
    /// </returns>
    public static async Task<ITnTResult<TSuccess>> ToTnTResultAsync<TSuccess>(this Task<ApiResponse<TSuccess>> task) => (await task).ToTnTResult();

    /// <summary>
    /// Asynchronously converts a <see cref="Task{ApiResponse{Stream}}" /> to an <see
    /// cref="ITnTResult{TnTFileStream}" />.
    /// </summary>
    /// <param name="task">The <see cref="Task{ApiResponse{Stream}}" /> to convert.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns an <see
    /// cref="ITnTResult{TnTFileStream}" />.
    /// </returns>
    public static async Task<ITnTResult<TnTFileDownload>> ToTnTResultAsync(this Task<ApiResponse<Stream>> task) => (await task).ToTnTResult();
}