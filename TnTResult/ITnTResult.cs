namespace TnTResult;

/// <summary>
///     Represents the result of an operation.
/// </summary>
public interface ITnTResult {

    /// <summary>
    ///     Gets the exception if the operation was not successful.
    /// </summary>
    Exception Error { get; }

    /// <summary>
    ///     Gets the error message if the operation was not successful.
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
    ///     Executes the specified action if the operation has failed.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The current result instance.</returns>
    ITnTResult OnFailure(Action<Exception> action);

    /// <summary>
    ///     Executes the specified action if the operation was successful.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The current result instance.</returns>
    ITnTResult OnSuccess(Action action);
}

/// <summary>
///     Represents the result of an operation with a specific success value.
/// </summary>
/// <typeparam name="TSuccess">The type of the success value.</typeparam>
public interface ITnTResult<TSuccess> : ITnTResult {

    /// <summary>
    ///     Gets the success value if the operation was successful.
    /// </summary>
    TSuccess Value { get; }

    /// <summary>
    ///     Executes the specified action if the operation has failed.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The current result instance.</returns>
    new ITnTResult<TSuccess> OnFailure(Action<Exception> action);

    /// <summary>
    ///     Executes the specified action if the operation was successful.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The current result instance.</returns>
    ITnTResult<TSuccess> OnSuccess(Action<TSuccess> action);

    /// <summary>
    ///     Executes the specified action if the operation was successful.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The current result instance.</returns>
    new ITnTResult<TSuccess> OnSuccess(Action action);
}