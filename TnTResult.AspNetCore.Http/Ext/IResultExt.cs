using Microsoft.AspNetCore.Http;
using System.Net;
using System.Reflection.Metadata;
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
    public static IResult ToIResult(this ITnTResult result, object? content = null, string? uri = null, HttpStatusCode successStatusCode = HttpStatusCode.OK) {
        if(result is HttpTnTResult httpTnTResult) {
            return httpTnTResult.Result;
        }


        if (result.IsSuccessful) {
            if (content is null && successStatusCode != HttpStatusCode.Created && successStatusCode != HttpStatusCode.Accepted) {
                return TypedResults.NoContent();
            }

            return successStatusCode switch {
                HttpStatusCode.OK => content is string str ? TypedResults.Text(str, "text/plain") : TypedResults.Ok(content),
                HttpStatusCode.Redirect => TypedResults.Redirect(uri!),
                HttpStatusCode.TemporaryRedirect => TypedResults.Redirect(uri!, false),
                HttpStatusCode.PermanentRedirect => TypedResults.Redirect(uri!, true),
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
                TaskCanceledException or OperationCanceledException => TypedResults.StatusCode(StatusCodes.Status408RequestTimeout),
                ForbiddenException => TypedResults.Forbid(),
                _ => TypedResults.BadRequest(result.ErrorMessage)
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
    public static IResult ToIResult<TSuccess>(this ITnTResult<TSuccess> result, string? uri = null, HttpStatusCode successStatusCode = HttpStatusCode.OK) =>
        result.ToIResult(result.IsSuccessful ? result.Value : null, uri, successStatusCode);

    /// <summary>
    /// Converts an <see cref="ITnTResult{Stream}" /> to an <see cref="IResult" />.
    /// </summary>
    /// <param name="result">The <see cref="ITnTResult{Stream}" /> to convert.</param>
    /// <param name="contentType">The content type of the stream.</param>
    /// <param name="fileDownloadName">The file download name.</param>
    /// <returns>The converted <see cref="IResult" />.</returns>
    public static IResult ToIResult(this ITnTResult<Stream> result, string? contentType, string? fileDownloadName) =>
        result.IsSuccessful ? Results.File(result.Value!, contentType, fileDownloadName) : result.ToIResult(uri: null);

    /// <summary>
    /// Converts an <see cref="ITnTResult{TnTFileStream}" /> to an <see cref="IResult" />.
    /// </summary>
    /// <param name="result">The <see cref="ITnTResult{TnTFileStream}" /> to convert.</param>
    /// <returns>The converted <see cref="IResult" />.</returns>
    public static IResult ToIResult(this ITnTResult<TnTFileDownload> result) {
        if (result.IsSuccessful) {
            if (result.Value.Contents.IsStream) {
                return Results.File(result.Value.Contents.Stream!, result.Value.ContentType, result.Value.Filename);
            }
            else if (result.Value.Contents.IsUrl) {
                return result.ToIResult(result.Value.Contents.Url);
            }
            else if (result.Value.Contents.IsByteArray) {
                return result.ToIResult(result.Value.Contents.ByteArray!, result.Value.ContentType);
            }
        }
        return TnTResult.Failure(result.Error).ToIResult();
    }

    /// <summary>
    /// Converts a <see cref="Task{ITnTResult}" /> to an <see cref="IResult" /> asynchronously.
    /// </summary>
    /// <param name="task">The <see cref="Task{ITnTResult}" /> to convert.</param>
    /// <param name="content">The optional content to include in the result.</param>
    /// <param name="uri">The optional URI to include in the result.</param>
    /// <param name="successStatusCode">The success status code to use in the result.</param>
    /// <returns>The converted <see cref="IResult" />.</returns>
    public static Task<IResult> ToIResultAsync(this Task<ITnTResult> task, object? content = null, string? uri = null, HttpStatusCode successStatusCode = HttpStatusCode.OK) => task.ContinueWith(t => t.Result.ToIResult(content, uri, successStatusCode));

    /// <summary>
    /// Converts a <see cref="Task{ITnTResult{TSuccess}}" /> to an <see cref="IResult" /> asynchronously.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="task">The <see cref="Task{ITnTResult{TSuccess}}" /> to convert.</param>
    /// <param name="uri">The optional URI to include in the result.</param>
    /// <param name="successStatusCode">The success status code to use in the result.</param>
    /// <returns>The converted <see cref="IResult" />.</returns>
    public static Task<IResult> ToIResultAsync<TSuccess>(this Task<ITnTResult<TSuccess>> task, string? uri = null, HttpStatusCode successStatusCode = HttpStatusCode.OK) => task.ContinueWith(t => t.Result.ToIResult(uri, successStatusCode));

    /// <summary>
    /// Converts a <see cref="Task{ITnTResult{Stream}}" /> to an <see cref="IResult" /> asynchronously.
    /// </summary>
    /// <param name="task">The <see cref="Task{ITnTResult{Stream}}" /> to convert.</param>
    /// <param name="contentType">The content type of the stream.</param>
    /// <param name="fileDownloadName">The file download name.</param>
    /// <returns>The converted <see cref="IResult" />.</returns>
    public static Task<IResult> ToIResultAsync(this Task<ITnTResult<Stream>> task, string? contentType, string? fileDownloadName) => task.ContinueWith(t => t.Result.ToIResult(contentType, fileDownloadName));

    /// <summary>
    /// Converts a <see cref="Task{ITnTResult{TnTFileStream}}" /> to an <see cref="IResult" /> asynchronously.
    /// </summary>
    /// <param name="task">The <see cref="Task{ITnTResult{TnTFileStream}}" /> to convert.</param>
    /// <returns>The converted <see cref="IResult" />.</returns>
    public static Task<IResult> ToIResultAsync(this Task<ITnTResult<TnTFileDownload>> task) => task.ContinueWith(t => t.Result.ToIResult());
}