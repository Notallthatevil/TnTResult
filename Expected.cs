using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TnTResult;

/// <summary>
///     Provides factory methods to create instances of <see cref="Expected{T, ErrorType}" />.
/// </summary>
public static class Expected {

    /// <summary>
    ///     Creates an instance of <see cref="Expected{T, ErrorType}" /> with a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="value">The value to be held.</param>
    /// <returns>An instance of <see cref="Expected{T, ErrorType}" /> holding the value.</returns>
    public static Expected<T, ErrorType> MakeExpected<T, ErrorType>(T value) => new(value);

    /// <summary>
    ///     Creates an instance of <see cref="Expected{T, ErrorType}" /> with an error.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="ErrorType">The type of the error.</typeparam>
    /// <param name="error">The error to be held.</param>
    /// <returns>An instance of <see cref="Expected{T, ErrorType}" /> holding the error.</returns>
    public static Expected<T, ErrorType> MakeUnexpected<T, ErrorType>(ErrorType error) => new(error);
}

/// <summary>
///     Represents a result that can either be a value of type <typeparamref name="T" /> or an error
///     of type <typeparamref name="ErrorType" />.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <typeparam name="ErrorType">The type of the error.</typeparam>
public readonly struct Expected<T, ErrorType> {

    /// <summary>
    ///     Gets the error if the result is an error.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the result is a value.</exception>
    public readonly ErrorType Error => !HasValue ? (ErrorType)_value! : throw new InvalidOperationException($"Attempted to access error, but the held type is {_heldType.Name}");

    /// <summary>
    ///     Gets a value indicating whether the result is a value.
    /// </summary>
    public readonly bool HasValue => _heldType.IsAssignableTo(typeof(T));

    /// <summary>
    ///     Gets the value if the result is a value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the result is an error.</exception>
    public readonly T Value => HasValue ? (T)_value! : throw new InvalidOperationException($"Attempted to access the expected value, but the held type is {_heldType.Name}");

    private readonly Type _heldType;
    private readonly object? _value;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Expected{T, ErrorType}" /> struct with a value.
    /// </summary>
    /// <param name="value">The value to be held.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if <typeparamref name="T" /> and <typeparamref name="ErrorType" /> are
    ///     convertible from one another.
    /// </exception>
    internal Expected(T value) {
        if (!ValidateTypes()) {
            throw new InvalidOperationException($"{typeof(T).Name} and {typeof(ErrorType).Name} cannot be convertible from one another");
        }
        _heldType = typeof(T);
        _value = value;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Expected{T, ErrorType}" /> struct with an error.
    /// </summary>
    /// <param name="error">The error to be held.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="error" /> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if <typeparamref name="T" /> and <typeparamref name="ErrorType" /> are inherited
    ///     from one another.
    /// </exception>
    internal Expected(ErrorType error) {
        ArgumentNullException.ThrowIfNull(error, nameof(error));
        if (!ValidateTypes()) {
            throw new InvalidOperationException($"{typeof(T).Name} and {typeof(ErrorType).Name} cannot be inherited from one another");
        }
        _heldType = typeof(ErrorType);
        _value = error;
    }

    /// <summary>
    ///     Applies a function to the value if the result is a value, otherwise returns the current error.
    /// </summary>
    /// <typeparam name="OutT">The type of the result of the function.</typeparam>
    /// <param name="func">The function to apply to the value.</param>
    /// <returns>
    ///     A new <see cref="Expected{OutT, ErrorType}" /> with the result of the function or the
    ///     current error.
    /// </returns>
    public Expected<OutT, ErrorType> AndThen<OutT>(Func<T, OutT> func) {
        if (HasValue) {
            var result = func(Value);
            return Expected.MakeExpected<OutT, ErrorType>(result);
        }
        return Expected.MakeUnexpected<OutT, ErrorType>(Error);
    }

    /// <summary>
    ///     Applies a function to the error if the result is an error, otherwise returns the current value.
    /// </summary>
    /// <typeparam name="OutErrorType">The type of the result of the function.</typeparam>
    /// <param name="func">The function to apply to the error.</param>
    /// <returns>
    ///     A new <see cref="Expected{T, OutErrorType}" /> with the result of the function or the
    ///     current value.
    /// </returns>
    public Expected<T, OutErrorType> OrElse<OutErrorType>(Func<ErrorType, OutErrorType> func) {
        if (!HasValue) {
            var result = func(Error);
            return Expected.MakeUnexpected<T, OutErrorType>(result);
        }
        return Expected.MakeExpected<T, OutErrorType>(Value);
    }

    /// <summary>
    ///     Transforms the value using a function if the result is a value, otherwise returns the
    ///     current error.
    /// </summary>
    /// <typeparam name="U">The type of the result of the function.</typeparam>
    /// <param name="func">The function to transform the value.</param>
    /// <returns>
    ///     A new <see cref="Expected{U, ErrorType}" /> with the transformed value or the current error.
    /// </returns>
    public Expected<OutT, ErrorType> Transform<OutT>(Func<T, OutT> func) {
        if (HasValue) {
            var result = func(Value);
            return Expected.MakeExpected<OutT, ErrorType>(result);
        }
        return Expected.MakeUnexpected<OutT, ErrorType>(Error);
    }

    /// <summary>
    ///     Transforms the error using a function if the result is an error, otherwise returns the
    ///     current value.
    /// </summary>
    /// <typeparam name="E">The type of the result of the function.</typeparam>
    /// <param name="func">The function to transform the error.</param>
    /// <returns>
    ///     A new <see cref="Expected{T, E}" /> with the transformed error or the current value.
    /// </returns>
    public Expected<T, OutErrorType> TransformError<OutErrorType>(Func<ErrorType, OutErrorType> func) {
        if (!HasValue) {
            var result = func(Error);
            return Expected.MakeUnexpected<T, OutErrorType>(result);
        }
        return Expected.MakeExpected<T, OutErrorType>(Value);
    }

    /// <summary>
    ///     Gets the value if the result is a value, otherwise returns the specified default value.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the result is an error.</param>
    /// <returns>The value if the result is a value, otherwise the specified default value.</returns>
    public T ValueOr(T defaultValue) => HasValue ? Value : defaultValue;

    /// <summary>
    ///     Validate that <typeparamref name="T" /> and <typeparamref name="ErrorType" /> are not
    ///     assignable to each other.
    /// </summary>
    /// <returns>True if types are not convertible to one another</returns>
    private static bool ValidateTypes() => !(typeof(T).IsAssignableTo(typeof(ErrorType)) || typeof(ErrorType).IsAssignableTo(typeof(T)));

    public static implicit operator Expected<T, ErrorType>(T value) => Expected.MakeExpected<T, ErrorType>(value);
    public static implicit operator Expected<T, ErrorType>(ErrorType value) => Expected.MakeUnexpected<T, ErrorType>(value);

}