namespace TnTResult.Ext;

/// <summary>
///     Provides extension methods for asynchronous operations on <see cref="Expected{T, ErrorType}" />.
/// </summary>
public static class ExpectedExt {

    /// <summary>
    ///     Applies a function to the value of the <see cref="Expected{T, ErrorType}" /> if it is a
    ///     value, asynchronously.
    /// </summary>
    /// <typeparam name="OutT">The type of the result of the function.</typeparam>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">
    ///     The task containing the <see cref="Expected{T, ErrorType}" />.
    /// </param>
    /// <param name="func">        The function to apply to the value.</param>
    /// <returns>
    ///     A task containing a new <see cref="Expected{OutT, ErrorType}" /> with the result of the
    ///     function or the current error.
    /// </returns>
    public static Task<Expected<OutT, ErrorType>> AndThenAsync<OutT, T, ErrorType>(this Task<Expected<T, ErrorType>> expectedTask, Func<T, OutT> func) => expectedTask.ContinueWith(task => task.Result.AndThen(func));

    /// <summary>
    ///     Applies a function to the value of the <see cref="Expected{T, ErrorType}" /> if it is a
    ///     value, asynchronously.
    /// </summary>
    /// <typeparam name="OutT">The type of the result of the function.</typeparam>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">
    ///     The value task containing the <see cref="Expected{T, ErrorType}" />.
    /// </param>
    /// <param name="func">        The function to apply to the value.</param>
    /// <returns>
    ///     A task containing a new <see cref="Expected{OutT, ErrorType}" /> with the result of the
    ///     function or the current error.
    /// </returns>
    public static Task<Expected<OutT, ErrorType>> AndThenAsync<OutT, T, ErrorType>(this ValueTask<Expected<T, ErrorType>> expectedTask, Func<T, OutT> func) => expectedTask.AsTask().AndThenAsync(func);

    /// <summary>
    ///     Applies a function to the error of the <see cref="Expected{T, ErrorType}" /> if it is an
    ///     error, asynchronously.
    /// </summary>
    /// <typeparam name="OutE">The type of the result of the function.</typeparam>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">
    ///     The task containing the <see cref="Expected{T, ErrorType}" />.
    /// </param>
    /// <param name="func">        The function to apply to the error.</param>
    /// <returns>
    ///     A task containing a new <see cref="Expected{T, OutE}" /> with the result of the function
    ///     or the current value.
    /// </returns>
    public static Task<Expected<T, OutE>> OrElseAsync<OutE, T, ErrorType>(this Task<Expected<T, ErrorType>> expectedTask, Func<ErrorType, OutE> func) => expectedTask.ContinueWith(task => task.Result.OrElse(func));

    /// <summary>
    ///     Applies a function to the error of the <see cref="Expected{T, ErrorType}" /> if it is an
    ///     error, asynchronously.
    /// </summary>
    /// <typeparam name="OutE">The type of the result of the function.</typeparam>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">
    ///     The value task containing the <see cref="Expected{T, ErrorType}" />.
    /// </param>
    /// <param name="func">        The function to apply to the error.</param>
    /// <returns>
    ///     A task containing a new <see cref="Expected{T, OutE}" /> with the result of the function
    ///     or the current value.
    /// </returns>
    public static Task<Expected<T, OutE>> OrElseAsync<OutE, T, ErrorType>(this ValueTask<Expected<T, ErrorType>> expectedTask, Func<ErrorType, OutE> func) => expectedTask.AsTask().OrElseAsync(func);

    /// <summary>
    ///     Transforms the value of the <see cref="Expected{T, ErrorType}" /> if it is a value, asynchronously.
    /// </summary>
    /// <typeparam name="OutT">The type of the result of the function.</typeparam>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">
    ///     The task containing the <see cref="Expected{T, ErrorType}" />.
    /// </param>
    /// <param name="func">        The function to transform the value.</param>
    /// <returns>
    ///     A task containing a new <see cref="Expected{OutT, ErrorType}" /> with the transformed
    ///     value or the current error.
    /// </returns>
    public static Task<Expected<OutT, ErrorType>> TransformAsync<OutT, T, ErrorType>(this Task<Expected<T, ErrorType>> expectedTask, Func<T, OutT> func) => expectedTask.ContinueWith(task => task.Result.Transform(func));

