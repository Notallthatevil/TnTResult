using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TnTResult.AspNetCore.Http.Ext;
using TnTResult.Ext;
using TnTResult.Exceptions;

namespace TnTResult.AspNetCore.Http;

/// <summary>
///     Represents an HTTP result that implements ASP.NET Core's IResult interface.
/// </summary>
public interface IHttpTnTResult : IResult {

    /// <summary>
    ///     Gets the underlying IResult implementation.
    /// </summary>
    /// <value>The IResult that will be executed when this HTTP result is processed.</value>
    IResult Result { get; }
}

/// <summary>
///     Represents an HTTP result for TnT operations that can be either successful or contain an error. This class implements both ITnTResult for functional error handling and IResult for ASP.NET Core integration.
/// </summary>
/// <remarks>
///     This class provides a way to handle operations that may succeed or fail while integrating seamlessly with ASP.NET Core's minimal APIs and MVC controllers. It wraps standard HTTP responses and
///     provides functional programming patterns for error handling.
/// </remarks>
public sealed class HttpTnTResult : ITnTResult, IHttpTnTResult {

    /// <summary>
    ///     Gets a successful result instance that can be reused for successful operations without errors.
    /// </summary>
    /// <value>A static instance representing a successful operation.</value>
    public static readonly HttpTnTResult Successful = new();

    /// <inheritdoc />
    /// <summary>
    ///     Gets the exception that caused the failure, if any.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when accessed on a successful result.</exception>
    public Exception Error => _error.Value;

    /// <inheritdoc />
    /// <summary>
    ///     Gets the error message from the exception that caused the failure.
    /// </summary>
    /// <value>The error message, or throws an exception if the result is successful.</value>
    public string ErrorMessage => Error.Message;

    /// <inheritdoc />
    /// <summary>
    ///     Gets a value indicating whether the operation was successful.
    /// </summary>
    /// <value><c>true</c> if the operation succeeded; otherwise, <c>false</c>.</value>
    public bool IsSuccessful => !_error.HasValue;

    /// <summary>
    ///     Gets a value indicating whether the operation failed.
    /// </summary>
    /// <value><c>true</c> if the operation failed; otherwise, <c>false</c>.</value>
    public bool HasFailed => !IsSuccessful; private readonly Optional<Exception> _error;

