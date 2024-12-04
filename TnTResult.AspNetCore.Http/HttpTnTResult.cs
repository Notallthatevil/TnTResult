using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TnTResult.AspNetCore.Http.Ext;
using TnTResult.Ext;

namespace TnTResult.AspNetCore.Http;
internal struct HttpTnTResult : ITnTResult, IResult {

    /// <inheritdoc />
    public readonly Exception Error => _error.Value;

    /// <inheritdoc />
    public readonly string ErrorMessage => Error.Message;

    /// <inheritdoc />
    public readonly bool IsSuccessful => !_error.HasValue;

    private HttpStatusCode _successStatusCode = HttpStatusCode.OK;

    /// <summary>
    /// Gets a successful result.
    /// </summary>
    public static HttpTnTResult Successful => new() { _error = Optional<Exception>.NullOpt };
    public static HttpTnTResult Created => new() { _successStatusCode = HttpStatusCode.Created };

    private Optional<Exception> _error = new Exception().MakeOptional();

    internal HttpTnTResult(Exception error) {
        ArgumentNullException.ThrowIfNull(error, nameof(error));
        _error = error.MakeOptional();
    }

    public static HttpTnTResult Failure(Exception error) => new(error);
    public static HttpTnTResult Failure(string error) => new(new(error));

    public Task ExecuteAsync(HttpContext httpContext) => this.ToIResult(successStatusCode: _successStatusCode).ExecuteAsync(httpContext);
}

internal struct HttpTnTResult<TSuccess> : ITnTResult<TSuccess>, IResult {
    /// <inheritdoc />
    public readonly Exception Error => _expected.Error;

    /// <inheritdoc />
    public readonly string ErrorMessage => Error.Message;

    /// <inheritdoc />
    public readonly bool IsSuccessful => _expected.HasValue;

    /// <inheritdoc />
    public readonly TSuccess Value => _expected.Value;

    private HttpStatusCode _successStatusCode = HttpStatusCode.OK;

    private readonly Expected<TSuccess, Exception> _expected;

    internal HttpTnTResult(Exception error) {
        ArgumentNullException.ThrowIfNull(error, nameof(error));
        _expected = error.MakeUnexpected<TSuccess, Exception>();
    }

    internal HttpTnTResult(TSuccess? success) {
        _expected = success.MakeExpected<TSuccess, Exception>();
    }

    public static HttpTnTResult<TSuccess> Failure(Exception error) => new(error);
    public static HttpTnTResult<TSuccess> Failure(string error) => new(new Exception(error));
    public static HttpTnTResult<TSuccess> Success(TSuccess value) => new(value);
    public static HttpTnTResult<TSuccess> Created(TSuccess value) => new(value) { _successStatusCode = HttpStatusCode.Created };

    public Task ExecuteAsync(HttpContext httpContext) => this.ToIResult(successStatusCode: _successStatusCode).ExecuteAsync(httpContext);
}