    /// <summary>
    ///     Transforms the value of the <see cref="Expected{T, ErrorType}" /> if it is a value, asynchronously.
    /// </summary>
    /// <typeparam name="OutT">The type of the result of the function.</typeparam>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">
    ///     The value task containing the <see cref="Expected{T, ErrorType}" />.
    /// </param>
    /// <param name="func">        The function to transform the value.</param>
    /// <returns>
    ///     A task containing a new <see cref="Expected{OutT, ErrorType}" /> with the transformed
    ///     value or the current error.
    /// </returns>
    public static Task<Expected<OutT, ErrorType>> TransformAsync<OutT, T, ErrorType>(this ValueTask<Expected<T, ErrorType>> expectedTask, Func<T, OutT> func) => expectedTask.AsTask().TransformAsync(func);

    /// <summary>
    ///     Transforms the error of the <see cref="Expected{T, ErrorType}" /> if it is an error, asynchronously.
    /// </summary>
    /// <typeparam name="OutE">The type of the result of the function.</typeparam>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">
    ///     The task containing the <see cref="Expected{T, ErrorType}" />.
    /// </param>
    /// <param name="func">        The function to transform the error.</param>
    /// <returns>
    ///     A task containing a new <see cref="Expected{T, OutE}" /> with the transformed error or
    ///     the current value.
    /// </returns>
    public static Task<Expected<T, OutE>> TransformErrorAsync<OutE, T, ErrorType>(this Task<Expected<T, ErrorType>> expectedTask, Func<ErrorType, OutE> func) => expectedTask.ContinueWith(task => task.Result.TransformError(func));

    /// <summary>
    ///     Transforms the error of the <see cref="Expected{T, ErrorType}" /> if it is an error, asynchronously.
    /// </summary>
    /// <typeparam name="OutE">The type of the result of the function.</typeparam>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">
    ///     The value task containing the <see cref="Expected{T, ErrorType}" />.
    /// </param>
    /// <param name="func">        The function to transform the error.</param>
    /// <returns>
    ///     A task containing a new <see cref="Expected{T, OutE}" /> with the transformed error or
    ///     the current value.
    /// </returns>
    public static Task<Expected<T, OutE>> TransformErrorAsync<OutE, T, ErrorType>(this ValueTask<Expected<T, ErrorType>> expectedTask, Func<ErrorType, OutE> func) => expectedTask.AsTask().TransformErrorAsync(func);

    /// <summary>
    ///     Gets the value of the <see cref="Expected{T, ErrorType}" /> if it is a value, otherwise
    ///     returns the specified default value, asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">
    ///     The task containing the <see cref="Expected{T, ErrorType}" />.
    /// </param>
    /// <param name="defaultValue">The default value to return if the result is an error.</param>
    /// <returns>
    ///     A task containing the value if the result is a value, otherwise the specified default value.
    /// </returns>
    public static Task<T> ValueOrAsync<T, ErrorType>(this Task<Expected<T, ErrorType>> expectedTask, T defaultValue) => expectedTask.ContinueWith(task => task.Result.ValueOr(defaultValue));

    /// <summary>
    ///     Gets the value of the <see cref="Expected{T, ErrorType}" /> if it is a value, otherwise
    ///     returns the specified default value, asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">
    ///     The value task containing the <see cref="Expected{T, ErrorType}" />.
    /// </param>
    /// <param name="defaultValue">The default value to return if the result is an error.</param>
    /// <returns>
    ///     A task containing the value if the result is a value, otherwise the specified default value.
    /// </returns>
    public static Task<T> ValueOrAsync<T, ErrorType>(this ValueTask<Expected<T, ErrorType>> expectedTask, T defaultValue) => expectedTask.AsTask().ValueOrAsync(defaultValue);
}