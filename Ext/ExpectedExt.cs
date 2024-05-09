namespace TnTResult.Ext;

public static class ExpectedExt {

    /// <summary>
    /// Chains an asynchronous operation to the result of the <see cref="Expected{T, ErrorType}" />.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">The task representing the expected result.</param>
    /// <param name="func">The asynchronous operation to be chained.</param>
    /// <returns>A task representing the result of the chained operation.</returns>
    public static async Task<Expected<T, ErrorType>> AndThenAsync<T, ErrorType>(this Task<Expected<T, ErrorType>> expectedTask, Func<T?, Task> func) {
        return await (await expectedTask).AndThenAsync(func);
    }

    /// <summary>
    /// Chains an action to the result of the <see cref="Expected{T, ErrorType}" />.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">The task representing the expected result.</param>
    /// <param name="action">The action to be chained.</param>
    /// <returns>An instance of <see cref="Expected{T, ErrorType}" /> after applying the action.</returns>
    public static async ValueTask<Expected<T, ErrorType>> AndThenAsync<T, ErrorType>(this Task<Expected<T, ErrorType>> expectedTask, Action<T?> action) {
        return (await expectedTask).AndThen(action);
    }

    /// <summary>
    /// Chains an asynchronous operation to the result of the <see cref="Expected{T, ErrorType}" />.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">The task representing the expected result.</param>
    /// <param name="func">The asynchronous operation to be chained.</param>
    /// <returns>A task representing the result of the chained operation.</returns>
    public static async Task<Expected<T, ErrorType>> AndThenAsync<T, ErrorType>(this ValueTask<Expected<T, ErrorType>> expectedTask, Func<T?, Task> func) {
        return await (await expectedTask).AndThenAsync(func);
    }

    /// <summary>
    /// Chains an action to the result of the <see cref="Expected{T, ErrorType}" />.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">The task representing the expected result.</param>
    /// <param name="action">The action to be chained.</param>
    /// <returns>An instance of <see cref="Expected{T, ErrorType}" /> after applying the action.</returns>
    public static async ValueTask<Expected<T, ErrorType>> AndThenAsync<T, ErrorType>(this ValueTask<Expected<T, ErrorType>> expectedTask, Action<T?> action) {
        return (await expectedTask).AndThen(action);
    }

    /// <summary>
    /// Creates an instance of <see cref="Expected{T, ErrorType}" /> with the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="t">The value to be wrapped in the expected result.</param>
    /// <returns>
    /// An instance of <see cref="Expected{T, ErrorType}" /> containing the specified value.
    /// </returns>
    public static Expected<T, ErrorType> MakeExpected<T, ErrorType>(this T? t) {
        return Expected<T, ErrorType>.MakeExpected(t);
    }

    /// <summary>
    /// Creates a <see cref="ValueTask{Expected{T, ErrorType}}" /> with the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="t">The value to be wrapped in the expected result.</param>
    /// <returns>
    /// A <see cref="ValueTask{Expected{T, ErrorType}}" /> containing the specified value.
    /// </returns>
    public static ValueTask<Expected<T, ErrorType>> MakeExpectedAsync<T, ErrorType>(this T? t) {
        return new ValueTask<Expected<T, ErrorType>>(t.MakeExpected<T, ErrorType>());
    }

    /// <summary>
    /// Creates an instance of <see cref="Expected{T, ErrorType}" /> with the specified error.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="error">The error to be wrapped in the expected result.</param>
    /// <returns>
    /// An instance of <see cref="Expected{T, ErrorType}" /> containing the specified error.
    /// </returns>
    public static Expected<T, ErrorType> MakeUnexpected<T, ErrorType>(this ErrorType error) {
        return Expected<T, ErrorType>.MakeUnexpected(error);
    }

    /// <summary>
    /// Creates a <see cref="ValueTask{Expected{T, ErrorType}}" /> with the specified error.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="error">The error to be wrapped in the expected result.</param>
    /// <returns>
    /// A <see cref="ValueTask{Expected{T, ErrorType}}" /> containing the specified error.
    /// </returns>
    public static ValueTask<Expected<T, ErrorType>> MakeUnexpectedAsync<T, ErrorType>(this ErrorType error) {
        return new ValueTask<Expected<T, ErrorType>>(error.MakeUnexpected<T, ErrorType>());
    }

    /// <summary>
    /// Chains an asynchronous operation to the error of the <see cref="Expected{T, ErrorType}" />.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">The task representing the expected result.</param>
    /// <param name="func">The asynchronous operation to be chained.</param>
    /// <returns>A task representing the result of the chained operation.</returns>
    public static async Task<Expected<T, ErrorType>> OrElseAsync<T, ErrorType>(this Task<Expected<T, ErrorType>> expectedTask, Func<ErrorType, Task> func) {
        return await (await expectedTask).OrElseAsync(func);
    }

