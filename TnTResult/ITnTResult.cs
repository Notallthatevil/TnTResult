#nullable enable

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
    bool TryGetValue(out TSuccess? value);
}