﻿using TnTResult.Ext;

namespace TnTResult;

/// <summary>
/// Represents the result of an operation with no return value.
/// </summary>
internal struct TnTResult : ITnTResult {

    /// <inheritdoc />
    public readonly Exception Error => _error.Value;

    /// <inheritdoc />
    public readonly string ErrorMessage => Error.Message;

    /// <inheritdoc />
    public readonly bool IsSuccessful => !_error.HasValue;

    /// <summary>
    /// Gets a successful result.
    /// </summary>
    internal static TnTResult Successful => new() { _error = Optional<Exception>.NullOpt };

    private Optional<Exception> _error = new Exception().MakeOptional();

    internal TnTResult(Exception error) {
        ArgumentNullException.ThrowIfNull(error, nameof(error));
        _error = error.MakeOptional();
    }

    /// <inheritdoc />
    public readonly ITnTResult OnFailure(Action<Exception> action) => OnFailureAsync((exc) => {
        action(exc);
        return Task.CompletedTask;
    }).Result;

    /// <inheritdoc />
    public readonly async Task<ITnTResult> OnFailureAsync(Func<Exception, Task> func) {
        if (!IsSuccessful) {
            await func(_error.Value);
        }
        return this;
    }

    /// <inheritdoc />
    public readonly ITnTResult OnSuccess(Action action) {
        return OnSuccessAsync(() => {
            action();
            return Task.CompletedTask;
        }).Result;
    }

    /// <inheritdoc />
    public readonly async Task<ITnTResult> OnSuccessAsync(Func<Task> func) {
        if (IsSuccessful) {
            await func();
        }
        return this;
    }
}

/// <summary>
/// Represents the result of an operation with a return value.
/// </summary>
/// <typeparam name="TSuccess">The type of the success value.</typeparam>
internal readonly struct TnTResult<TSuccess> : ITnTResult<TSuccess> {

    /// <inheritdoc />
    public readonly Exception Error => _expected.Error;

    /// <inheritdoc />
    public readonly string ErrorMessage => Error.Message;

    /// <inheritdoc />
    public readonly bool IsSuccessful => _expected.HasValue;

    /// <inheritdoc />
    public readonly TSuccess? Value => _expected.Value;

    private readonly Expected<TSuccess, Exception> _expected;

    internal TnTResult(Exception error) {
        ArgumentNullException.ThrowIfNull(error, nameof(error));
        _expected = error.MakeUnexpected<TSuccess, Exception>();
    }

    internal TnTResult(TSuccess? success) {
        _expected = success.MakeExpected<TSuccess, Exception>();
    }

    /// <inheritdoc />
    public ITnTResult OnFailure(Action<Exception> action) => OnFailureAsync((exc) => {
        action(exc);
        return Task.CompletedTask;
    }).Result;

    /// <inheritdoc />
    public async Task<ITnTResult> OnFailureAsync(Func<Exception, Task> func) {
        if (!IsSuccessful) {
            await func(Error);
        }
        return this;
    }

    /// <inheritdoc />
    public readonly ITnTResult<TSuccess> OnSuccess(Action<TSuccess?> action) => OnSuccessAsync((value) => {
        action(value);
        return Task.CompletedTask;
    }).Result;

    /// <inheritdoc />
    public ITnTResult OnSuccess(Action action) => OnSuccess(_ => action());

    /// <inheritdoc />
    public readonly async Task<ITnTResult<TSuccess>> OnSuccessAsync(Func<TSuccess?, Task> func) {
        if (IsSuccessful) {
            await func(Value);
        }
        return this;
    }

    /// <inheritdoc />
    public async Task<ITnTResult> OnSuccessAsync(Func<Task> func) => await OnSuccessAsync(_ => func());
}