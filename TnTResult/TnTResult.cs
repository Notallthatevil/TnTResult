using TnTResult.Ext;

namespace TnTResult;

/// <summary>
///     Provides static factory methods for creating <see cref="ITnTResult" /> and <see cref="ITnTResult{TSuccess}" /> instances.
/// </summary>
public static class TnTResult {

    /// <inheritdoc cref="ITnTResult.IsSuccessful" />
    public static ITnTResult Successful => new InternalTnTResult();

    /// <inheritdoc />
    public static ITnTResult<TSuccess> Failure<TSuccess>(Exception error) => new InternalTnTResult<TSuccess>(error);

    /// <inheritdoc  />
    public static ITnTResult Failure(Exception error) => new InternalTnTResult(error);

    /// <inheritdoc />
    public static ITnTResult<TSuccess> Success<TSuccess>(TSuccess value) => new InternalTnTResult<TSuccess>(value);
}

/// <summary>
///     Internal implementation of <see cref="ITnTResult" />.
/// </summary>
internal readonly struct InternalTnTResult : ITnTResult {

    /// <inheritdoc />
    public readonly Exception Error => _error.Value;

    /// <inheritdoc />
    public readonly string ErrorMessage => Error.Message;

    /// <inheritdoc />
    public bool HasFailed => !IsSuccessful;

    /// <inheritdoc />
    public readonly bool IsSuccessful => !_error.HasValue;

    internal readonly Optional<Exception> _error;

    internal InternalTnTResult(Exception error) => _error = Optional.MakeOptional(error);

    /// <inheritdoc />
    public void Finally(Action action) {
        action();
    }

    /// <inheritdoc />
    public ITnTResult OnFailure(Action<Exception> action) {
        if (!IsSuccessful) {
            action(Error);
        }
        return this;
    }

    /// <inheritdoc />
    public ITnTResult OnSuccess(Action action) {
        if (IsSuccessful) {
            action();
        }
        return this;
    }
}

/// <summary>
///     Internal implementation of <see cref="ITnTResult{TSuccess}" />.
/// </summary>
internal readonly struct InternalTnTResult<TSuccess> : ITnTResult<TSuccess> {

    /// <inheritdoc />
    public Exception Error => _result.Error;

    /// <inheritdoc />
    public string ErrorMessage => Error.Message;

    /// <inheritdoc />
    public bool HasFailed => !IsSuccessful;

    /// <inheritdoc />
    public bool IsSuccessful => _result.HasValue;

    /// <inheritdoc />
    public TSuccess Value => _result.Value;

    public readonly Expected<TSuccess, Exception> _result;

    internal InternalTnTResult(TSuccess value) => _result = Expected.MakeExpected<TSuccess, Exception>(value);

    internal InternalTnTResult(Exception error) => _result = Expected.MakeUnexpected<TSuccess, Exception>(error);

    /// <inheritdoc />
    public void Finally(Action action) {
        action();
    }

    /// <inheritdoc />
    public ITnTResult<TSuccess> OnFailure(Action<Exception> action) {
        if (!IsSuccessful) {
            action(Error);
        }
        return this;
    }

    /// <inheritdoc />
    public ITnTResult<TSuccess> OnSuccess(Action<TSuccess> action) {
        if (IsSuccessful) {
            action(Value);
        }
        return this;
    }

    /// <inheritdoc />
    public ITnTResult<TSuccess> OnSuccess(Action action) {
        if (IsSuccessful) {
            action();
        }
        return this;
    }

    /// <inheritdoc />
    public bool TryGetValue(out TSuccess value) {
        if (IsSuccessful) {
            value = Value;
            return true;
        }
        value = default!;
        return false;
    }

    /// <inheritdoc />
    void ITnTResult.Finally(Action action) => Finally(action);

    /// <inheritdoc />
    ITnTResult ITnTResult.OnFailure(Action<Exception> action) => OnFailure(action);

    /// <inheritdoc />
    ITnTResult ITnTResult.OnSuccess(Action action) => OnSuccess(action);
}