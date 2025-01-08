namespace TnTResult.Ext;

/// <summary>
///     Provides extension methods for working with <see cref="Optional{OptType}" /> asynchronously.
/// </summary>
public static class OptionalExt {

    /// <summary>
    ///     Performs an action on the value of the optional if it has a value, asynchronously.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optionalTask">The task that represents the optional value.</param>
    /// <param name="action">      The action to perform on the value.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation, containing the optional value.
    /// </returns>
    public static Task<Optional<OptType>> AndThenAsync<OptType>(this ValueTask<Optional<OptType>> optionalTask, Action<OptType> action) => optionalTask.AsTask().AndThenAsync(action);

    /// <summary>
    ///     Performs an action on the value of the optional if it has a value, asynchronously.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optionalTask">The task that represents the optional value.</param>
    /// <param name="action">      The action to perform on the value.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation, containing the optional value.
    /// </returns>
    public static Task<Optional<OptType>> AndThenAsync<OptType>(this Task<Optional<OptType>> optionalTask, Action<OptType> action) => optionalTask.ContinueWith(c => c.Result.AndThen(action));

    /// <summary>
    ///     Performs an action if the optional does not have a value, asynchronously.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optionalTask">The task that represents the optional value.</param>
    /// <param name="action">      The action to perform if the optional does not have a value.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation, containing the optional value.
    /// </returns>
    public static Task<Optional<OptType>> OrElseAsync<OptType>(this Task<Optional<OptType>> optionalTask, Action action) => optionalTask.ContinueWith(c => c.Result.OrElse(action));

    /// <summary>
    ///     Performs an action if the optional does not have a value, asynchronously.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optionalTask">The task that represents the optional value.</param>
    /// <param name="action">      The action to perform if the optional does not have a value.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation, containing the optional value.
    /// </returns>
    public static Task<Optional<OptType>> OrElseAsync<OptType>(this ValueTask<Optional<OptType>> optionalTask, Action action) => optionalTask.AsTask().OrElseAsync(action);
}