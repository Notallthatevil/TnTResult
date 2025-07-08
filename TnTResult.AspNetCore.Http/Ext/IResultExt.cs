using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using TnTResult.Exceptions;

namespace TnTResult.AspNetCore.Http.Ext;

/// <summary>
///     Provides extension methods for converting <see cref="ITnTResult" /> and <see cref="ITnTResult{T}" /> instances to ASP.NET Core <see cref="IResult" /> objects for use in minimal APIs and MVC controllers.
/// </summary>
/// <remarks>
///     <para>
///         This class bridges the gap between functional result types ( <see cref="ITnTResult" />) and ASP.NET Core's HTTP result system. It automatically maps success and failure states to
///         appropriate HTTP status codes and response formats.
///     </para>
///     <para>The conversion methods handle various scenarios including:
///         <list type="bullet">
///             <item>
///                 <description>Different content types (JSON, text, files, streams)</description>
///             </item>
///             <item>
///                 <description>Custom HTTP status codes for success scenarios</description>
///             </item>
///             <item>
///                 <description>Automatic error mapping based on exception types</description>
///             </item>
///             <item>
///                 <description>File downloads with proper content disposition</description>
///             </item>
///             <item>
///                 <description>Asynchronous operations with proper ConfigureAwait usage</description>
///             </item>
///         </list>
///     </para>
/// </remarks>
/// <example>
///     <code>
///app.MapGet("/users/{id}", async (int id, IUserService userService) =&gt;
///{
///var result = await userService.GetUserAsync(id);
///return result.ToIResult();
///});
///
///
///app.MapPost("/users", async (CreateUserRequest request, IUserService userService) =&gt;
///{
///var result = await userService.CreateUserAsync(request);
///return result.ToIResult(uri: $"/users/{result.Value?.Id}", successStatusCode: HttpStatusCode.Created);
///});
///
///
///app.MapGet("/files/{id}/download", async (int id, IFileService fileService) =&gt;
///{
///var result = await fileService.GetFileAsync(id);
///return result.ToIResult();
///});
///     </code>
/// </example>
public static class IResultExt {

