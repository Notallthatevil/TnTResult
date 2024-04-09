namespace TnTResult;

/// <summary>
/// Represents the result of an operation.
/// </summary>
public interface ITnTResult {

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    static ITnTResult Successful => TnTResult.Successful;

    /// <summary>
    /// Gets the exception if the operation was not successful.
    /// </summary>
    Exception Error { get; }

    /// <summary>
    /// Gets the error message if the operation was not successful.
    /// </summary>
    string ErrorMessage { get; }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    bool IsSuccessful { get; }

    /// <summary>
    /// Creates a failure result with the specified exception.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <returns>A failure TnTResult instance.</returns>
    static ITnTResult Failure(Exception ex) {
        return new TnTResult(ex);
    }

    /// <summary>
    /// Creates a failure result with the specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A failure TnTResult instance.</returns>
    static ITnTResult Failure(string message) {
        return new TnTResult(new Exception(message));
    }

    /// <summary>
    /// Executes the specified action if the result is a failure.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The current TnTResult instance.</returns>
    ITnTResult OnFailure(Action<Exception> action);

    /// <summary>
    /// Asynchronously executes the specified function if the result is a failure.
    /// </summary>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<ITnTResult> OnFailureAsync(Func<Exception, Task> func);

    /// <summary>
    /// Executes the specified action if the result is a success.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The current TnTResult instance.</returns>
    ITnTResult OnSuccess(Action action);

    /// <summary>
    /// Asynchronously executes the specified function if the result is a success.
    /// </summary>
    /// <param name="func">The function to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<ITnTResult> OnSuccessAsync(Func<Task> func);
}

/// <summary>
/// Represents the result of an operation with a specific success value.
/// </summary>
/// <typeparam name="TSuccess">The type of the success value.</typeparam>
public interface ITnTResult<TSuccess> : ITnTResult {

    /// <summary>
    /// Gets the success value if the operation was successful.
    /// </summary>
    TSuccess? Value { get; }

    /// <summary>
    /// Creates a failure result with the specified exception.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <returns>A failure TnTResult instance.</returns>
    static new ITnTResult<TSuccess> Failure(Exception ex) {
        return new TnTResult<TSuccess>(ex);
    }

    /// <summary>
    /// Creates a failure result with the specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A failure TnTResult instance.</returns>
    static new ITnTResult<TSuccess> Failure(string message) {
        return new TnTResult<TSuccess>(new Exception(message));
    }

    /// <summary>
    /// Creates a success result with the specified value.
    /// </summary>
    /// <param name="value">The success value.</param>
    /// <returns>A success TnTResult instance.</returns>
    static ITnTResult<TSuccess> Success(TSuccess? value) {
        return new TnTResult<TSuccess>(value);
    }

    /// <summary>
    /// Executes the specified action if the operation was successful.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    ITnTResult<TSuccess> OnSuccess(Action<TSuccess?> action);

    /// <summary>
    /// Executes the specified asynchronous function if the operation was successful.
    /// </summary>
    /// <param name="func">The asynchronous function to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task<ITnTResult<TSuccess>> OnSuccessAsync(Func<TSuccess?, Task> func);
}