    /// <summary>
    /// Chains an action to the error of the <see cref="Expected{T, ErrorType}" />.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">The task representing the expected result.</param>
    /// <param name="action">The action to be chained.</param>
    /// <returns>An instance of <see cref="Expected{T, ErrorType}" /> after applying the action.</returns>
    public static async ValueTask<Expected<T, ErrorType>> OrElseAsync<T, ErrorType>(this Task<Expected<T, ErrorType>> expectedTask, Action<ErrorType> action) {
        return (await expectedTask).OrElse(action);
    }

    /// <summary>
    /// Chains an asynchronous operation to the error of the <see cref="Expected{T, ErrorType}" />.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">The task representing the expected result.</param>
    /// <param name="func">The asynchronous operation to be chained.</param>
    /// <returns>A task representing the result of the chained operation.</returns>
    public static async Task<Expected<T, ErrorType>> OrElseAsync<T, ErrorType>(this ValueTask<Expected<T, ErrorType>> expectedTask, Func<ErrorType, Task> func) {
        return await (await expectedTask).OrElseAsync(func);
    }

    /// <summary>
    /// Chains an action to the error of the <see cref="Expected{T, ErrorType}" />.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">The task representing the expected result.</param>
    /// <param name="action">The action to be chained.</param>
    /// <returns>An instance of <see cref="Expected{T, ErrorType}" /> after applying the action.</returns>
    public static async ValueTask<Expected<T, ErrorType>> OrElseAsync<T, ErrorType>(this ValueTask<Expected<T, ErrorType>> expectedTask, Action<ErrorType> action) {
        return (await expectedTask).OrElse(action);
    }

    /// <summary>
    /// Transforms the value of the <see cref="Expected{T, ErrorType}" /> using an asynchronous operation.
    /// </summary>
    /// <typeparam name="U">The type of the transformed value.</typeparam>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">The task representing the expected result.</param>
    /// <param name="func">The asynchronous operation to transform the value.</param>
    /// <returns>A task representing the transformed result.</returns>
    public static async Task<Expected<U, ErrorType>> TransformAsync<U, T, ErrorType>(this Task<Expected<T, ErrorType>> expectedTask, Func<T?, Task<U>> func) {
        return await (await expectedTask).TransformAsync(func);
    }

    /// <summary>
    /// Transforms the value of the <see cref="Expected{T, ErrorType}" /> using an asynchronous operation.
    /// </summary>
    /// <typeparam name="U">The type of the transformed value.</typeparam>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">The task representing the expected result.</param>
    /// <param name="func">The asynchronous operation to transform the value.</param>
    /// <returns>A task representing the transformed result.</returns>
    public static async Task<Expected<U, ErrorType>> TransformAsync<U, T, ErrorType>(this ValueTask<Expected<T, ErrorType>> expectedTask, Func<T?, Task<U>> func) {
        return await (await expectedTask).TransformAsync(func);
    }

    /// <summary>
    /// Transforms the error of the <see cref="Expected{T, ErrorType}" /> using an asynchronous operation.
    /// </summary>
    /// <typeparam name="E">The type of the transformed error.</typeparam>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">The task representing the expected result.</param>
    /// <param name="func">The asynchronous operation to transform the error.</param>
    /// <returns>A task representing the transformed result.</returns>
    public static async Task<Expected<T, E>> TransformErrorAsync<E, T, ErrorType>(this Task<Expected<T, ErrorType>> expectedTask, Func<ErrorType, Task<E>> func) {
        return await (await expectedTask).TransformErrorAsync(func);
    }

    /// <summary>
    /// Transforms the error of the <see cref="Expected{T, ErrorType}" /> using an asynchronous operation.
    /// </summary>
    /// <typeparam name="E">The type of the transformed error.</typeparam>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">The task representing the expected result.</param>
    /// <param name="func">The asynchronous operation to transform the error.</param>
    /// <returns>A task representing the transformed result.</returns>
    public static async Task<Expected<T, E>> TransformErrorAsync<E, T, ErrorType>(this ValueTask<Expected<T, ErrorType>> expectedTask, Func<ErrorType, Task<E>> func) {
        return await (await expectedTask).TransformErrorAsync(func);
    }

    /// <summary>
    /// Gets the value of the <see cref="Expected{T, ErrorType}" /> or a default value asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">The task representing the expected result.</param>
    /// <param name="defaultValue">The default value to return if the expected result is empty.</param>
    /// <returns>The value of the expected result or the default value.</returns>
    public static async ValueTask<T?> ValueOrAsync<T, ErrorType>(this Task<Expected<T, ErrorType>> expectedTask, T? defaultValue = default) {
        return (await expectedTask).ValueOr(defaultValue);
    }

    /// <summary>
    /// Gets the value of the <see cref="Expected{T, ErrorType}" /> or a default value asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="expectedTask">The task representing the expected result.</param>
    /// <param name="defaultValue">The default value to return if the expected result is empty.</param>
    /// <returns>The value of the expected result or the default value.</returns>
    public static async ValueTask<T?> ValueOrAsync<T, ErrorType>(this ValueTask<Expected<T, ErrorType>> expectedTask, T? defaultValue = default) {
        return (await expectedTask).ValueOr(defaultValue);
    }
}