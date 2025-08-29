#nullable enable

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TnTResult;

/// <summary>
///     Represents the result of an operation that can either succeed or fail.
/// </summary>
public interface ITnTResult {

    /// <summary>
    ///     Gets the exception that caused the operation to fail. Only valid when <see cref="HasFailed" /> is <c>true</c>.
    /// </summary>
    Exception Error { get; }

    /// <summary>
    ///     Gets the error message describing why the operation failed. Only valid when <see cref="HasFailed" /> is <c>true</c>.
    /// </summary>
    string ErrorMessage { get; }

    /// <summary>
    ///     Gets a value indicating whether the operation has failed.
    /// </summary>
    bool HasFailed { get; }

    /// <summary>
    ///     Gets a value indicating whether the operation was successful.
    /// </summary>
    bool IsSuccessful { get; }

    /// <summary>
    ///     Executes the specified action regardless of success or failure.
    /// </summary>
    /// <param name="action">The action to execute after the operation completes.</param>
    void Finally(Action action);

    /// <summary>
    ///     Executes the specified action if the operation has failed.
    /// </summary>
    /// <param name="action">The action to execute with the error that caused the failure.</param>
    /// <returns>The current result instance for method chaining.</returns>
    ITnTResult OnFailure(Action<Exception> action);

    /// <summary>
    ///     Executes the specified action if the operation was successful.
    /// </summary>
    /// <param name="action">The action to execute on success.</param>
    /// <returns>The current result instance for method chaining.</returns>
    ITnTResult OnSuccess(Action action);

    /// <summary>
    ///     Throws the original error when the operation has failed; otherwise, returns the current result for chaining.
    /// </summary>
    /// <returns>The current result instance when the operation did not fail.</returns>
    /// <exception cref="Exception">Thrown with the original <see cref="Error" /> when <see cref="HasFailed" /> is <c>true</c>.</exception>
    ITnTResult ThrowOnFailure() => HasFailed ? throw Error : this;

    /// <summary>
    ///     Throws a custom exception when the operation has failed; otherwise, returns the current result for chaining.
    /// </summary>
    /// <param name="exceptionCreationFunc">A factory that creates the exception to throw when the operation has failed.</param>
    /// <returns>The current result instance when the operation did not fail.</returns>
    /// <exception cref="Exception">Thrown with the exception returned by <paramref name="exceptionCreationFunc"/> when <see cref="HasFailed"/> is <c>true</c>.</exception>
    ITnTResult ThrowOnFailure(Func<Exception> exceptionCreationFunc) => HasFailed ? throw exceptionCreationFunc() : this;
}

/// <summary>
///     Represents the result of an operation with a specific success value type.
/// </summary>
/// <typeparam name="TSuccess">The type of the success value.</typeparam>
public interface ITnTResult<TSuccess> : ITnTResult {

    /// <summary>
    ///     Gets the success value if the operation was successful. Only valid when <see cref="ITnTResult.IsSuccessful" /> is <c>true</c>.
    /// </summary>
    TSuccess Value { get; }

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, returns the default value for <typeparamref name="TSuccess" />.
    /// </summary>
    /// <returns>The success value when successful; otherwise, the default value for <typeparamref name="TSuccess" />.</returns>
    /// <remarks>Convenience wrapper around <see cref="TryGetValue" /> that never throws.</remarks>
    TSuccess GetValueOrDefault() => TryGetValue(out var value) ? value! : default!;

    /// <summary>
    ///     Executes the specified action if the operation has failed.
    /// </summary>
    /// <param name="action">The action to execute with the error that caused the failure.</param>
    /// <returns>The current result instance for method chaining.</returns>
    new ITnTResult<TSuccess> OnFailure(Action<Exception> action);

    /// <summary>
    ///     Executes the specified action if the operation was successful.
    /// </summary>
    /// <param name="action">The action to execute with the success value.</param>
    /// <returns>The current result instance for method chaining.</returns>
    ITnTResult<TSuccess> OnSuccess(Action<TSuccess> action);

    /// <summary>
    ///     Executes the specified action if the operation was successful.
    /// </summary>
    /// <param name="action">The action to execute on success.</param>
    /// <returns>The current result instance for method chaining.</returns>
    new ITnTResult<TSuccess> OnSuccess(Action action);

    /// <summary>
    ///     Tries to get the success value.
    /// </summary>
    /// <param name="value">When this method returns, contains the success value if successful; otherwise, the default value.</param>
    /// <returns><c>true</c> if the operation was successful; otherwise, <c>false</c>.</returns>
    bool TryGetValue(out TSuccess value);

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, returns the specified default value.
    /// </summary>
    /// <param name="default">The value to return when the operation has failed.</param>
    /// <returns>The success value when successful; otherwise, the provided <paramref name="default" /> value.</returns>
    /// <remarks>Convenience wrapper around <see cref="TryGetValue" /> that never throws.</remarks>
    TSuccess ValueOr(TSuccess @default) => TryGetValue(out var value) ? value! : @default;

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, throws the original error that caused the failure.
    /// </summary>
    /// <returns>The success value.</returns>
    /// <exception cref="Exception">Thrown when the operation has failed. The thrown exception is the original <see cref="Error" />.</exception>
    TSuccess ValueOrThrow() => TryGetValue(out var value) ? value! : throw Error;

    /// <summary>
    ///     Gets the success value if the operation was successful; otherwise, throws a custom exception.
    /// </summary>
    /// <param name="exceptionCreationFunc">A factory that creates the exception to throw when the operation has failed.</param>
    /// <returns>The success value.</returns>
    /// <exception cref="Exception">Thrown with the exception returned by <paramref name="exceptionCreationFunc"/> when the operation has failed.</exception>
    TSuccess ValueOrThrow(Func<Exception> exceptionCreationFunc) => TryGetValue(out var value) ? value! : throw exceptionCreationFunc();

    /// <summary>
    ///     Throws the original error when the operation has failed; otherwise, returns the current result for chaining.
    ///     This member hides <see cref="ITnTResult.ThrowOnFailure()"/> to return a typed result.
    /// </summary>
    /// <returns>The current typed result instance when the operation did not fail.</returns>
    /// <exception cref="Exception">Thrown with the original <see cref="Error" /> when <see cref="ITnTResult.HasFailed" /> is <c>true</c>.</exception>
    new ITnTResult<TSuccess> ThrowOnFailure() => HasFailed ? throw Error : this;

    /// <summary>
    ///     Throws a custom exception when the operation has failed; otherwise, returns the current result for chaining.
    ///     This member hides <see cref="ITnTResult.ThrowOnFailure(System.Func{System.Exception})"/> to return a typed result.
    /// </summary>
    /// <param name="exceptionCreationFunc">A factory that creates the exception to throw when the operation has failed.</param>
    /// <returns>The current typed result instance when the operation did not fail.</returns>
    /// <exception cref="Exception">Thrown with the exception returned by <paramref name="exceptionCreationFunc"/> when <see cref="ITnTResult.HasFailed"/> is <c>true</c>.</exception>
    new ITnTResult<TSuccess> ThrowOnFailure(Func<Exception> exceptionCreationFunc) => HasFailed ? throw exceptionCreationFunc(): this;
}