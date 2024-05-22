using TnTResult.Ext;

namespace TnTResult.AspNetCore.Http;
public class CreatedTnTResult : ITnTResult {
    /// <inheritdoc />
    public Exception Error => _error.Value;

    /// <inheritdoc />
    public string ErrorMessage => Error.Message;

    /// <inheritdoc />
    public bool IsSuccessful => !_error.HasValue;

    public static CreatedTnTResult Successful => new();

    private readonly Optional<Exception> _error = new Exception().MakeOptional();

    protected CreatedTnTResult() { _error = Optional<Exception>.NullOpt; }

    internal CreatedTnTResult(Exception error) {
        ArgumentNullException.ThrowIfNull(error, nameof(error));
        _error = error.MakeOptional();
    }
}

public class CreatedTnTResult<TSuccess> : CreatedTnTResult, ITnTResult<TSuccess> {
    /// <inheritdoc />
    public TSuccess? Value => _expected.Value;

    private readonly Expected<TSuccess, Exception> _expected;

    internal CreatedTnTResult(Exception error) : base(error) { }

    internal CreatedTnTResult(TSuccess? success) : base() {
        _expected = success.MakeExpected<TSuccess, Exception>();
    }
}
