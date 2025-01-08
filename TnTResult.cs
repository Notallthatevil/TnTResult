using TnTResult.Ext;

namespace TnTResult;

public static class TnTResult {
    public static ITnTResult Successful => new InternalTnTResult();
    public static ITnTResult<TSuccess> Success<TSuccess>(TSuccess value) => new InternalTnTResult<TSuccess>(value);
    public static ITnTResult<TSuccess> Failure<TSuccess>(Exception error) => new InternalTnTResult<TSuccess>(error);
    public static ITnTResult Failure(Exception error) => new InternalTnTResult(error);
}

internal readonly struct InternalTnTResult : ITnTResult {
    public readonly Exception Error => _error.Value;
    public readonly string ErrorMessage => Error.Message;
    public readonly bool IsSuccessful => !_error.HasValue;
    public bool HasFailed => !IsSuccessful;

    internal readonly Optional<Exception> _error;
    internal InternalTnTResult(Exception? error) => _error = error!;

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

internal readonly struct InternalTnTResult<TSuccess> : ITnTResult<TSuccess> {
    public TSuccess Value => _result.Value;
    public Exception Error => _result.Error;
    public string ErrorMessage => Error.Message;
    public bool IsSuccessful => _result.HasValue;

    public bool HasFailed => !IsSuccessful;

    public readonly Expected<TSuccess, Exception> _result;

    internal InternalTnTResult(TSuccess value) => _result = value;
    internal InternalTnTResult(Exception error) => _result = error;

    public ITnTResult<TSuccess> OnSuccess(Action<TSuccess> action) {
        if (IsSuccessful) {
            action(Value);
        }
        return this;
    }
    public ITnTResult<TSuccess> OnFailure(Action<Exception> action) {
        if (!IsSuccessful) {
            action(Error);
        }
        return this;
    }
    public ITnTResult<TSuccess> OnSuccess(Action action) {
        if (IsSuccessful) {
            action();
        }
        return this;
    }

    ITnTResult ITnTResult.OnFailure(Action<Exception> action) => OnFailure(action);
    ITnTResult ITnTResult.OnSuccess(Action action) => OnSuccess(action);
}