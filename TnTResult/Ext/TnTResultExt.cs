namespace TnTResult.Ext;

/// <summary>
///     Provides extension methods for handling success and failure of <see cref="ITnTResult" /> and <see cref="ITnTResult{TSuccess}" /> asynchronously.
/// </summary>
public static class TnTResultExt {

    /// <summary>
    ///     Executes the specified action regardless of the result (finally pattern). Runs even if the task faults.
    /// </summary>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="action">The action to execute after the result completes.</param>
    public static async Task FinallyAsync(this Task<ITnTResult> result, Action action) {
        ArgumentNullException.ThrowIfNull(action);
        try {
            await result.ConfigureAwait(false);
        }
        finally {
            action();
        }
    }

    /// <summary>
    ///     Executes the specified action regardless of the result (finally pattern). Runs even if the task faults.
    /// </summary>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="action">The action to execute after the result completes.</param>
    public static async ValueTask FinallyAsync(this ValueTask<ITnTResult> result, Action action) {
        ArgumentNullException.ThrowIfNull(action);
        try {
            await result.ConfigureAwait(false);
        }
        finally {
            action();
        }
    }

    /// <summary>
    ///     Executes the specified action regardless of the result (finally pattern). Runs even if the task faults.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="action">The action to execute after the result completes.</param>
    public static async Task FinallyAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Action action) {
        ArgumentNullException.ThrowIfNull(action);
        try {
            await result.ConfigureAwait(false);
        }
        finally {
            action();
        }
    }

    /// <summary>
    ///     Executes the specified action regardless of the result (finally pattern). Runs even if the task faults.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="action">The action to execute after the result completes.</param>
    public static async ValueTask FinallyAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Action action) {
        ArgumentNullException.ThrowIfNull(action);
        try {
            await result.ConfigureAwait(false);
        }
        finally {
            action();
        }
    }

    /// <summary>
    ///     Executes the specified asynchronous function regardless of the result (finally pattern). Runs even if the task faults.
    /// </summary>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="func">  The asynchronous function to execute after the result completes.</param>
    public static async Task FinallyAsync(this Task<ITnTResult> result, Func<Task> func) {
        ArgumentNullException.ThrowIfNull(func);
        try {
            await result.ConfigureAwait(false);
        }
        finally {
            await func().ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Executes the specified asynchronous function regardless of the result (finally pattern). Runs even if the task faults.
    /// </summary>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="func">  The asynchronous function to execute after the result completes.</param>
    public static async ValueTask FinallyAsync(this ValueTask<ITnTResult> result, Func<Task> func) {
        ArgumentNullException.ThrowIfNull(func);
        try {
            await result.ConfigureAwait(false);
        }
        finally {
            await func().ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Executes the specified asynchronous function regardless of the result (finally pattern). Runs even if the task faults.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="func">  The asynchronous function to execute after the result completes.</param>
    public static async Task FinallyAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<Task> func) {
        ArgumentNullException.ThrowIfNull(func);
        try {
            await result.ConfigureAwait(false);
        }
        finally {
            await func().ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Executes the specified asynchronous function regardless of the result (finally pattern). Runs even if the task faults.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="func">  The asynchronous function to execute after the result completes.</param>
    public static async ValueTask FinallyAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<Task> func) {
        ArgumentNullException.ThrowIfNull(func);
        try {
            await result.ConfigureAwait(false);
        }
        finally {
            await func().ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Executes the specified asynchronous function regardless of the result (finally pattern). Runs even if the task faults. Supports cancellation.
    /// </summary>
    /// <param name="result">           The asynchronous result to evaluate.</param>
    /// <param name="action">           The action to execute after the result completes.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public static async Task FinallyAsync(this Task<ITnTResult> result, Action action, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(action);
        try {
            await result.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        finally {
            action();
        }
    }

    /// <summary>
    ///     Executes the specified action regardless of the result (finally pattern). Runs even if the task faults. Supports cancellation.
    /// </summary>
    /// <param name="result">           The asynchronous result to evaluate.</param>
    /// <param name="action">           The action to execute after the result completes.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public static async ValueTask FinallyAsync(this ValueTask<ITnTResult> result, Action action, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(action);
        try {
            await result.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        finally {
            action();
        }
    }

    /// <summary>
    ///     Executes the specified action regardless of the result (finally pattern). Runs even if the task faults. Supports cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">           The asynchronous result to evaluate.</param>
    /// <param name="action">           The action to execute after the result completes.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public static async Task FinallyAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Action action, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(action);
        try {
            await result.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        finally {
            action();
        }
    }

    /// <summary>
    ///     Executes the specified action regardless of the result (finally pattern). Runs even if the task faults. Supports cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">           The asynchronous result to evaluate.</param>
    /// <param name="action">           The action to execute after the result completes.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public static async ValueTask FinallyAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Action action, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(action);
        try {
            await result.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        finally {
            action();
        }
    }

    /// <summary>
    ///     Executes the specified asynchronous function regardless of the result (finally pattern). Runs even if the task faults. Supports cancellation.
    /// </summary>
    /// <param name="result">           The asynchronous result to evaluate.</param>
    /// <param name="func">             The asynchronous function to execute after the result completes.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public static async Task FinallyAsync(this Task<ITnTResult> result, Func<Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        try {
            await result.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        finally {
            await func().ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Executes the specified asynchronous function regardless of the result (finally pattern). Runs even if the task faults. Supports cancellation.
    /// </summary>
    /// <param name="result">           The asynchronous result to evaluate.</param>
    /// <param name="func">             The asynchronous function to execute after the result completes.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public static async ValueTask FinallyAsync(this ValueTask<ITnTResult> result, Func<Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        try {
            await result.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        finally {
            await func().ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Executes the specified asynchronous function regardless of the result (finally pattern). Runs even if the task faults. Supports cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">           The asynchronous result to evaluate.</param>
    /// <param name="func">             The asynchronous function to execute after the result completes.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public static async Task FinallyAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        try {
            await result.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        finally {
            await func().ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Executes the specified asynchronous function regardless of the result (finally pattern). Runs even if the task faults. Supports cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">           The asynchronous result to evaluate.</param>
    /// <param name="func">             The asynchronous function to execute after the result completes.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public static async ValueTask FinallyAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        try {
            await result.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        finally {
            await func().ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Executes the specified asynchronous function with cancellation support regardless of the result (finally pattern). Runs even if the task faults.
    /// </summary>
    /// <param name="result">           The asynchronous result to evaluate.</param>
    /// <param name="func">             The asynchronous function to execute after the result completes, accepting a <see cref="CancellationToken" />.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public static async Task FinallyAsync(this Task<ITnTResult> result, Func<CancellationToken, Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        try {
            await result.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        finally {
            await func(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Executes the specified asynchronous function with cancellation support regardless of the result (finally pattern). Runs even if the task faults.
    /// </summary>
    /// <param name="result">           The asynchronous result to evaluate.</param>
    /// <param name="func">             The asynchronous function to execute after the result completes, accepting a <see cref="CancellationToken" />.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    public static async ValueTask FinallyAsync(this ValueTask<ITnTResult> result, Func<CancellationToken, Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        try {
            await result.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        finally {
            await func(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, returns the default value for <typeparamref name="TSuccess" />.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <returns>The success value when successful; otherwise, the default value for <typeparamref name="TSuccess" />.</returns>
    public static async ValueTask<TSuccess> GetValueOrDefaultAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result) {
        var r = await result.ConfigureAwait(false);
        return r.GetValueOrDefault();
    }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, returns the default value for <typeparamref name="TSuccess" />.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <returns>The success value when successful; otherwise, the default value for <typeparamref name="TSuccess" />.</returns>
    public static async Task<TSuccess> GetValueOrDefaultAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result) {
        var r = await result.ConfigureAwait(false);
        return r.GetValueOrDefault();
    }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, returns the default value for <typeparamref name="TSuccess" />. Supports cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">           The asynchronous result to evaluate.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The success value when successful; otherwise, the default value for <typeparamref name="TSuccess" />.</returns>
    public static async ValueTask<TSuccess> GetValueOrDefaultAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, CancellationToken cancellationToken) {
        var r = await result.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        return r.GetValueOrDefault();
    }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, returns the default value for <typeparamref name="TSuccess" />. Supports cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">           The asynchronous result to evaluate.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The success value when successful; otherwise, the default value for <typeparamref name="TSuccess" />.</returns>
    public static async Task<TSuccess> GetValueOrDefaultAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, CancellationToken cancellationToken) {
        var r = await result.WaitAsync(cancellationToken).ConfigureAwait(false);
        return r.GetValueOrDefault();
    }

    /// <summary>
    ///     Executes the specified action if the result has failed.
    /// </summary>
    public static Task<ITnTResult> OnFailureAsync(this Task<ITnTResult> result, Action<Exception> action) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(r => r.OnFailure(action));
    }

    /// <summary>
    ///     Executes the specified action if the result has failed.
    /// </summary>
    public static ValueTask<ITnTResult> OnFailureAsync(this ValueTask<ITnTResult> result, Action<Exception> action) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(r => r.OnFailure(action));
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result has failed.
    /// </summary>
    public static Task<ITnTResult> OnFailureAsync(this Task<ITnTResult> result, Func<Exception, Task> func) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(async r => {
            if (r.HasFailed) {
                await func(r.Error).ConfigureAwait(false);
            }
        });
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result has failed.
    /// </summary>
    public static ValueTask<ITnTResult> OnFailureAsync(this ValueTask<ITnTResult> result, Func<Exception, Task> func) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(async r => {
            if (r.HasFailed) {
                await func(r.Error).ConfigureAwait(false);
            }
        });
    }

    /// <summary>
    ///     Executes the specified action if the result has failed.
    /// </summary>
    public static Task<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Action<Exception> action) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(r => r.OnFailure(action));
    }

    /// <summary>
    ///     Executes the specified action if the result has failed.
    /// </summary>
    public static ValueTask<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Action<Exception> action) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(r => r.OnFailure(action));
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result has failed.
    /// </summary>
    public static Task<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<Exception, Task> func) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(async r => {
            if (r.HasFailed) {
                await func(r.Error).ConfigureAwait(false);
            }
        });
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result has failed.
    /// </summary>
    public static ValueTask<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<Exception, Task> func) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(async r => {
            if (r.HasFailed) {
                await func(r.Error).ConfigureAwait(false);
            }
        });
    }

    public static Task<ITnTResult> OnFailureAsync(this Task<ITnTResult> result, Action<Exception> action, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(cancellationToken, r => {
            cancellationToken.ThrowIfCancellationRequested();
            return r.OnFailure(action);
        });
    }

    public static ValueTask<ITnTResult> OnFailureAsync(this ValueTask<ITnTResult> result, Action<Exception> action, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(cancellationToken, r => {
            cancellationToken.ThrowIfCancellationRequested();
            return r.OnFailure(action);
        });
    }

    public static Task<ITnTResult> OnFailureAsync(this Task<ITnTResult> result, Func<Exception, Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(cancellationToken, async (r, ct) => {
            if (r.HasFailed) {
                ct.ThrowIfCancellationRequested();
                await func(r.Error).ConfigureAwait(false);
            }
        });
    }

    public static ValueTask<ITnTResult> OnFailureAsync(this ValueTask<ITnTResult> result, Func<Exception, Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(cancellationToken, async (r, ct) => {
            if (r.HasFailed) {
                ct.ThrowIfCancellationRequested();
                await func(r.Error).ConfigureAwait(false);
            }
        });
    }

    public static Task<ITnTResult> OnFailureAsync(this Task<ITnTResult> result, Func<Exception, CancellationToken, Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(cancellationToken, async (r, ct) => {
            if (r.HasFailed) {
                await func(r.Error, ct).ConfigureAwait(false);
            }
        });
    }

    public static ValueTask<ITnTResult> OnFailureAsync(this ValueTask<ITnTResult> result, Func<Exception, CancellationToken, Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(cancellationToken, async (r, ct) => {
            if (r.HasFailed) {
                await func(r.Error, ct).ConfigureAwait(false);
            }
        });
    }

    public static Task<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Action<Exception> action, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(cancellationToken, r => {
            cancellationToken.ThrowIfCancellationRequested();
            return r.OnFailure(action);
        });
    }

    public static ValueTask<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Action<Exception> action, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(cancellationToken, r => {
            cancellationToken.ThrowIfCancellationRequested();
            return r.OnFailure(action);
        });
    }

    public static Task<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<Exception, Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(cancellationToken, async (r, ct) => {
            if (r.HasFailed) {
                ct.ThrowIfCancellationRequested();
                await func(r.Error).ConfigureAwait(false);
            }
        });
    }

    public static ValueTask<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<Exception, Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(cancellationToken, async (r, ct) => {
            if (r.HasFailed) {
                ct.ThrowIfCancellationRequested();
                await func(r.Error).ConfigureAwait(false);
            }
        });
    }

    public static Task<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<Exception, CancellationToken, Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(cancellationToken, async (r, ct) => {
            if (r.HasFailed) {
                await func(r.Error, ct).ConfigureAwait(false);
            }
        });
    }

    public static ValueTask<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<Exception, CancellationToken, Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(cancellationToken, async (r, ct) => {
            if (r.HasFailed) {
                await func(r.Error, ct).ConfigureAwait(false);
            }
        });
    }

    /// <summary>
    ///     Executes the specified action if the result is successful.
    /// </summary>
    public static Task<ITnTResult> OnSuccessAsync(this Task<ITnTResult> result, Action action) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(r => r.OnSuccess(action));
    }

    /// <summary>
    ///     Executes the specified action if the result is successful.
    /// </summary>
    public static ValueTask<ITnTResult> OnSuccessAsync(this ValueTask<ITnTResult> result, Action action) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(r => r.OnSuccess(action));
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result is successful.
    /// </summary>
    public static Task<ITnTResult> OnSuccessAsync(this Task<ITnTResult> result, Func<Task> func) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(async r => {
            if (r.IsSuccessful) {
                await func().ConfigureAwait(false);
            }
        });
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result is successful.
    /// </summary>
    public static ValueTask<ITnTResult> OnSuccessAsync(this ValueTask<ITnTResult> result, Func<Task> func) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(async r => {
            if (r.IsSuccessful) {
                await func().ConfigureAwait(false);
            }
        });
    }

    /// <summary>
    ///     Executes the specified action if the result is successful.
    /// </summary>
    public static Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Action action) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(r => r.OnSuccess(action));
    }

    /// <summary>
    ///     Executes the specified action if the result is successful.
    /// </summary>
    public static ValueTask<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Action action) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(r => r.OnSuccess(action));
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result is successful.
    /// </summary>
    public static Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<Task> func) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(async r => {
            if (r.IsSuccessful) {
                await func().ConfigureAwait(false);
            }
        });
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result is successful.
    /// </summary>
    public static ValueTask<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<Task> func) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(async r => {
            if (r.IsSuccessful) {
                await func().ConfigureAwait(false);
            }
        });
    }

    /// <summary>
    ///     Executes the specified action with the success value if the result is successful.
    /// </summary>
    public static Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Action<TSuccess> action) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(r => r.OnSuccess(action));
    }

    /// <summary>
    ///     Executes the specified action with the success value if the result is successful.
    /// </summary>
    public static ValueTask<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Action<TSuccess> action) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(r => r.OnSuccess(action));
    }

    /// <summary>
    ///     Executes the specified asynchronous function with the success value if the result is successful.
    /// </summary>
    public static Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<TSuccess, Task> func) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(async r => {
            if (r.IsSuccessful) {
                await func(r.Value).ConfigureAwait(false);
            }
        });
    }

    /// <summary>
    ///     Executes the specified asynchronous function with the success value if the result is successful.
    /// </summary>
    public static ValueTask<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<TSuccess, Task> func) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(async r => {
            if (r.IsSuccessful) {
                await func(r.Value).ConfigureAwait(false);
            }
        });
    }

    public static Task<ITnTResult> OnSuccessAsync(this Task<ITnTResult> result, Action action, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(cancellationToken, r => {
            cancellationToken.ThrowIfCancellationRequested();
            return r.OnSuccess(action);
        });
    }

    public static ValueTask<ITnTResult> OnSuccessAsync(this ValueTask<ITnTResult> result, Action action, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(cancellationToken, r => {
            cancellationToken.ThrowIfCancellationRequested();
            return r.OnSuccess(action);
        });
    }

    public static Task<ITnTResult> OnSuccessAsync(this Task<ITnTResult> result, Func<Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(cancellationToken, async (r, ct) => {
            if (r.IsSuccessful) {
                ct.ThrowIfCancellationRequested();
                await func().ConfigureAwait(false);
            }
        });
    }

    public static ValueTask<ITnTResult> OnSuccessAsync(this ValueTask<ITnTResult> result, Func<Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(cancellationToken, async (r, ct) => {
            if (r.IsSuccessful) {
                ct.ThrowIfCancellationRequested();
                await func().ConfigureAwait(false);
            }
        });
    }

    public static Task<ITnTResult> OnSuccessAsync(this Task<ITnTResult> result, Func<CancellationToken, Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(cancellationToken, async (r, ct) => {
            if (r.IsSuccessful) {
                await func(ct).ConfigureAwait(false);
            }
        });
    }

    public static ValueTask<ITnTResult> OnSuccessAsync(this ValueTask<ITnTResult> result, Func<CancellationToken, Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(cancellationToken, async (r, ct) => {
            if (r.IsSuccessful) {
                await func(ct).ConfigureAwait(false);
            }
        });
    }

    public static Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Action action, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(cancellationToken, r => {
            cancellationToken.ThrowIfCancellationRequested();
            return r.OnSuccess(action);
        });
    }

    public static ValueTask<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Action action, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(action);
        return result.Then(cancellationToken, r => {
            cancellationToken.ThrowIfCancellationRequested();
            return r.OnSuccess(action);
        });
    }

    public static Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(cancellationToken, async (r, ct) => {
            if (r.IsSuccessful) {
                ct.ThrowIfCancellationRequested();
                await func().ConfigureAwait(false);
            }
        });
    }

    public static ValueTask<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(cancellationToken, async (r, ct) => {
            if (r.IsSuccessful) {
                ct.ThrowIfCancellationRequested();
                await func().ConfigureAwait(false);
            }
        });
    }

    public static Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<CancellationToken, Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(cancellationToken, async (r, ct) => {
            if (r.IsSuccessful) {
                await func(ct).ConfigureAwait(false);
            }
        });
    }

    public static ValueTask<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<CancellationToken, Task> func, CancellationToken cancellationToken) {
        ArgumentNullException.ThrowIfNull(func);
        return result.ThenAwait(cancellationToken, async (r, ct) => {
            if (r.IsSuccessful) {
                await func(ct).ConfigureAwait(false);
            }
        });
    }

    public static async ValueTask<ITnTResult> ThrowOnFailureAsync(this ValueTask<ITnTResult> result) {
        var r = await result.ConfigureAwait(false);
        return r.ThrowOnFailure();
    }

    public static async ValueTask<ITnTResult> ThrowOnFailureAsync(this ValueTask<ITnTResult> result, Func<Exception> exceptionCreationFunc) {
        var r = await result.ConfigureAwait(false);
        return r.ThrowOnFailure(exceptionCreationFunc);
    }

    public static async ValueTask<ITnTResult<TSuccess>> ThrowOnFailureAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result) {
        var r = await result.ConfigureAwait(false);
        return r.ThrowOnFailure();
    }

    public static async ValueTask<ITnTResult<TSuccess>> ThrowOnFailureAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<Exception> exceptionCreationFunc) {
        var r = await result.ConfigureAwait(false);
        return r.ThrowOnFailure(exceptionCreationFunc);
    }

    public static async Task<ITnTResult> ThrowOnFailureAsync(this Task<ITnTResult> result) {
        var r = await result.ConfigureAwait(false);
        return r.ThrowOnFailure();
    }

    public static async Task<ITnTResult> ThrowOnFailureAsync(this Task<ITnTResult> result, Func<Exception> exceptionCreationFunc) {
        var r = await result.ConfigureAwait(false);
        return r.ThrowOnFailure(exceptionCreationFunc);
    }

    public static async Task<ITnTResult<TSuccess>> ThrowOnFailureAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result) {
        var r = await result.ConfigureAwait(false);
        return r.ThrowOnFailure();
    }

    public static async Task<ITnTResult<TSuccess>> ThrowOnFailureAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<Exception> exceptionCreationFunc) {
        var r = await result.ConfigureAwait(false);
        return r.ThrowOnFailure(exceptionCreationFunc);
    }

    public static async ValueTask<ITnTResult> ThrowOnFailureAsync(this ValueTask<ITnTResult> result, CancellationToken cancellationToken) {
        var r = await result.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        return r.ThrowOnFailure();
    }

    public static async ValueTask<ITnTResult> ThrowOnFailureAsync(this ValueTask<ITnTResult> result, Func<Exception> exceptionCreationFunc, CancellationToken cancellationToken) {
        var r = await result.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        return r.ThrowOnFailure(exceptionCreationFunc);
    }

    public static async ValueTask<ITnTResult<TSuccess>> ThrowOnFailureAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, CancellationToken cancellationToken) {
        var r = await result.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        return r.ThrowOnFailure();
    }

    public static async ValueTask<ITnTResult<TSuccess>> ThrowOnFailureAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<Exception> exceptionCreationFunc, CancellationToken cancellationToken) {
        var r = await result.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        return r.ThrowOnFailure(exceptionCreationFunc);
    }

    public static async Task<ITnTResult> ThrowOnFailureAsync(this Task<ITnTResult> result, CancellationToken cancellationToken) {
        var r = await result.WaitAsync(cancellationToken).ConfigureAwait(false);
        return r.ThrowOnFailure();
    }

    public static async Task<ITnTResult> ThrowOnFailureAsync(this Task<ITnTResult> result, Func<Exception> exceptionCreationFunc, CancellationToken cancellationToken) {
        var r = await result.WaitAsync(cancellationToken).ConfigureAwait(false);
        return r.ThrowOnFailure(exceptionCreationFunc);
    }

    public static async Task<ITnTResult<TSuccess>> ThrowOnFailureAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, CancellationToken cancellationToken) {
        var r = await result.WaitAsync(cancellationToken).ConfigureAwait(false);
        return r.ThrowOnFailure();
    }

    public static async Task<ITnTResult<TSuccess>> ThrowOnFailureAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<Exception> exceptionCreationFunc, CancellationToken cancellationToken) {
        var r = await result.WaitAsync(cancellationToken).ConfigureAwait(false);
        return r.ThrowOnFailure(exceptionCreationFunc);
    }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, returns the specified default value.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result"> The asynchronous result to evaluate.</param>
    /// <param name="default">The value to return when the operation has failed.</param>
    /// <returns>The success value when successful; otherwise, the provided <paramref name="default" /> value.</returns>
    public static async ValueTask<TSuccess> ValueOrAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, TSuccess @default) {
        var r = await result.ConfigureAwait(false);
        return r.ValueOr(@default);
    }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, returns the specified default value. Supports cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">           The asynchronous result to evaluate.</param>
    /// <param name="default">          The value to return when the operation has failed.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The success value when successful; otherwise, the provided <paramref name="default" /> value.</returns>
    public static async ValueTask<TSuccess> ValueOrAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, TSuccess @default, CancellationToken cancellationToken) {
        var r = await result.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        return r.ValueOr(@default);
    }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, returns the specified default value.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result"> The asynchronous result to evaluate.</param>
    /// <param name="default">The value to return when the operation has failed.</param>
    /// <returns>The success value when successful; otherwise, the provided <paramref name="default" /> value.</returns>
    public static async Task<TSuccess> ValueOrAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, TSuccess @default) {
        var r = await result.ConfigureAwait(false);
        return r.ValueOr(@default);
    }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, returns the specified default value. Supports cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">           The asynchronous result to evaluate.</param>
    /// <param name="default">          The value to return when the operation has failed.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The success value when successful; otherwise, the provided <paramref name="default" /> value.</returns>
    public static async Task<TSuccess> ValueOrAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, TSuccess @default, CancellationToken cancellationToken) {
        var r = await result.WaitAsync(cancellationToken).ConfigureAwait(false);
        return r.ValueOr(@default);
    }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, throws the original error that caused the failure.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <returns>The success value.</returns>
    /// <exception cref="Exception">Thrown when the operation has failed. The thrown exception is the original error.</exception>
    public static async ValueTask<TSuccess> ValueOrThrowAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result) {
        var r = await result.ConfigureAwait(false);
        return r.ValueOrThrow();
    }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, throws the specified exception.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">               The asynchronous result to evaluate.</param>
    /// <param name="exceptionCreationFunc">The function to create the exception to throw when the operation has failed.</param>
    /// <returns>The success value.</returns>
    /// <exception cref="Exception">Thrown with the exception returned by <paramref name="exceptionCreationFunc" /> when the operation has failed.</exception>
    public static async ValueTask<TSuccess> ValueOrThrowAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<Exception> exceptionCreationFunc) {
        var r = await result.ConfigureAwait(false);
        return r.ValueOrThrow(exceptionCreationFunc);
    }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, throws the original error that caused the failure. Supports cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">           The asynchronous result to evaluate.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The success value.</returns>
    /// <exception cref="Exception">Thrown when the operation has failed. The thrown exception is the original error.</exception>
    public static async ValueTask<TSuccess> ValueOrThrowAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, CancellationToken cancellationToken) {
        var r = await result.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        return r.ValueOrThrow();
    }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, throws the specified exception. Supports cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">               The asynchronous result to evaluate.</param>
    /// <param name="exceptionCreationFunc">The function to create the exception to throw when the operation has failed.</param>
    /// <param name="cancellationToken">    The cancellation token to observe.</param>
    /// <returns>The success value.</returns>
    /// <exception cref="Exception">Thrown with the exception returned by <paramref name="exceptionCreationFunc" /> when the operation has failed.</exception>
    public static async ValueTask<TSuccess> ValueOrThrowAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<Exception> exceptionCreationFunc, CancellationToken cancellationToken) {
        var r = await result.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        return r.ValueOrThrow(exceptionCreationFunc);
    }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, throws the original error that caused the failure.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <returns>The success value.</returns>
    /// <exception cref="Exception">Thrown when the operation has failed. The thrown exception is the original error.</exception>
    public static async Task<TSuccess> ValueOrThrowAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result) {
        var r = await result.ConfigureAwait(false);
        return r.ValueOrThrow();
    }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, throws the specified exception.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">               The asynchronous result to evaluate.</param>
    /// <param name="exceptionCreationFunc">The function to create the exception to throw when the operation has failed.</param>
    /// <returns>The success value.</returns>
    /// <exception cref="Exception">Thrown with the exception returned by <paramref name="exceptionCreationFunc" /> when the operation has failed.</exception>
    public static async Task<TSuccess> ValueOrThrowAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<Exception> exceptionCreationFunc) {
        var r = await result.ConfigureAwait(false);
        return r.ValueOrThrow(exceptionCreationFunc);
    }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, throws the original error that caused the failure. Supports cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">           The asynchronous result to evaluate.</param>
    /// <param name="cancellationToken">The cancellation token to observe.</param>
    /// <returns>The success value.</returns>
    /// <exception cref="Exception">Thrown when the operation has failed. The thrown exception is the original error.</exception>
    public static async Task<TSuccess> ValueOrThrowAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, CancellationToken cancellationToken) {
        var r = await result.WaitAsync(cancellationToken).ConfigureAwait(false);
        return r.ValueOrThrow();
    }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, throws the specified exception. Supports cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">               The asynchronous result to evaluate.</param>
    /// <param name="exceptionCreationFunc">The function to create the exception to throw when the operation has failed.</param>
    /// <param name="cancellationToken">    The cancellation token to observe.</param>
    /// <returns>The success value.</returns>
    /// <exception cref="Exception">Thrown with the exception returned by <paramref name="exceptionCreationFunc" /> when the operation has failed.</exception>
    public static async Task<TSuccess> ValueOrThrowAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<Exception> exceptionCreationFunc, CancellationToken cancellationToken) {
        var r = await result.WaitAsync(cancellationToken).ConfigureAwait(false);
        return r.ValueOrThrow(exceptionCreationFunc);
    }

    /// <summary>
    ///     Awaits the given Task and applies the continuation function to its result.
    /// </summary>
    private static async Task<ITnTResult> Then(this Task<ITnTResult> task, Func<ITnTResult, ITnTResult> cont) {
        var r = await task.ConfigureAwait(false);
        return cont(r);
    }

    /// <summary>
    ///     Awaits the given ValueTask and applies the continuation function to its result.
    /// </summary>
    private static async ValueTask<ITnTResult> Then(this ValueTask<ITnTResult> task, Func<ITnTResult, ITnTResult> cont) {
        var r = await task.ConfigureAwait(false);
        return cont(r);
    }

    /// <summary>
    ///     Awaits the given Task and applies the continuation function to its result for generic success type.
    /// </summary>
    private static async Task<ITnTResult<TSuccess>> Then<TSuccess>(this Task<ITnTResult<TSuccess>> task, Func<ITnTResult<TSuccess>, ITnTResult<TSuccess>> cont) {
        var r = await task.ConfigureAwait(false);
        return cont(r);
    }

    /// <summary>
    ///     Awaits the given ValueTask and applies the continuation function to its result for generic success type.
    /// </summary>
    private static async ValueTask<ITnTResult<TSuccess>> Then<TSuccess>(this ValueTask<ITnTResult<TSuccess>> task, Func<ITnTResult<TSuccess>, ITnTResult<TSuccess>> cont) {
        var r = await task.ConfigureAwait(false);
        return cont(r);
    }

    /// <summary>
    ///     Awaits the given Task with cancellation and applies the continuation function to its result.
    /// </summary>
    private static async Task<ITnTResult> Then(this Task<ITnTResult> task, CancellationToken cancellationToken, Func<ITnTResult, ITnTResult> cont) {
        var r = await task.WaitAsync(cancellationToken).ConfigureAwait(false);
        return cont(r);
    }

    /// <summary>
    ///     Awaits the given ValueTask with cancellation and applies the continuation function to its result.
    /// </summary>
    private static async ValueTask<ITnTResult> Then(this ValueTask<ITnTResult> task, CancellationToken cancellationToken, Func<ITnTResult, ITnTResult> cont) {
        var r = await task.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        return cont(r);
    }

    /// <summary>
    ///     Awaits the given Task with cancellation and applies the continuation function to its result for generic success type.
    /// </summary>
    private static async Task<ITnTResult<TSuccess>> Then<TSuccess>(this Task<ITnTResult<TSuccess>> task, CancellationToken cancellationToken, Func<ITnTResult<TSuccess>, ITnTResult<TSuccess>> cont) {
        var r = await task.WaitAsync(cancellationToken).ConfigureAwait(false);
        return cont(r);
    }

    /// <summary>
    ///     Awaits the given ValueTask with cancellation and applies the continuation function to its result for generic success type.
    /// </summary>
    private static async ValueTask<ITnTResult<TSuccess>> Then<TSuccess>(this ValueTask<ITnTResult<TSuccess>> task, CancellationToken cancellationToken, Func<ITnTResult<TSuccess>, ITnTResult<TSuccess>> cont) {
        var r = await task.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        return cont(r);
    }

    /// <summary>
    ///     Awaits the given Task and applies the asynchronous continuation function to its result.
    /// </summary>
    private static async Task<ITnTResult> ThenAwait(this Task<ITnTResult> task, Func<ITnTResult, Task> cont) {
        var r = await task.ConfigureAwait(false);
        await cont(r).ConfigureAwait(false);
        return r;
    }

    /// <summary>
    ///     Awaits the given ValueTask and applies the asynchronous continuation function to its result.
    /// </summary>
    private static async ValueTask<ITnTResult> ThenAwait(this ValueTask<ITnTResult> task, Func<ITnTResult, Task> cont) {
        var r = await task.ConfigureAwait(false);
        await cont(r).ConfigureAwait(false);
        return r;
    }

    /// <summary>
    ///     Awaits the given Task and applies the asynchronous continuation function to its result for generic success type.
    /// </summary>
    private static async Task<ITnTResult<TSuccess>> ThenAwait<TSuccess>(this Task<ITnTResult<TSuccess>> task, Func<ITnTResult<TSuccess>, Task> cont) {
        var r = await task.ConfigureAwait(false);
        await cont(r).ConfigureAwait(false);
        return r;
    }

    /// <summary>
    ///     Awaits the given ValueTask and applies the asynchronous continuation function to its result for generic success type.
    /// </summary>
    private static async ValueTask<ITnTResult<TSuccess>> ThenAwait<TSuccess>(this ValueTask<ITnTResult<TSuccess>> task, Func<ITnTResult<TSuccess>, Task> cont) {
        var r = await task.ConfigureAwait(false);
        await cont(r).ConfigureAwait(false);
        return r;
    }

    /// <summary>
    ///     Awaits the given Task with cancellation and applies the asynchronous continuation function to its result.
    /// </summary>
    private static async Task<ITnTResult> ThenAwait(this Task<ITnTResult> task, CancellationToken cancellationToken, Func<ITnTResult, CancellationToken, Task> cont) {
        var r = await task.WaitAsync(cancellationToken).ConfigureAwait(false);
        await cont(r, cancellationToken).ConfigureAwait(false);
        return r;
    }

    /// <summary>
    ///     Awaits the given ValueTask with cancellation and applies the asynchronous continuation function to its result.
    /// </summary>
    private static async ValueTask<ITnTResult> ThenAwait(this ValueTask<ITnTResult> task, CancellationToken cancellationToken, Func<ITnTResult, CancellationToken, Task> cont) {
        var r = await task.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        await cont(r, cancellationToken).ConfigureAwait(false);
        return r;
    }

    /// <summary>
    ///     Awaits the given Task with cancellation and applies the asynchronous continuation function to its result for generic success type.
    /// </summary>
    private static async Task<ITnTResult<TSuccess>> ThenAwait<TSuccess>(this Task<ITnTResult<TSuccess>> task, CancellationToken cancellationToken, Func<ITnTResult<TSuccess>, CancellationToken, Task> cont) {
        var r = await task.WaitAsync(cancellationToken).ConfigureAwait(false);
        await cont(r, cancellationToken).ConfigureAwait(false);
        return r;
    }

    /// <summary>
    ///     Awaits the given ValueTask with cancellation and applies the asynchronous continuation function to its result for generic success type.
    /// </summary>
    private static async ValueTask<ITnTResult<TSuccess>> ThenAwait<TSuccess>(this ValueTask<ITnTResult<TSuccess>> task, CancellationToken cancellationToken, Func<ITnTResult<TSuccess>, CancellationToken, Task> cont) {
        var r = await task.AsTask().WaitAsync(cancellationToken).ConfigureAwait(false);
        await cont(r, cancellationToken).ConfigureAwait(false);
        return r;
    }
}