    /// <summary>
    ///     Converts an <see cref="ITnTResult" /> to an <see cref="IResult" />.
    /// </summary>
    /// <param name="result">           The <see cref="ITnTResult" /> to convert.</param>
    /// <param name="content">          The optional content to include in the result. Can be any serializable object.</param>
    /// <param name="uri">              The optional URI to include in the result. Required for redirect status codes.</param>
    /// <param name="successStatusCode">The success status code to use in the result. Defaults to <see cref="HttpStatusCode.OK" />.</param>
    /// <returns>
    ///     An <see cref="IResult" /> that represents the converted result. Success results return the specified status code with optional content, while failure results are automatically mapped to
    ///     appropriate HTTP error status codes.
    /// </returns>
    /// <remarks>
    ///     <para>This method provides intelligent handling of different scenarios:
    ///         <list type="bullet">
    ///             <item>
    ///                 <description>If the result is already an <see cref="IHttpTnTResult" />, its existing <see cref="IResult" /> is returned directly</description>
    ///             </item>
    ///             <item>
    ///                 <description>For successful results with no content, returns 204 No Content (unless status is Created or Accepted)</description>
    ///             </item>
    ///             <item>
    ///                 <description>String content is returned as plain text, other content as JSON</description>
    ///             </item>
    ///             <item>
    ///                 <description>Redirect status codes require a valid URI parameter</description>
    ///             </item>
    ///             <item>
    ///                 <description>Failed results are mapped to appropriate HTTP error codes based on exception type</description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="result" /> is null.</exception>
    /// <example>
    ///     <code>
    ///var result = TnTResult.Success();
    ///return result.ToIResult(); // Returns 204 No Content
    ///
    ///var userResult = TnTResult.Success(new User { Id = 1, Name = "John" });
    ///return userResult.ToIResult(); // Returns 200 OK with JSON content
    ///
    ///return result.ToIResult(content: newUser, successStatusCode: HttpStatusCode.Created);
    ///
    ///return result.ToIResult(uri: "/new-location", successStatusCode: HttpStatusCode.Redirect);
    ///     </code>
    /// </example>
    public static IResult ToIResult(this ITnTResult result, object? content = null, string? uri = null, HttpStatusCode? successStatusCode = null) {
        // Fast path for HTTP results that already have an IResult
        if (result is IHttpTnTResult httpTnTResult && httpTnTResult.Result is not null) {
            return httpTnTResult.Result;
        }

        if (result.IsSuccessful) {
            // Handle no-content scenarios
            if (content is null && successStatusCode is null) {
                return TypedResults.NoContent();
            }

            successStatusCode ??= HttpStatusCode.OK;

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

        return CreateErrorResult(result.Error, result.ErrorMessage);
    }

    /// <summary>
    ///     Converts an <see cref="ITnTResult{TSuccess}" /> to an <see cref="IResult" />.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">           The <see cref="ITnTResult{TSuccess}" /> to convert.</param>
    /// <param name="uri">              The optional URI to include in the result. Required for redirect status codes.</param>
    /// <param name="successStatusCode">The success status code to use in the result. Defaults to <see cref="HttpStatusCode.OK" />.</param>
    /// <returns>
    ///     An <see cref="IResult" /> that represents the converted result. For successful results, the value is used as content. For failed results, appropriate HTTP error status codes are returned.
    /// </returns>
    /// <remarks>
    ///     This overload automatically uses the success value as the response content when the result is successful. If the result has failed, the value is ignored and error handling is applied.
    /// </remarks>
    /// <example>
    ///     <code>
    ///var userResult = await userService.GetUserAsync(id);
    ///return userResult.ToIResult(); // Returns user object as JSON on success, or appropriate error code on failure
    ///
    ///var createResult = await userService.CreateUserAsync(request);
    ///return createResult.ToIResult(uri: $"/users/{createResult.Value?.Id}", HttpStatusCode.Created);
    ///     </code>
    /// </example>
    public static IResult ToIResult<TSuccess>(this ITnTResult<TSuccess> result, string? uri = null, HttpStatusCode? successStatusCode = null) => result.ToIResult(result.IsSuccessful ? result.Value : null, uri, successStatusCode);

    /// <summary>
    ///     Converts an <see cref="ITnTResult{Stream}" /> to an <see cref="IResult" /> for file streaming scenarios.
    /// </summary>
    /// <param name="result">          The <see cref="ITnTResult{Stream}" /> to convert.</param>
    /// <param name="contentType">     The MIME content type of the stream (e.g., "application/pdf", "image/jpeg", "text/plain").</param>
    /// <param name="fileDownloadName">The suggested filename for download. This will be used in the Content-Disposition header.</param>
    /// <returns>An <see cref="IResult" /> that serves the stream as a file download on success, or appropriate error codes on failure.</returns>
    /// <remarks>
    ///     <para>
    ///         This method is specifically designed for streaming file content to HTTP responses. The stream will be automatically disposed after the response is sent. If the stream supports seeking,
    ///         it will be reset to position 0 before serving.
    ///     </para>
    ///     <para>The Content-Disposition header will be set to suggest the provided filename to the browser for downloads.</para>
    /// </remarks>
    /// <example>
    ///     <code>
    ///var fileResult = await fileService.GetFileStreamAsync(fileId);
    ///return fileResult.ToIResult("application/pdf", "document.pdf");
    ///     </code>
    /// </example>
    public static IResult ToIResult(this ITnTResult<Stream> result, string? contentType, string? fileDownloadName) => result.IsSuccessful ? TypedResults.File(result.Value!, contentType, fileDownloadName) : CreateErrorResult(result.Error, result.ErrorMessage);

    /// <summary>
    ///     Converts an <see cref="ITnTResult{TnTFileDownload}" /> to an <see cref="IResult" /> with automatic content type detection.
    /// </summary>
    /// <param name="result">The <see cref="ITnTResult{TnTFileDownload}" /> to convert.</param>
    /// <returns>An <see cref="IResult" /> that serves the file download on success, or appropriate error codes on failure.</returns>
    /// <remarks>
    ///     <para>This method handles different types of file content sources:
    ///         <list type="bullet">
    ///             <item>
    ///                 <description><strong>Stream:</strong> Serves the stream directly with automatic position reset</description>
    ///             </item>
    ///             <item>
    ///                 <description><strong>URL:</strong> Returns a redirect to the file URL</description>
    ///             </item>
    ///             <item>
    ///                 <description><strong>Byte Array:</strong> Serves the byte array as file content</description>
    ///             </item>
    ///             <item>
    ///                 <description><strong>Null/Empty:</strong> Returns 204 No Content</description>
    ///             </item>
    ///         </list>
    ///     </para>
    ///     <para>The content type and filename are automatically extracted from the <see cref="TnTFileDownload" /> object, eliminating the need to specify them manually.</para>
    /// </remarks>
    /// <example>
    ///     <code>
    ///var fileResult = await fileService.GetFileDownloadAsync(fileId);
    ///return fileResult.ToIResult(); // Automatically handles content type, filename, and file data
    ///     </code>
    /// </example>
    public static IResult ToIResult(this ITnTResult<TnTFileDownload> result) {
        if (!result.IsSuccessful) {
            return CreateErrorResult(result.Error, result.ErrorMessage);
        }

        var fileDownload = result.Value;
        if (fileDownload?.Contents is null) {
            return TypedResults.NoContent();
        }

        return fileDownload.Contents switch {
            { IsStream: true } when fileDownload.Contents.Stream is not null => CreateFileStreamResult(fileDownload.Contents.Stream, fileDownload.ContentType, fileDownload.Filename),
            { IsUrl: true } => TypedResults.Text(fileDownload.Contents.Url!),
            { IsByteArray: true } when fileDownload.Contents.ByteArray is not null => TypedResults.File(fileDownload.Contents.ByteArray, fileDownload.ContentType, fileDownload.Filename),
            _ => TypedResults.NoContent()
        };
    }

    /// <summary>
    ///     Converts a <see cref="Task{ITnTResult}" /> to an <see cref="IResult" /> asynchronously with proper async/await patterns.
    /// </summary>
    /// <param name="task">             The <see cref="Task{ITnTResult}" /> to convert.</param>
    /// <param name="content">          The optional content to include in the result. Can be any serializable object.</param>
    /// <param name="uri">              The optional URI to include in the result. Required for redirect status codes.</param>
    /// <param name="successStatusCode">The success status code to use in the result. Defaults to <see cref="HttpStatusCode.OK" />.</param>
    /// <returns>A <see cref="Task{IResult}" /> that represents the asynchronous conversion operation.</returns>
    /// <remarks>
    ///     <para>
    ///         This method uses proper async/await patterns with ConfigureAwait(false) for optimal performance in library scenarios. It waits for the task to complete and then applies the same
    ///         conversion logic as the synchronous version.
    ///     </para>
    ///     <para>This is the preferred method for converting asynchronous operations that return <see cref="ITnTResult" />.</para>
    /// </remarks>
    /// <example>
    ///     <code>
    ///app.MapGet("/users", async (IUserService userService) =&gt;
    ///{
    ///var usersTask = userService.GetAllUsersAsync();
    ///return await usersTask.ToIResultAsync();
    ///});
    ///
    ///app.MapGet("/users", (IUserService userService) =&gt;
    ///userService.GetAllUsersAsync().ToIResultAsync());
    ///     </code>
    /// </example>
    public static async Task<IResult> ToIResultAsync(this Task<ITnTResult> task, object? content = null, string? uri = null, HttpStatusCode? successStatusCode = null) {
        var result = await task.ConfigureAwait(false);
        return result.ToIResult(content, uri, successStatusCode);
    }

    /// <summary>
    ///     Converts a <see cref="Task{ITnTResult{TSuccess}}" /> to an <see cref="IResult" /> asynchronously with proper async/await patterns.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="task">             The <see cref="Task{ITnTResult{TSuccess}}" /> to convert.</param>
    /// <param name="uri">              The optional URI to include in the result. Required for redirect status codes.</param>
    /// <param name="successStatusCode">The success status code to use in the result. Defaults to <see cref="HttpStatusCode.OK" />.</param>
    /// <returns>A <see cref="Task{IResult}" /> that represents the asynchronous conversion operation.</returns>
    /// <remarks>
    ///     This method waits for the task to complete and then automatically uses the success value as the response content when the result is successful. Uses ConfigureAwait(false) for optimal
    ///     library performance.
    /// </remarks>
    /// <example>
    ///     <code>
    ///app.MapGet("/users/{id}", (int id, IUserService userService) =&gt;
    ///userService.GetUserAsync(id).ToIResultAsync());
    ///
    ///app.MapPost("/users", (CreateUserRequest request, IUserService userService) =&gt;
    ///userService.CreateUserAsync(request).ToIResultAsync(successStatusCode: HttpStatusCode.Created));
    ///     </code>
    /// </example>
    public static async Task<IResult> ToIResultAsync<TSuccess>(this Task<ITnTResult<TSuccess>> task, string? uri = null, HttpStatusCode? successStatusCode = null) {
        var result = await task.ConfigureAwait(false);
        return result.ToIResult(uri, successStatusCode);
    }

    /// <summary>
    ///     Converts a <see cref="Task{ITnTResult{Stream}}" /> to an <see cref="IResult" /> asynchronously for file streaming scenarios.
    /// </summary>
    /// <param name="task">            The <see cref="Task{ITnTResult{Stream}}" /> to convert.</param>
    /// <param name="contentType">     The MIME content type of the stream (e.g., "application/pdf", "image/jpeg", "text/plain").</param>
    /// <param name="fileDownloadName">The suggested filename for download. This will be used in the Content-Disposition header.</param>
    /// <returns>A <see cref="Task{IResult}" /> that represents the asynchronous conversion operation.</returns>
    /// <remarks>
    ///     This method waits for the task to complete and then serves the stream as a file download on success. Uses ConfigureAwait(false) for optimal library performance and automatically handles
    ///     stream positioning.
    /// </remarks>
    /// <example>
    ///     <code>
    ///app.MapGet("/files/{id}/download", (int id, IFileService fileService) =&gt;
    ///fileService.GetFileStreamAsync(id).ToIResultAsync("application/pdf", "document.pdf"));
    ///     </code>
    /// </example>
    public static async Task<IResult> ToIResultAsync(this Task<ITnTResult<Stream>> task, string? contentType, string? fileDownloadName) {
        var result = await task.ConfigureAwait(false);
        return result.ToIResult(contentType, fileDownloadName);
    }

    /// <summary>
    ///     Converts a <see cref="Task{ITnTResult{TnTFileDownload}}" /> to an <see cref="IResult" /> asynchronously with automatic content handling.
    /// </summary>
    /// <param name="task">The <see cref="Task{ITnTResult{TnTFileDownload}}" /> to convert.</param>
    /// <returns>A <see cref="Task{IResult}" /> that represents the asynchronous conversion operation.</returns>
    /// <remarks>
    ///     <para>This method waits for the task to complete and then handles different types of file content sources automatically. Uses ConfigureAwait(false) for optimal library performance.</para>
    ///     <para>The content type, filename, and file data are all extracted from the <see cref="TnTFileDownload" /> object, providing a seamless file serving experience.</para>
    /// </remarks>
    /// <example>
    ///     <code>
    ///app.MapGet("/files/{id}", (int id, IFileService fileService) =&gt;
    ///fileService.GetFileDownloadAsync(id).ToIResultAsync());
    ///     </code>
    /// </example>
    public static async Task<IResult> ToIResultAsync(this Task<ITnTResult<TnTFileDownload>> task) {
        var result = await task.ConfigureAwait(false);
        return result.ToIResult();
    }

    /// <summary>
    ///     Creates an appropriate <see cref="IResult" /> for error scenarios based on the exception type.
    /// </summary>
    /// <param name="error">       The exception that caused the failure.</param>
    /// <param name="errorMessage">The error message to include in the response.</param>
    /// <returns>
    ///     An <see cref="IResult" /> with the appropriate HTTP status code:
    ///     <list type="bullet">
    ///         <item>
    ///             <description><see cref="NotFoundException" /> → 404 Not Found</description>
    ///         </item>
    ///         <item>
    ///             <description><see cref="UnauthorizedAccessException" /> → 401 Unauthorized</description>
    ///         </item>
    ///         <item>
    ///             <description><see cref="TaskCanceledException" /> or <see cref="OperationCanceledException" /> → 408 Request Timeout</description>
    ///         </item>
    ///         <item>
    ///             <description><see cref="ForbiddenException" /> → 403 Forbidden</description>
    ///         </item>
    ///         <item>
    ///             <description>All other exceptions → 400 Bad Request</description>
    ///         </item>
    ///     </list>
    /// </returns>
    private static IResult CreateErrorResult(Exception error, string errorMessage) {
        return error switch {
            NotFoundException => TypedResults.NotFound(errorMessage),
            UnauthorizedAccessException => TypedResults.Unauthorized(),
            TaskCanceledException or OperationCanceledException => TypedResults.StatusCode(StatusCodes.Status408RequestTimeout),
            ForbiddenException => TypedResults.Forbid(),
            _ => TypedResults.BadRequest(errorMessage)
        };
    }

    /// <summary>
    ///     Creates a file stream result with proper stream positioning and content type handling.
    /// </summary>
    /// <param name="stream">     The stream containing the file data.</param>
    /// <param name="contentType">The MIME content type of the file (e.g., "application/pdf", "image/jpeg").</param>
    /// <param name="filename">   The suggested filename for download. This will be used in the Content-Disposition header.</param>
    /// <returns>A <see cref="FileStreamHttpResult" /> that can be returned from an ASP.NET Core endpoint to serve the file.</returns>
    /// <remarks>
    ///     This method automatically resets the stream position to the beginning if the stream supports seeking. This ensures that the entire file content is served, regardless of the current stream position.
    /// </remarks>
    private static FileStreamHttpResult CreateFileStreamResult(Stream stream, string? contentType, string? filename) {
        // Reset stream position if possible
        if (stream.CanSeek) {
            stream.Seek(0, SeekOrigin.Begin);
        }
        return TypedResults.File(stream, contentType, filename);
    }
}