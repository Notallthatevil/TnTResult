namespace TnTResult.Ext;

/// <summary>
///     Provides extension methods for working with <see cref="Optional{OptType}" /> asynchronously. These methods allow chaining actions or handling the absence of a value in an asynchronous context
///     for both <see cref="Task{Optional{OptType}}" /> and <see cref="ValueTask{Optional{OptType}}" />.
/// </summary>
public static class OptionalExt {

    /// <summary>
    ///     Asynchronously performs an action on the value of the <see cref="Optional{OptType}" /> if it has a value. This overload accepts a <see cref="ValueTask{Optional{OptType}}" />.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optionalTask">The <see cref="ValueTask{Optional{OptType}}" /> representing the optional value.</param>
    /// <param name="action">      The action to perform on the value if present. Must not be null.</param>
    /// <returns>A <see cref="ValueTask{Optional{OptType}}" /> representing the asynchronous operation, containing the optional value.</returns>
    public static async ValueTask<Optional<OptType>> AndThenAsync<OptType>(this ValueTask<Optional<OptType>> optionalTask, Action<OptType> action) {
        ArgumentNullException.ThrowIfNull(action);
        var optional = await optionalTask.ConfigureAwait(false);
        return AndThenHelper(optional, action);
    }

    /// <summary>
    ///     Asynchronously performs an action on the value of the <see cref="Optional{OptType}" /> if it has a value. This overload accepts a <see cref="Task{Optional{OptType}}" />.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optionalTask">The <see cref="Task{Optional{OptType}}" /> representing the optional value.</param>
    /// <param name="action">      The action to perform on the value if present. Must not be null.</param>
    /// <returns>A <see cref="Task{Optional{OptType}}" /> representing the asynchronous operation, containing the optional value.</returns>
    public static async Task<Optional<OptType>> AndThenAsync<OptType>(this Task<Optional<OptType>> optionalTask, Action<OptType> action) {
        ArgumentNullException.ThrowIfNull(action);
        var optional = await optionalTask.ConfigureAwait(false);
        return AndThenHelper(optional, action);
    }

    /// <summary>
    ///     Asynchronously performs an action if the <see cref="Optional{OptType}" /> does not have a value. This overload accepts a <see cref="ValueTask{Optional{OptType}}" />.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optionalTask">The <see cref="ValueTask{Optional{OptType}}" /> representing the optional value.</param>
    /// <param name="action">      The action to perform if the optional does not have a value. Must not be null.</param>
    /// <returns>A <see cref="ValueTask{Optional{OptType}}" /> representing the asynchronous operation, containing the optional value.</returns>
    public static async ValueTask<Optional<OptType>> OrElseAsync<OptType>(this ValueTask<Optional<OptType>> optionalTask, Action action) {
        ArgumentNullException.ThrowIfNull(action);
        var optional = await optionalTask.ConfigureAwait(false);
        return OrElseHelper(optional, action);
    }

    /// <summary>
    ///     Asynchronously performs an action if the <see cref="Optional{OptType}" /> does not have a value. This overload accepts a <see cref="Task{Optional{OptType}}" />.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optionalTask">The <see cref="Task{Optional{OptType}}" /> representing the optional value.</param>
    /// <param name="action">      The action to perform if the optional does not have a value. Must not be null.</param>
    /// <returns>A <see cref="Task{Optional{OptType}}" /> representing the asynchronous operation, containing the optional value.</returns>
    public static async Task<Optional<OptType>> OrElseAsync<OptType>(this Task<Optional<OptType>> optionalTask, Action action) {
        ArgumentNullException.ThrowIfNull(action);
        var optional = await optionalTask.ConfigureAwait(false);
        return OrElseHelper(optional, action);
    }

    /// <summary>
    ///     Helper method for <see cref="AndThenAsync" /> to invoke the action on the value if present.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optional">The optional value.</param>
    /// <param name="action">  The action to perform on the value if present.</param>
    /// <returns>The original <see cref="Optional{OptType}" /> after performing the action if applicable.</returns>
    private static Optional<OptType> AndThenHelper<OptType>(Optional<OptType> optional, Action<OptType> action) => optional.AndThen(action);

    /// <summary>
    ///     Helper method for <see cref="OrElseAsync" /> to invoke the action if the optional does not have a value.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optional">The optional value.</param>
    /// <param name="action">  The action to perform if the optional does not have a value.</param>
    /// <returns>The original <see cref="Optional{OptType}" /> after performing the action if applicable.</returns>
    private static Optional<OptType> OrElseHelper<OptType>(Optional<OptType> optional, Action action) => optional.OrElse(action);
}