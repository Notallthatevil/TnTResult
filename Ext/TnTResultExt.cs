namespace TnTResult.Ext;

/// <summary>
///     Provides extension methods for handling success and failure of <see cref="ITnTResult" /> and <see cref="ITnTResult{TSuccess}" /> asynchronously.
/// </summary>
public static class TnTResultExt {

    /// <summary>
    ///     Executes the specified action if the task result indicates a failure.
    /// </summary>
    /// <param name="result">The task result to check for failure.</param>
    /// <param name="action">The action to execute on failure.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult> OnFailureAsync(this Task<ITnTResult> result, Action<Exception> action) => result.ContinueWith(task => task.Result.OnFailure(action));

    /// <summary>
    ///     Executes the specified action if the value task result indicates a failure.
    /// </summary>
    /// <param name="result">The value task result to check for failure.</param>
    /// <param name="action">The action to execute on failure.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult> OnFailureAsync(this ValueTask<ITnTResult> result, Action<Exception> action) => result.AsTask().OnFailureAsync(action);

    /// <summary>
    ///     Executes the specified asynchronous function if the task result indicates a failure.
    /// </summary>
    /// <param name="result">The task result to check for failure.</param>
    /// <param name="func">  The asynchronous function to execute on failure.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult> OnFailureAsync(this Task<ITnTResult> result, Func<Exception, Task> func) {
        return result.ContinueWith(async task => {
            if (task.Result.HasFailed) {
                await func(task.Result.Error);
            }
            return task.Result;
        }).Unwrap();
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the value task result indicates a failure.
    /// </summary>
    /// <param name="result">The value task result to check for failure.</param>
    /// <param name="func">  The asynchronous function to execute on failure.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult> OnFailureAsync(this ValueTask<ITnTResult> result, Func<Exception, Task> func) => result.AsTask().OnFailureAsync(func);

    /// <summary>
    ///     Executes the specified action if the task result indicates a failure.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The task result to check for failure.</param>
    /// <param name="action">The action to execute on failure.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Action<Exception> action) => result.ContinueWith(task => task.Result.OnFailure(action));

    /// <summary>
    ///     Executes the specified action if the value task result indicates a failure.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The value task result to check for failure.</param>
    /// <param name="action">The action to execute on failure.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Action<Exception> action) => result.AsTask().OnFailureAsync(action);

    /// <summary>
    ///     Executes the specified asynchronous function if the task result indicates a failure.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The task result to check for failure.</param>
    /// <param name="func">  The asynchronous function to execute on failure.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<Exception, Task> func) {
        return result.ContinueWith(async task => {
            if (task.Result.HasFailed) {
                await func(task.Result.Error);
            }
            return task.Result;
        }).Unwrap();
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the value task result indicates a failure.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The value task result to check for failure.</param>
    /// <param name="func">  The asynchronous function to execute on failure.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult<TSuccess>> OnFailureAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<Exception, Task> func) => result.AsTask().OnFailureAsync(func);

    /// <summary>
    ///     Executes the specified action if the task result indicates success.
    /// </summary>
    /// <param name="result">The task result to check for success.</param>
    /// <param name="action">The action to execute on success.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult> OnSuccessAsync(this Task<ITnTResult> result, Action action) => result.ContinueWith(task => task.Result.OnSuccess(action));

    /// <summary>
    ///     Executes the specified action if the value task result indicates success.
    /// </summary>
    /// <param name="result">The value task result to check for success.</param>
    /// <param name="action">The action to execute on success.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult> OnSuccessAsync(this ValueTask<ITnTResult> result, Action action) => result.AsTask().OnSuccessAsync(action);

    /// <summary>
    ///     Executes the specified asynchronous function if the task result indicates success.
    /// </summary>
    /// <param name="result">The task result to check for success.</param>
    /// <param name="func">  The asynchronous function to execute on success.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult> OnSuccessAsync(this Task<ITnTResult> result, Func<Task> func) {
        return result.ContinueWith(async task => {
            if (task.Result.IsSuccessful) {
                await func();
            }
            return task.Result;
        }).Unwrap();
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the value task result indicates success.
    /// </summary>
    /// <param name="result">The value task result to check for success.</param>
    /// <param name="func">  The asynchronous function to execute on success.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult> OnSuccessAsync(this ValueTask<ITnTResult> result, Func<Task> func) => result.AsTask().OnSuccessAsync(func);

    /// <summary>
    ///     Executes the specified action if the task result indicates success.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The task result to check for success.</param>
    /// <param name="action">The action to execute on success.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Action action) => result.ContinueWith(task => task.Result.OnSuccess(action));

    /// <summary>
    ///     Executes the specified action if the value task result indicates success.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The value task result to check for success.</param>
    /// <param name="action">The action to execute on success.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Action action) => result.AsTask().OnSuccessAsync(action);

    /// <summary>
    ///     Executes the specified asynchronous function if the task result indicates success.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The task result to check for success.</param>
    /// <param name="func">  The asynchronous function to execute on success.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<Task> func) {
        return result.ContinueWith(async task => {
            if (task.Result.IsSuccessful) {
                await func();
            }
            return task.Result;
        }).Unwrap();
    }

    /// <summary>
    ///     Executes the specified asynchronous function if the value task result indicates success.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The value task result to check for success.</param>
    /// <param name="func">  The asynchronous function to execute on success.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<Task> func) => result.AsTask().OnSuccessAsync(func);

    /// <summary>
    ///     Executes the specified action with the success value if the task result indicates success.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The task result to check for success.</param>
    /// <param name="action">The action to execute on success.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Action<TSuccess> action) => result.ContinueWith(task => task.Result.OnSuccess(action));

    /// <summary>
    ///     Executes the specified action with the success value if the value task result indicates success.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The value task result to check for success.</param>
    /// <param name="action">The action to execute on success.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Action<TSuccess> action) => result.AsTask().OnSuccessAsync(action);

    /// <summary>
    ///     Executes the specified asynchronous function with the success value if the task result indicates success.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The task result to check for success.</param>
    /// <param name="func">  The asynchronous function to execute on success.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this Task<ITnTResult<TSuccess>> result, Func<TSuccess, Task> func) {
        return result.ContinueWith(async task => {
            if (task.Result.IsSuccessful) {
                await func(task.Result.Value);
            }
            return task.Result;
        }).Unwrap();
    }

    /// <summary>
    ///     Executes the specified asynchronous function with the success value if the value task result indicates success.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <param name="result">The value task result to check for success.</param>
    /// <param name="func">  The asynchronous function to execute on success.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public static Task<ITnTResult<TSuccess>> OnSuccessAsync<TSuccess>(this ValueTask<ITnTResult<TSuccess>> result, Func<TSuccess, Task> func) => result.AsTask().OnSuccessAsync(func);
}