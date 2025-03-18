using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TnTResult.AspNetCore.Http.Ext;
using TnTResult.Ext;

namespace TnTResult.AspNetCore.Http;

/// <summary>
///     Represents an HTTP result for TnT operations.
/// </summary>
internal class HttpTnTResult : ITnTResult, IResult {

    /// <summary>
    ///     Gets a successful result.
    /// </summary>
    public static HttpTnTResult Successful {
        get {
            var result = new HttpTnTResult();
            result.Result = result.ToIResult();
            return result;
        }
    }

    /// <inheritdoc />
    public virtual Exception Error => _error.Value;

    /// <inheritdoc />
    public virtual string ErrorMessage => Error.Message;

    /// <inheritdoc />
    public virtual bool IsSuccessful => !_error.HasValue;

    public bool HasFailed => !IsSuccessful;

    private readonly Optional<Exception> _error;
    internal protected IResult Result = default!;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpTnTResult" /> class with the specified error.
    /// </summary>
    /// <param name="error">The error that occurred.</param>
    internal HttpTnTResult(Exception error, IResult? result = null) {
        ArgumentNullException.ThrowIfNull(error, nameof(error));
        _error = Optional.MakeOptional(error);
        Result = result ?? this.ToIResult();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpTnTResult" /> class.
    /// </summary>
    internal HttpTnTResult() {
        _error = Optional<Exception>.NullOpt;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpTnTResult" /> class with the specified result.
    /// </summary>
    /// <param name="result">The result to wrap.</param>
    protected HttpTnTResult(IResult result) {
        ArgumentNullException.ThrowIfNull(result, nameof(result));
        Result = result;
        _error = Optional<Exception>.NullOpt;
    }

    public static HttpTnTResult Accepted() => new(TypedResults.Accepted((string?)null));

    public static HttpTnTResult Created<TValue>(TValue? value) => new(TypedResults.Created((string?)null, value));

    public static HttpTnTResult Created() => new(TypedResults.Created());

    public static HttpTnTResult CustomError(Exception exception, IResult result) => new(exception, result);

    public static HttpTnTResult CustomResult(IResult result) => new(result);
    
    public static HttpTnTResult Redirect(Uri uri, bool permanent, bool preserveMethod) => new(TypedResults.Redirect(uri.AbsoluteUri, permanent, preserveMethod));

    /// <summary>
    ///     Creates a failure result with the specified error.
    /// </summary>
    /// <param name="error">The error that occurred.</param>
    /// <returns>A new instance of <see cref="HttpTnTResult" /> representing the failure.</returns>
    public static HttpTnTResult Failure(Exception error, IResult? result = null) => new(error, result);

    /// <summary>
    ///     Creates a failure result with the specified error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A new instance of <see cref="HttpTnTResult" /> representing the failure.</returns>
    public static HttpTnTResult Failure(string error, IResult? result = null) => new(new Exception(error), result);

    /// <summary>
    ///     Executes the result asynchronously.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual Task ExecuteAsync(HttpContext httpContext) => Result.ExecuteAsync(httpContext);
    public ITnTResult OnFailure(Action<Exception> action) {
        if (!IsSuccessful) {
            action(Error);
        }
        return this;
    }
    public ITnTResult OnSuccess(Action action) {
        if (IsSuccessful) {
            action();
        }
        return this;
    }
}

/// <summary>
///     Represents an HTTP result for TnT operations with a specific success value.
/// </summary>
/// <typeparam name="TSuccess">The type of the success value.</typeparam>
internal class HttpTnTResult<TSuccess> : HttpTnTResult, ITnTResult<TSuccess> {
    public override Exception Error => _expected.Error;

    public override string ErrorMessage => Error.Message;

    public override bool IsSuccessful => _expected.HasValue;

    /// <inheritdoc />
    public TSuccess Value => _expected.Value;

    private readonly Expected<TSuccess, Exception> _expected;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpTnTResult" /> class with the specified error.
    /// </summary>
    /// <param name="error">The error that occurred.</param>
    internal HttpTnTResult(Exception error, IResult? result = null) : base(error, result) {
        ArgumentNullException.ThrowIfNull(error, nameof(error));
        _expected = Expected.MakeUnexpected<TSuccess, Exception>(error);
        Result = result ?? this.ToIResult();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="HttpTnTResult{TSuccess}" /> class with the
    ///     specified success value and result.
    /// </summary>
    /// <param name="success">The success value.</param>
    /// <param name="result"> The result to wrap.</param>
    private HttpTnTResult(TSuccess success, IResult? result = null) {
        _expected = Expected.MakeExpected<TSuccess, Exception>(success);

        var fileDownloadResult = this as ITnTResult<TnTFileDownload>;

        Result = result ?? fileDownloadResult?.ToIResult() ?? this.ToIResult();
    }

    /// <summary>
    ///     Creates a result indicating that a resource was created with the specified value.
    /// </summary>
    /// <param name="value">The success value.</param>
    /// <returns>
    ///     A new instance of <see cref="HttpTnTResult{TSuccess}" /> representing the creation.
    /// </returns>
    public static HttpTnTResult<TSuccess> Created(TSuccess value) => new(value, TypedResults.Created((string?)null, value));

    public new static HttpTnTResult<TSuccess> CustomError(Exception exception, IResult result) => new(exception, result);

    public static HttpTnTResult<TSuccess> CustomResult(TSuccess success, IResult result) => new(success, result);

    /// <summary>
    ///     Creates a failure result with the specified error.
    /// </summary>
    /// <param name="error">The error that occurred.</param>
    /// <returns>
    ///     A new instance of <see cref="HttpTnTResult{TSuccess}" /> representing the failure.
    /// </returns>
    public static new HttpTnTResult<TSuccess> Failure(Exception error, IResult? result = null) => new(error, result);

    public static HttpTnTResult<TSuccess> Redirect(TSuccess value, Uri uri, bool permanent, bool preserveMethod) => new(value, TypedResults.Redirect(uri.AbsoluteUri, permanent, preserveMethod));

    /// <summary>
    ///     Creates a failure result with the specified error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>
    ///     A new instance of <see cref="HttpTnTResult{TSuccess}" /> representing the failure.
    /// </returns>
    public static new HttpTnTResult<TSuccess> Failure(string error, IResult? result = null) => new(new Exception(error), result);

    /// <summary>
    ///     Creates a success result with the specified value.
    /// </summary>
    /// <param name="value">The success value.</param>
    /// <returns>
    ///     A new instance of <see cref="HttpTnTResult{TSuccess}" /> representing the success.
    /// </returns>
    public static HttpTnTResult<TSuccess> Success(TSuccess value, IResult? result = null) => new(value, result);

    public ITnTResult<TSuccess> OnSuccess(Action<TSuccess> action) {
        if (IsSuccessful) {
            action(Value);
        }
        return this;
    }
    public new ITnTResult<TSuccess> OnFailure(Action<Exception> action) {
        if (!IsSuccessful) {
            action(Error);
        }
        return this;
    }
    public new ITnTResult<TSuccess> OnSuccess(Action action) {
        if (IsSuccessful) {
            action();
        }
        return this;
    }

    ITnTResult ITnTResult.OnFailure(Action<Exception> action) => OnFailure(action);
    ITnTResult ITnTResult.OnSuccess(Action action) => OnSuccess(action);
}