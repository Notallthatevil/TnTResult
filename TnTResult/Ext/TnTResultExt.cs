namespace TnTResult.Ext;

/// <summary>
///     Provides extension methods for handling success and failure of <see cref="ITnTResult" /> and <see cref="ITnTResult{TSuccess}" /> asynchronously.
/// </summary>
public static class TnTResultExt {

    /// <summary>
    ///     Executes the specified action if the result has failed.
    /// </summary>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="action">The action to execute on failure.</param>
    /// <returns>The original result.</returns>
    public static async Task<ITnTResult> OnFailureAsync(this Task<ITnTResult> result, Action<Exception> action) {
        var r = await result.ConfigureAwait(false);
        return r.OnFailure(action);
    }

    /// <summary>
    ///     Executes the specified action if the result has failed.
    /// </summary>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="action">The action to execute on failure.</param>
    /// <returns>The original result.</returns>
    public static async ValueTask<ITnTResult> OnFailureAsync(this ValueTask<ITnTResult> result, Action<Exception> action) {
        var r = await result.ConfigureAwait(false);
        return r.OnFailure(action);
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result has failed.
    /// </summary>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="func">  The asynchronous function to execute on failure.</param>
    /// <returns>The original result.</returns>
    public static async Task<ITnTResult> OnFailureAsync(this Task<ITnTResult> result, Func<Exception, Task> func) {
        var r = await result.ConfigureAwait(false);
        if (r.HasFailed) {
            await func(r.Error).ConfigureAwait(false);
        }
        return r;
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result has failed.
    /// </summary>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="func">  The asynchronous function to execute on failure.</param>
    /// <returns>The original result.</returns>
    public static async ValueTask<ITnTResult> OnFailureAsync(this ValueTask<ITnTResult> result, Func<Exception, Task> func) {
        var r = await result.ConfigureAwait(false);
        if (r.HasFailed) {
            await func(r.Error).ConfigureAwait(false);
        }
        return r;
    }

    /// <summary>
    ///     Executes the specified action if the result has failed.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="action">The action to execute on failure.</param>
    /// <returns>The original result.</returns>
    public static async Task<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Action<Exception> action) {
        var r = await result.ConfigureAwait(false);
        return r.OnFailure(action);
    }

    /// <summary>
    ///     Executes the specified action if the result has failed.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="action">The action to execute on failure.</param>
    /// <returns>The original result.</returns>
    public static async ValueTask<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Action<Exception> action) {
        var r = await result.ConfigureAwait(false);
        return r.OnFailure(action);
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result has failed.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="func">  The asynchronous function to execute on failure.</param>
    /// <returns>The original result.</returns>
    public static async Task<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<Exception, Task> func) {
        var r = await result.ConfigureAwait(false);
        if (r.HasFailed) {
            await func(r.Error).ConfigureAwait(false);
        }
        return r;
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result has failed.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="func">  The asynchronous function to execute on failure.</param>
    /// <returns>The original result.</returns>
    public static async ValueTask<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<Exception, Task> func) {
        var r = await result.ConfigureAwait(false);
        if (r.HasFailed) {
            await func(r.Error).ConfigureAwait(false);
        }
        return r;
    }

    /// <summary>
    ///     Executes the specified action if the result is successful.
    /// </summary>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="action">The action to execute on success.</param>
    /// <returns>The original result.</returns>
    public static async Task<ITnTResult> OnSuccessAsync(this Task<ITnTResult> result, Action action) {
        var r = await result.ConfigureAwait(false);
        return r.OnSuccess(action);
    }

    /// <summary>
    ///     Executes the specified action if the result is successful.
    /// </summary>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="action">The action to execute on success.</param>
    /// <returns>The original result.</returns>
    public static async ValueTask<ITnTResult> OnSuccessAsync(this ValueTask<ITnTResult> result, Action action) {
        var r = await result.ConfigureAwait(false);
        return r.OnSuccess(action);
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result is successful.
    /// </summary>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="func">  The asynchronous function to execute on success.</param>
    /// <returns>The original result.</returns>
    public static async Task<ITnTResult> OnSuccessAsync(this Task<ITnTResult> result, Func<Task> func) {
        var r = await result.ConfigureAwait(false);
        if (r.IsSuccessful) {
            await func().ConfigureAwait(false);
        }
        return r;
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result is successful.
    /// </summary>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="func">  The asynchronous function to execute on success.</param>
    /// <returns>The original result.</returns>
    public static async ValueTask<ITnTResult> OnSuccessAsync(this ValueTask<ITnTResult> result, Func<Task> func) {
        var r = await result.ConfigureAwait(false);
        if (r.IsSuccessful) {
            await func().ConfigureAwait(false);
        }
        return r;
    }

    /// <summary>
    ///     Executes the specified action if the result is successful.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="action">The action to execute on success.</param>
    /// <returns>The original result.</returns>
    public static async Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Action action) {
        var r = await result.ConfigureAwait(false);
        return r.OnSuccess(action);
    }

    /// <summary>
    ///     Executes the specified action if the result is successful.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="action">The action to execute on success.</param>
    /// <returns>The original result.</returns>
    public static async ValueTask<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Action action) {
        var r = await result.ConfigureAwait(false);
        return r.OnSuccess(action);
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result is successful.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="func">  The asynchronous function to execute on success.</param>
    /// <returns>The original result.</returns>
    public static async Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<Task> func) {
        var r = await result.ConfigureAwait(false);
        if (r.IsSuccessful) {
            await func().ConfigureAwait(false);
        }
        return r;
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the result is successful.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="func">  The asynchronous function to execute on success.</param>
    /// <returns>The original result.</returns>
    public static async ValueTask<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<Task> func) {
        var r = await result.ConfigureAwait(false);
        if (r.IsSuccessful) {
            await func().ConfigureAwait(false);
        }
        return r;
    }

    /// <summary>
    ///     Executes the specified action with the success value if the result is successful.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="action">The action to execute on success.</param>
    /// <returns>The original result.</returns>
    public static async Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Action<TSuccess> action) {
        var r = await result.ConfigureAwait(false);
        return r.OnSuccess(action);
    }

    /// <summary>
    ///     Executes the specified action with the success value if the result is successful.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="action">The action to execute on success.</param>
    /// <returns>The original result.</returns>
    public static async ValueTask<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Action<TSuccess> action) {
        var r = await result.ConfigureAwait(false);
        return r.OnSuccess(action);
    }

    /// <summary>
    ///     Executes the specified asynchronous function with the success value if the result is successful.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="func">  The asynchronous function to execute on success.</param>
    /// <returns>The original result.</returns>
    public static async Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<TSuccess, Task> func) {
        var r = await result.ConfigureAwait(false);
        if (r.IsSuccessful) {
            await func(r.Value).ConfigureAwait(false);
        }
        return r;
    }

    /// <summary>
    ///     Executes the specified asynchronous function with the success value if the result is successful.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="func">  The asynchronous function to execute on success.</param>
    /// <returns>The original result.</returns>
    public static async ValueTask<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<TSuccess, Task> func) {
        var r = await result.ConfigureAwait(false);
        if (r.IsSuccessful) {
            await func(r.Value).ConfigureAwait(false);
        }
        return r;
    }
}