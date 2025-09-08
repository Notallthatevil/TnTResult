namespace TnTResult.Ext;

/// <summary>
///     Provides extension methods for working with <see cref="Optional{OptType}" /> asynchronously. These methods allow chaining actions or handling the absence of a value in an asynchronous context
///     for both <see cref="Task" /> and <see cref="ValueTask" /> that produce an <see cref="Optional{OptType}" /> value.
/// </summary>
public static class OptionalExt {

    /// <summary>
    ///     Asynchronously performs an action on the value of the <see cref="Optional{OptType}" /> if it has a value. This overload accepts a <see cref="ValueTask{TResult}" /> whose result is an <see
    ///     cref="Optional{OptType}" />.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optionalTask">The <see cref="ValueTask{TResult}" /> producing the <see cref="Optional{OptType}" /> value.</param>
    /// <param name="action">      The action to perform on the value if present. Must not be null.</param>
    /// <returns>A <see cref="ValueTask{TResult}" /> whose result is the original <see cref="Optional{OptType}" />.</returns>
    public static async ValueTask<Optional<OptType>> AndThenAsync<OptType>(this ValueTask<Optional<OptType>> optionalTask, Action<OptType> action) {
        ArgumentNullException.ThrowIfNull(action);
        var optional = await optionalTask.ConfigureAwait(false);
        return AndThenHelper(optional, action);
    }

    /// <summary>
    ///     Asynchronously performs an action on the value of the <see cref="Optional{OptType}" /> if it has a value. This overload accepts a <see cref="Task{TResult}" /> whose result is an <see
    ///     cref="Optional{OptType}" />.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optionalTask">The <see cref="Task{TResult}" /> producing the <see cref="Optional{OptType}" /> value.</param>
    /// <param name="action">      The action to perform on the value if present. Must not be null.</param>
    /// <returns>A <see cref="Task{TResult}" /> whose result is the original <see cref="Optional{OptType}" />.</returns>
    public static async Task<Optional<OptType>> AndThenAsync<OptType>(this Task<Optional<OptType>> optionalTask, Action<OptType> action) {
        ArgumentNullException.ThrowIfNull(action);
        var optional = await optionalTask.ConfigureAwait(false);
        return AndThenHelper(optional, action);
    }

    /// <summary>
    ///     Asynchronously performs an action if the <see cref="Optional{OptType}" /> does not have a value. This overload accepts a <see cref="ValueTask{TResult}" /> whose result is an <see
    ///     cref="Optional{OptType}" />.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optionalTask">The <see cref="ValueTask{TResult}" /> producing the <see cref="Optional{OptType}" /> value.</param>
    /// <param name="action">      The action to perform if the optional does not have a value. Must not be null.</param>
    /// <returns>A <see cref="ValueTask{TResult}" /> whose result is the original <see cref="Optional{OptType}" />.</returns>
    public static async ValueTask<Optional<OptType>> OrElseAsync<OptType>(this ValueTask<Optional<OptType>> optionalTask, Action action) {
        ArgumentNullException.ThrowIfNull(action);
        var optional = await optionalTask.ConfigureAwait(false);
        return OrElseHelper(optional, action);
    }

    /// <summary>
    ///     Asynchronously performs an action if the <see cref="Optional{OptType}" /> does not have a value. This overload accepts a <see cref="Task{TResult}" /> whose result is an <see
    ///     cref="Optional{OptType}" />.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optionalTask">The <see cref="Task{TResult}" /> producing the <see cref="Optional{OptType}" /> value.</param>
    /// <param name="action">      The action to perform if the optional does not have a value. Must not be null.</param>
    /// <returns>A <see cref="Task{TResult}" /> whose result is the original <see cref="Optional{OptType}" />.</returns>
    public static async Task<Optional<OptType>> OrElseAsync<OptType>(this Task<Optional<OptType>> optionalTask, Action action) {
        ArgumentNullException.ThrowIfNull(action);
        var optional = await optionalTask.ConfigureAwait(false);
        return OrElseHelper(optional, action);
    }

    /// <summary>
    ///     Helper method used to invoke the action on the value if present for the asynchronous <c>AndThenAsync</c> overloads.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optional">The optional value.</param>
    /// <param name="action">  The action to perform on the value if present.</param>
    /// <returns>The original <see cref="Optional{OptType}" /> after performing the action if applicable.</returns>
    private static Optional<OptType> AndThenHelper<OptType>(Optional<OptType> optional, Action<OptType> action) => optional.AndThen(action);

    /// <summary>
    ///     Helper method used to invoke the action if the optional does not have a value for the asynchronous <c>OrElseAsync</c> overloads.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optional">The optional value.</param>
    /// <param name="action">  The action to perform if the optional does not have a value.</param>
    /// <returns>The original <see cref="Optional{OptType}" /> after performing the action if applicable.</returns>
    private static Optional<OptType> OrElseHelper<OptType>(Optional<OptType> optional, Action action) => optional.OrElse(action);
}