    /// <summary>
    ///     Gets the underlying IResult that will be executed when this HTTP result is processed.
    /// </summary>
    /// <value>The IResult implementation that handles the HTTP response.</value>
    public IResult Result { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpTnTResult" /> class with the specified error.
    /// </summary>
    /// <param name="error">The error that occurred.</param>
    /// <param name="result">Optional explicit <see cref="IResult" /> to wrap; if null a default error mapping is generated.</param>
    private HttpTnTResult(Exception error, IResult? result = null) {
        ArgumentNullException.ThrowIfNull(error, nameof(error));
        _error = Optional.MakeOptional(error);
        Result = result ?? this.ToIResult();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpTnTResult" /> class representing a successful operation.
    /// </summary>
    private HttpTnTResult() {
        _error = Optional<Exception>.NullOpt;
        Result = this.ToIResult();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpTnTResult" /> class with the specified result.
    /// </summary>
    /// <param name="result">The IResult to wrap.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="result" /> is null.</exception>
    private HttpTnTResult(IResult result) {
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        _error = Optional<Exception>.NullOpt;
        Result = result;
    }

    /// <summary>
    ///     Creates an HTTP 202 Accepted response.
    /// </summary>
    /// <returns>A new <see cref="HttpTnTResult" /> representing an accepted request.</returns>
    public static HttpTnTResult Accepted() => new(TypedResults.Accepted((string?)null));

    /// <summary>
    ///     Creates an HTTP 201 Created response.
    /// </summary>
    /// <returns>A new <see cref="HttpTnTResult" /> representing a successful resource creation.</returns>
    public static HttpTnTResult Created() => new(TypedResults.Created());

    /// <summary>
    ///     Creates a custom error result with the specified exception and HTTP result.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="result">   The HTTP result to return.</param>
    /// <returns>A new <see cref="HttpTnTResult" /> representing the custom error.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="exception" /> is null.</exception>
    public static HttpTnTResult CustomError(Exception exception, IResult result) => new(exception, result);

    /// <summary>
    ///     Creates a custom successful result with the specified HTTP result.
    /// </summary>
    /// <param name="result">The HTTP result to wrap.</param>
    /// <returns>A new <see cref="HttpTnTResult" /> representing the custom result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="result" /> is null.</exception>
    public static HttpTnTResult CustomResult(IResult result) => new(result);

    /// <summary>
    ///     Creates an HTTP 204 No Content response.
    /// </summary>
    /// <returns>A new <see cref="HttpTnTResult" /> representing a successful operation with no content.</returns>
    public static HttpTnTResult NoContent() => new(TypedResults.NoContent());

    /// <summary>
    ///     Creates an HTTP redirect response to the specified URI.
    /// </summary>
    /// <param name="uri">           The URI to redirect to.</param>
    /// <param name="permanent">     Whether the redirect is permanent (301) or temporary (302).</param>
    /// <param name="preserveMethod">Whether to preserve the HTTP method in the redirect.</param>
    /// <returns>A new <see cref="HttpTnTResult" /> representing the redirect.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="uri" /> is null.</exception>
    public static HttpTnTResult Redirect(Uri uri, bool permanent = false, bool preserveMethod = false) => new(TypedResults.Redirect(uri.AbsoluteUri, permanent, preserveMethod));

    /// <summary>
    ///     Creates an HTTP redirect response to the specified URL.
    /// </summary>
    /// <param name="url">           The URL to redirect to.</param>
    /// <param name="permanent">     Whether the redirect is permanent (301) or temporary (302).</param>
    /// <param name="preserveMethod">Whether to preserve the HTTP method in the redirect.</param>
    /// <returns>A new <see cref="HttpTnTResult" /> representing the redirect.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="url" /> is null.</exception>
    public static HttpTnTResult Redirect(string url, bool permanent = false, bool preserveMethod = false) => new(TypedResults.Redirect(url, permanent, preserveMethod));

    /// <summary>
    ///     Creates an HTTP 404 Not Found response with the specified value.
    /// </summary>
    /// <param name="value">The optional value to include in the response body.</param>
    /// <returns>A new <see cref="HttpTnTResult" /> representing a not found error.</returns>
    public static HttpTnTResult NotFound<T>(T? value = default) => new(new NotFoundException(value?.ToString() ?? "Resource not found"), TypedResults.NotFound(value));

    /// <summary>
    ///     Creates an HTTP 400 Bad Request response with the specified error information.
    /// </summary>
    /// <param name="error">The optional error information to include in the response.</param>
    /// <returns>A new <see cref="HttpTnTResult" /> representing a bad request error.</returns>
    public static HttpTnTResult BadRequest<T>(T? error = default) => new(new ArgumentException(error?.ToString() ?? "Bad request"), TypedResults.BadRequest(error));

    /// <summary>
    ///     Creates an HTTP 401 Unauthorized response.
    /// </summary>
    /// <returns>A new <see cref="HttpTnTResult" /> representing an unauthorized access error.</returns>
    public static HttpTnTResult Unauthorized() => new(new UnauthorizedAccessException(), TypedResults.Unauthorized());

    /// <summary>
    ///     Creates an HTTP 403 Forbidden response.
    /// </summary>
    /// <returns>A new <see cref="HttpTnTResult" /> representing a forbidden access error.</returns>
    public static HttpTnTResult Forbid() => new(new ForbiddenException(), TypedResults.Forbid());

    /// <summary>
    ///     Creates a failure result with the specified error.
    /// </summary>
    /// <param name="error"> The error that occurred.</param>
    /// <param name="result">The optional HTTP result to return. If null, a default result will be generated.</param>
    /// <returns>A new instance of <see cref="HttpTnTResult" /> representing the failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="error" /> is null.</exception>
    public static HttpTnTResult Failure(Exception error, IResult? result = null) => new(error, result);

    /// <summary>
    ///     Creates a failure result with the specified error message.
    /// </summary>
    /// <param name="error"> The error message.</param>
    /// <param name="result">The optional HTTP result to return. If null, a default result will be generated.</param>
    /// <returns>A new instance of <see cref="HttpTnTResult" /> representing the failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="error" /> is null or empty.</exception>
    public static HttpTnTResult Failure(string error, IResult? result = null) => new(new Exception(error), result);

#if NET9_0_OR_GREATER
    /// <summary>
    ///     Creates an HTTP 500 Internal Server Error response with an optional message.
    /// </summary>
    /// <param name="message">The optional error message to include in the response.</param>
    /// <returns>A new <see cref="HttpTnTResult" /> representing an internal server error.</returns>
    public static HttpTnTResult InternalServerError(string? message = null) => new(new Exception(message), TypedResults.InternalServerError(message));

    /// <summary>
    ///     Creates an HTTP 500 Internal Server Error response with problem details.
    /// </summary>
    /// <param name="problemDetails">The problem details to include in the response.</param>
    /// <returns>A new <see cref="HttpTnTResult" /> representing an internal server error.</returns>
    public static HttpTnTResult InternalServerError(ProblemDetails? problemDetails) => new(new Exception(problemDetails?.Title ?? "An error occurred"), TypedResults.InternalServerError(problemDetails));
#else

    /// <summary>
    ///     Creates an HTTP 500 Internal Server Error response with an optional message.
    /// </summary>
    /// <param name="message">The optional error message to include in the response.</param>
    /// <returns>A new <see cref="HttpTnTResult" /> representing an internal server error.</returns>
    public static HttpTnTResult InternalServerError(string? message = null) => new(new Exception(message), TypedResults.StatusCode(StatusCodes.Status500InternalServerError));

#endif

    /// <summary>
    ///     Executes the result asynchronously by delegating to the underlying IResult implementation.
    /// </summary>
    /// <param name="httpContext">The HTTP context for the current request.</param>
    /// <returns>A task that represents the asynchronous execution operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpContext" /> is null.</exception>
    public Task ExecuteAsync(HttpContext httpContext) => Result.ExecuteAsync(httpContext);

    /// <summary>
    ///     Executes the specified action if the result represents a failure.
    /// </summary>
    /// <param name="action">The action to execute with the error information.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="action" /> is null.</exception>
    public ITnTResult OnFailure(Action<Exception> action) {
        if (!IsSuccessful) {
            action(Error);
        }
        return this;
    }

    /// <summary>
    ///     Executes the specified action if the result represents a success.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="action" /> is null.</exception>
    public ITnTResult OnSuccess(Action action) {
        if (IsSuccessful) {
            action();
        }
        return this;
    }

    /// <summary>
    ///     Executes the specified action regardless of whether the result is successful or not.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="action" /> is null.</exception>
    public void Finally(Action action) => action();
}

/// <summary>
///     Represents an HTTP result for TnT operations with a specific success value type. This class implements both ITnTResult&lt;TSuccess&gt; for functional error handling and IResult for ASP.NET
///     Core integration.
/// </summary>
/// <typeparam name="TSuccess">The type of the success value that this result can contain.</typeparam>
/// <remarks>
///     This generic version provides type-safe access to success values while maintaining the same functional programming patterns and ASP.NET Core integration as the non-generic version. Use this
///     when your operation returns a specific value type upon success.
/// </remarks>
public sealed class HttpTnTResult<TSuccess> : ITnTResult<TSuccess>, IHttpTnTResult {

    /// <inheritdoc />
    /// <summary>
    ///     Gets the exception that caused the failure, if any.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when accessed on a successful result.</exception>
    public Exception Error => _expected.Error;

    /// <inheritdoc />
    /// <summary>
    ///     Gets the error message from the exception that caused the failure.
    /// </summary>
    /// <value>The error message, or throws an exception if the result is successful.</value>
    public string ErrorMessage => Error.Message;

    /// <inheritdoc />
    /// <summary>
    ///     Gets a value indicating whether the operation was successful.
    /// </summary>
    /// <value><c>true</c> if the operation succeeded and contains a value; otherwise, <c>false</c>.</value>
    public bool IsSuccessful => _expected.HasValue;

    /// <inheritdoc />
    /// <summary>
    ///     Gets the success value if the operation was successful.
    /// </summary>
    /// <value>The success value of type <typeparamref name="TSuccess" />.</value>
    /// <exception cref="InvalidOperationException">Thrown when accessed on a failed result.</exception>
    public TSuccess Value => _expected.Value;

    /// <summary>
    ///     Gets a value indicating whether the operation failed.
    /// </summary>
    /// <value><c>true</c> if the operation failed; otherwise, <c>false</c>.</value>
    public bool HasFailed => !IsSuccessful;

    private readonly Expected<TSuccess, Exception> _expected;

    /// <summary>
    ///     Gets the underlying IResult that will be executed when this HTTP result is processed.
    /// </summary>
    /// <value>The IResult implementation that handles the HTTP response.</value>
    public IResult Result { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpTnTResult{TSuccess}" /> class with the specified error.
    /// </summary>
    /// <param name="error"> The error that occurred.</param>
    /// <param name="result">The optional HTTP result to return. If null, a default result will be generated.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="error" /> is null.</exception>
    internal HttpTnTResult(Exception error, IResult? result = null) {
        ArgumentNullException.ThrowIfNull(error, nameof(error));
        _expected = Expected.MakeUnexpected<TSuccess, Exception>(error);
        Result = result ?? this.ToIResult();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpTnTResult{TSuccess}" /> class with the specified success value and optional HTTP result.
    /// </summary>
    /// <param name="success">The success value to store.</param>
    /// <param name="result"> The optional HTTP result to return. If null, a default result will be generated based on the success value.</param>
    internal HttpTnTResult(TSuccess success, IResult? result = null) {
        _expected = Expected.MakeExpected<TSuccess, Exception>(success);

        Result = result ?? (this is ITnTResult<TnTFileDownload> fileDownloadResult
            ? fileDownloadResult.ToIResult()
            : this.ToIResult());
    }

    /// <summary>
    ///     Creates an HTTP 201 Created response with the specified value.
    /// </summary>
    /// <param name="value">The success value to include in the response.</param>
    /// <returns>A new instance of <see cref="HttpTnTResult{TSuccess}" /> representing the successful creation.</returns>
    public static HttpTnTResult<TSuccess> Created(TSuccess value) => new(value, TypedResults.Created((string?)null, value));

    /// <summary>
    ///     Creates an HTTP 201 Created response with the specified URI and value.
    /// </summary>
    /// <param name="uri">  The URI of the created resource.</param>
    /// <param name="value">The success value to include in the response.</param>
    /// <returns>A new instance of <see cref="HttpTnTResult{TSuccess}" /> representing the successful creation.</returns>
    public static HttpTnTResult<TSuccess> Created(string? uri, TSuccess value) => new(value, TypedResults.Created(uri, value));

    /// <summary>
    ///     Creates an HTTP 202 Accepted response with the specified value.
    /// </summary>
    /// <param name="value">The success value to include in the response.</param>
    /// <returns>A new instance of <see cref="HttpTnTResult{TSuccess}" /> representing the accepted request.</returns>
    public static HttpTnTResult<TSuccess> Accepted(TSuccess value) => new(value, TypedResults.Accepted((string?)null, value));

    /// <summary>
    ///     Creates an HTTP 200 OK response with the specified value.
    /// </summary>
    /// <param name="value">The success value to include in the response.</param>
    /// <returns>A new instance of <see cref="HttpTnTResult{TSuccess}" /> representing the successful operation.</returns>
    public static HttpTnTResult<TSuccess> Ok(TSuccess value) => new(value, TypedResults.Ok(value));

    /// <summary>
    ///     Creates a custom error result with the specified exception and HTTP result.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="result">   The HTTP result to return.</param>
    /// <returns>A new <see cref="HttpTnTResult{TSuccess}" /> representing the custom error.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="exception" /> is null.</exception>
    public static HttpTnTResult<TSuccess> CustomError(Exception exception, IResult result) => new(exception, result);

    /// <summary>
    ///     Creates a custom successful result with the specified value and HTTP result.
    /// </summary>
    /// <param name="success">The success value to store.</param>
    /// <param name="result"> The HTTP result to return.</param>
    /// <returns>A new <see cref="HttpTnTResult{TSuccess}" /> representing the custom result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="result" /> is null.</exception>
    public static HttpTnTResult<TSuccess> CustomResult(TSuccess success, IResult result) => new(success, result);

    /// <summary>
    ///     Creates an HTTP 404 Not Found response.
    /// </summary>
    /// <param name="value">The optional value to include in the response body.</param>
    /// <returns>A new <see cref="HttpTnTResult{TSuccess}" /> representing a not found error.</returns>
    public static HttpTnTResult<TSuccess> NotFound(object? value = null) => new(new NotFoundException(value?.ToString() ?? "Resource not found"));

    /// <summary>
    ///     Creates an HTTP 400 Bad Request response.
    /// </summary>
    /// <param name="error">The optional error information to include in the response.</param>
    /// <returns>A new <see cref="HttpTnTResult{TSuccess}" /> representing a bad request error.</returns>
    public static HttpTnTResult<TSuccess> BadRequest(object? error = null) => new(new ArgumentException(error?.ToString() ?? "Bad request"));

    /// <summary>
    ///     Creates an HTTP 401 Unauthorized response.
    /// </summary>
    /// <returns>A new <see cref="HttpTnTResult{TSuccess}" /> representing an unauthorized access error.</returns>
    public static HttpTnTResult<TSuccess> Unauthorized() => new(new UnauthorizedAccessException());

    /// <summary>
    ///     Creates an HTTP 403 Forbidden response.
    /// </summary>
    /// <returns>A new <see cref="HttpTnTResult{TSuccess}" /> representing a forbidden access error.</returns>
    public static HttpTnTResult<TSuccess> Forbid() => new(new ForbiddenException());

    /// <summary>
    ///     Creates a failure result with the specified error.
    /// </summary>
    /// <param name="error"> The error that occurred.</param>
    /// <param name="result">The optional HTTP result to return. If null, a default result will be generated.</param>
    /// <returns>A new instance of <see cref="HttpTnTResult{TSuccess}" /> representing the failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="error" /> is null.</exception>
    public static HttpTnTResult<TSuccess> Failure(Exception error, IResult? result = null) => new(error, result);

    /// <summary>
    ///     Creates a failure result with the specified error message.
    /// </summary>
    /// <param name="error"> The error message.</param>
    /// <param name="result">The optional HTTP result to return. If null, a default result will be generated.</param>
    /// <returns>A new instance of <see cref="HttpTnTResult{TSuccess}" /> representing the failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="error" /> is null or empty.</exception>
    public static HttpTnTResult<TSuccess> Failure(string error, IResult? result = null) => new(new Exception(error), result);

    /// <summary>
    ///     Creates a success result with the specified value.
    /// </summary>
    /// <param name="value"> The success value to store.</param>
    /// <param name="result">The optional HTTP result to return. If null, a default result will be generated based on the value.</param>
    /// <returns>A new instance of <see cref="HttpTnTResult{TSuccess}" /> representing the success.</returns>
    public static HttpTnTResult<TSuccess> Success(TSuccess value, IResult? result = null) => new(value, result);

    /// <summary>
    ///     Creates an HTTP redirect response with the specified value and URI.
    /// </summary>
    /// <param name="value">         The success value to store.</param>
    /// <param name="uri">           The URI to redirect to.</param>
    /// <param name="permanent">     Whether the redirect is permanent (301) or temporary (302).</param>
    /// <param name="preserveMethod">Whether to preserve the HTTP method in the redirect.</param>
    /// <returns>A new <see cref="HttpTnTResult{TSuccess}" /> representing the redirect with a success value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="uri" /> is null.</exception>
    public static HttpTnTResult<TSuccess> Redirect(TSuccess value, Uri uri, bool permanent = false, bool preserveMethod = false) => new(value, TypedResults.Redirect(uri.AbsoluteUri, permanent, preserveMethod));

#if NET9_0_OR_GREATER
    /// <summary>
    ///     Creates an HTTP 500 Internal Server Error response with an optional message.
    /// </summary>
    /// <param name="message">The optional error message to include in the response.</param>
    /// <returns>A new <see cref="HttpTnTResult{TSuccess}" /> representing an internal server error.</returns>
    public static HttpTnTResult<TSuccess> InternalServerError(string? message = null) => new(new Exception(message), TypedResults.InternalServerError(message));

    /// <summary>
    ///     Creates an HTTP 500 Internal Server Error response with problem details.
    /// </summary>
    /// <param name="problemDetails">The problem details to include in the response.</param>
    /// <returns>A new <see cref="HttpTnTResult{TSuccess}" /> representing an internal server error.</returns>
    public static HttpTnTResult<TSuccess> InternalServerError(ProblemDetails? problemDetails) => new(new Exception(problemDetails?.Title ?? "An error occurred"), TypedResults.InternalServerError(problemDetails));
#else

    /// <summary>
    ///     Creates an HTTP 500 Internal Server Error response with an optional message.
    /// </summary>
    /// <param name="message">The optional error message to include in the response.</param>
    /// <returns>A new <see cref="HttpTnTResult{TSuccess}" /> representing an internal server error.</returns>
    public static HttpTnTResult<TSuccess> InternalServerError(string? message = null) =>
        new(new Exception(message), TypedResults.StatusCode(StatusCodes.Status500InternalServerError));

#endif

    /// <summary>
    ///     Executes the specified action with the success value if the result represents a success.
    /// </summary>
    /// <param name="action">The action to execute with the success value.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="action" /> is null.</exception>
    public ITnTResult<TSuccess> OnSuccess(Action<TSuccess> action) {
        if (IsSuccessful) {
            action(Value);
        }
        return this;
    }

    /// <summary>
    ///     Executes the specified action if the result represents a failure.
    /// </summary>
    /// <param name="action">The action to execute with the error information.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="action" /> is null.</exception>
    public ITnTResult<TSuccess> OnFailure(Action<Exception> action) {
        if (!IsSuccessful) {
            action(Error);
        }
        return this;
    }

    /// <summary>
    ///     Executes the specified action if the result represents a success.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The current instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="action" /> is null.</exception>
    public ITnTResult<TSuccess> OnSuccess(Action action) {
        if (IsSuccessful) {
            action();
        }
        return this;
    }

    /// <summary>
    ///     Executes the specified action regardless of whether the result is successful or not.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public void Finally(Action action) => action();

    /// <summary>
    ///     Attempts to get the success value from the result.
    /// </summary>
    /// <param name="value">When this method returns, contains the success value if the result is successful; otherwise, the default value for the type.</param>
    /// <returns><c>true</c> if the result is successful and contains a value; otherwise, <c>false</c>.</returns>
    public bool TryGetValue(out TSuccess value) {
        if (IsSuccessful) {
            value = Value;
            return true;
        }
        value = default!; // Default value when unsuccessful (may be null for reference types)
        return false;
    }

    /// <summary>
    ///     Explicitly implements the non-generic interface methods to support polymorphic usage.
    /// </summary>
    ITnTResult ITnTResult.OnFailure(Action<Exception> action) => OnFailure(action);

    /// <summary>
    ///     Explicitly implements the non-generic interface methods to support polymorphic usage.
    /// </summary>
    ITnTResult ITnTResult.OnSuccess(Action action) => OnSuccess(action);

    /// <summary>
    ///     Executes the result asynchronously by delegating to the underlying IResult implementation.
    /// </summary>
    /// <param name="httpContext">The HTTP context for the current request.</param>
    /// <returns>A task that represents the asynchronous execution operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpContext" /> is null.</exception>
    public Task ExecuteAsync(HttpContext httpContext) => Result.ExecuteAsync(httpContext);
}