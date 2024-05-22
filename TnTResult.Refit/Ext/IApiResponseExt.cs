using Refit;
using System.Net;

namespace TnTResult.Refit.Ext;

public static class IApiResponseExt {

    /// <summary>
    /// Converts an <see cref="IApiResponse" /> to an <see cref="ITnTResult" />.
    /// </summary>
    /// <param name="apiResponse">The <see cref="IApiResponse" /> to convert.</param>
    /// <returns>An <see cref="ITnTResult" /> representing the conversion result.</returns>
    public static ITnTResult ToResult(this IApiResponse apiResponse) {
        if (apiResponse == null) {
            return ITnTResult.Failure("Failed with empty response from the server");
        }
        else {
            if (!apiResponse.IsSuccessStatusCode) {
                return ITnTResult.Failure(apiResponse.Error.Content?.Trim('"') ?? $"Failed with status code {apiResponse.StatusCode} {apiResponse.ReasonPhrase}");
            }

            return ITnTResult.Successful;
        }
    }

    /// <summary>
    /// Converts an <see cref="ApiResponse{TSuccess}" /> to an <see cref="ITnTResult{TSuccess}" />.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success content.</typeparam>
    /// <param name="apiResponse">The <see cref="ApiResponse{TSuccess}" /> to convert.</param>
    /// <returns>An <see cref="ITnTResult{TSuccess}" /> representing the conversion result.</returns>
    public static ITnTResult<TSuccess> ToResult<TSuccess>(this ApiResponse<TSuccess> apiResponse) {
        if (apiResponse?.IsSuccessStatusCode == true) {
            if (apiResponse.StatusCode == HttpStatusCode.Created && apiResponse.Headers.Location is not null && typeof(TSuccess) == typeof(string)) {
                return ITnTResult<TSuccess>.Success((TSuccess)Convert.ChangeType(apiResponse.Headers.Location.ToString(), typeof(TSuccess)));
            }

            return ITnTResult<TSuccess>.Success(apiResponse.Content);
        }
        else {
            var result = ((IApiResponse?)apiResponse)?.ToResult();
            return ITnTResult<TSuccess>.Failure(result?.Error ?? new Exception("An unknown error occurred"));
        }
    }

    /// <summary>
    /// Asynchronously converts a <see cref="Task{IApiResponse}" /> to an <see cref="ITnTResult" />.
    /// </summary>
    /// <param name="task">The <see cref="Task{IApiResponse}" /> to convert.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns an <see cref="ITnTResult" />.
    /// </returns>
    public static async Task<ITnTResult> ToResultAsync(this Task<IApiResponse> task) => (await task).ToResult();

    /// <summary>
    /// Asynchronously converts a <see cref="Task{ApiResponse{TSuccess}}" /> to an <see
    /// cref="ITnTResult{TSuccess}" />.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success content.</typeparam>
    /// <param name="func">The <see cref="Task{ApiResponse{TSuccess}}" /> to convert.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns an <see
    /// cref="ITnTResult{TSuccess}" />.
    /// </returns>
    public static async Task<ITnTResult<TSuccess>> ToResultAsync<TSuccess>(this Task<ApiResponse<TSuccess>> func) => (await func).ToResult<TSuccess>();
}

