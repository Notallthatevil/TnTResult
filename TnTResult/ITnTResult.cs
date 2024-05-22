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
}