using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TnTResult.Exceptions;

namespace TnTResult.AspNetCore.Http.Ext;
public static class IResultExt {

    /// <summary>
    /// Converts an <see cref="ITnTResult" /> to an <see cref="IResult" />.
    /// </summary>
    /// <param name="result">The <see cref="ITnTResult" /> to convert.</param>
    /// <param name="content">The optional content to include in the result.</param>
    /// <param name="uri">The optional URI to include in the result.</param>
    /// <param name="successStatusCode">The success status code to use in the result.</param>
    /// <returns>The converted <see cref="IResult" />.</returns>
    public static IResult ToResult(this ITnTResult result, object? content = null, string? uri = null, HttpStatusCode successStatusCode = HttpStatusCode.OK) {
        if (result.IsSuccessful) {
            if (content is null && successStatusCode != HttpStatusCode.Created && successStatusCode != HttpStatusCode.Accepted) {
                return TypedResults.NoContent();
            }

            if (result is CreatedTnTResult<string> created) {
                return TypedResults.Created(created.Value, content);
            }
            else if (result is CreatedTnTResult) {
                return TypedResults.Created(uri, content);
            }

            return successStatusCode switch {
                HttpStatusCode.OK => TypedResults.Ok(content),
                HttpStatusCode.Created => TypedResults.Created(uri, content),
                HttpStatusCode.Accepted => TypedResults.Accepted(uri, content),
                HttpStatusCode.NoContent => TypedResults.NoContent(),
                _ => TypedResults.Json(content, statusCode: (int)successStatusCode),
            };
        }
        else {
            return result.Error switch {
                NotFoundException => TypedResults.NotFound(result.ErrorMessage),
                UnauthorizedAccessException => TypedResults.Unauthorized(),
                _ => TypedResults.BadRequest()
            };
        }
    }

    /// <summary>
    /// Converts an <see cref="ITnTResult{TSuccess}" /> to an <see cref="IResult" />.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The <see cref="ITnTResult{TSuccess}" /> to convert.</param>
    /// <param name="uri">The optional URI to include in the result.</param>
    /// <param name="successStatusCode">The success status code to use in the result.</param>
    /// <returns>The converted <see cref="IResult" />.</returns>
    public static IResult ToResult<TSuccess>(this ITnTResult<TSuccess> result, string? uri = null, HttpStatusCode successStatusCode = HttpStatusCode.OK) => result.ToResult(result.IsSuccessful ? result.Value : null, uri, successStatusCode);

    public static IResult ToResult(this ITnTResult<Stream> result, string? contentType, string? fileDownloadName) {
        if (result.IsSuccessful) {
            return Results.File(result.Value!, contentType, fileDownloadName);
        }
        else {
            return result.Error switch {
                NotFoundException => Results.NotFound(result.ErrorMessage),
                UnauthorizedAccessException => Results.Unauthorized(),
                _ => Results.BadRequest()
            };
        }
    }

    /// <summary>
    /// Converts a <see cref="Task{ITnTResult}" /> to an <see cref="IResult" /> asynchronously.
    /// </summary>
    /// <param name="task">The <see cref="Task{ITnTResult}" /> to convert.</param>
    /// <param name="content">The optional content to include in the result.</param>
    /// <param name="uri">The optional URI to include in the result.</param>
    /// <param name="successStatusCode">The success status code to use in the result.</param>
    /// <returns>The converted <see cref="IResult" />.</returns>
    public static async Task<IResult> ToResultAsync(this Task<ITnTResult> task, object? content = null, string? uri = null, HttpStatusCode successStatusCode = HttpStatusCode.OK) => (await task).ToResult(content, uri, successStatusCode);

    /// <summary>
    /// Converts a <see cref="Task{ITnTResult{TSuccess}}" /> to an <see cref="IResult" /> asynchronously.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="task">The <see cref="Task{ITnTResult{TSuccess}}" /> to convert.</param>
    /// <param name="uri">The optional URI to include in the result.</param>
    /// <param name="successStatusCode">The success status code to use in the result.</param>
    /// <returns>The converted <see cref="IResult" />.</returns>
    public static async Task<IResult> ToResultAsync<TSuccess>(this Task<ITnTResult<TSuccess>> task, string? uri = null, HttpStatusCode successStatusCode = HttpStatusCode.OK) => (await task).ToResult(uri, successStatusCode);

    public static async Task<IResult> ToResultAsync(this Task<ITnTResult<Stream>> task, string? contentType, string? fileDownloadName) => (await task).ToResult(contentType, fileDownloadName);
}

