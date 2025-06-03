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
///     Represents a result that can either be a value of type <typeparamref name="T" /> or an error of type <typeparamref name="ErrorType" />.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <typeparam name="ErrorType">The type of the error.</typeparam>
public readonly struct Expected<T, ErrorType> {

    /// <summary>
    ///     Gets the error if the result is an error.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the result is a value.</exception>
    public readonly ErrorType Error => !_hasValue ? _errorValue! : throw new InvalidOperationException("Attempted to access error, but the result contains a value");

    /// <summary>
    ///     Gets a value indicating whether the result is a value.
    /// </summary>
    public readonly bool HasValue => _hasValue;

    /// <summary>
    ///     Gets the value if the result is a value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the result is an error.</exception>
    public readonly T Value => _hasValue ? _value! : throw new InvalidOperationException("Attempted to access the expected value, but the result contains an error");

    private readonly ErrorType? _errorValue;
    private readonly bool _hasValue;
    private readonly T? _value;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Expected{T, ErrorType}" /> struct with a value.
    /// </summary>
    /// <param name="value">The value to be held.</param>
    /// <exception cref="InvalidOperationException">Thrown if <typeparamref name="T" /> and <typeparamref name="ErrorType" /> are convertible from one another.</exception>
    internal Expected(T value) {
        ValidateTypesThrow();
        _hasValue = true;
        _value = value;
        _errorValue = default;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Expected{T, ErrorType}" /> struct with an error.
    /// </summary>
    /// <param name="error">The error to be held.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="error" /> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if <typeparamref name="T" /> and <typeparamref name="ErrorType" /> are inherited from one another.</exception>
    internal Expected(ErrorType error) {
        ArgumentNullException.ThrowIfNull(error, nameof(error));
        ValidateTypesThrow();
        _hasValue = false;
        _value = default;
        _errorValue = error;
    }

    public static implicit operator Expected<T, ErrorType>(T value) => Expected.MakeExpected<T, ErrorType>(value);

    public static implicit operator Expected<T, ErrorType>(ErrorType value) => Expected.MakeUnexpected<T, ErrorType>(value);

    /// <summary>
    ///     Determines whether two Expected instances are not equal.
    /// </summary>
    public static bool operator !=(Expected<T, ErrorType> left, Expected<T, ErrorType> right) => !left.Equals(right);

    /// <summary>
    ///     Determines whether two Expected instances are equal.
    /// </summary>
    public static bool operator ==(Expected<T, ErrorType> left, Expected<T, ErrorType> right) => left.Equals(right);

    /// <summary>
    ///     Applies a function to the value if the result is a value, otherwise returns the current error.
    /// </summary>
    /// <typeparam name="OutT">The type of the result of the function.</typeparam>
    /// <param name="func">The function to apply to the value.</param>
    /// <returns>A new <see cref="Expected{OutT, ErrorType}" /> with the result of the function or the current error.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Expected<OutT, ErrorType> AndThen<OutT>(Func<T, OutT> func) {
        return _hasValue
            ? Expected.MakeExpected<OutT, ErrorType>(func(_value!))
            : Expected.MakeUnexpected<OutT, ErrorType>(_errorValue!);
    }

    /// <summary>
    ///     Determines whether two Expected instances are equal.
    /// </summary>
    public bool Equals(Expected<T, ErrorType> other) {
        if (_hasValue != other._hasValue)
            return false;

        return _hasValue
            ? EqualityComparer<T>.Default.Equals(_value, other._value)
            : EqualityComparer<ErrorType>.Default.Equals(_errorValue, other._errorValue);
    }

    /// <summary>
    ///     Determines whether this instance and a specified object are equal.
    /// </summary>
    public override bool Equals(object? obj) => obj is Expected<T, ErrorType> other && Equals(other);

    /// <summary>
    ///     Returns the hash code for this instance.
    /// </summary>
    public override int GetHashCode() {
        return _hasValue
            ? HashCode.Combine(true, _value)
            : HashCode.Combine(false, _errorValue);
    }

    /// <summary>
    ///     Applies a function to the error if the result is an error, otherwise returns the current value.
    /// </summary>
    /// <typeparam name="OutErrorType">The type of the result of the function.</typeparam>
    /// <param name="func">The function to apply to the error.</param>
    /// <returns>A new <see cref="Expected{T, OutErrorType}" /> with the result of the function or the current value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Expected<T, OutErrorType> OrElse<OutErrorType>(Func<ErrorType, OutErrorType> func) {
        return !_hasValue
            ? Expected.MakeUnexpected<T, OutErrorType>(func(_errorValue!))
            : Expected.MakeExpected<T, OutErrorType>(_value!);
    }

    /// <summary>
    ///     Returns a string representation of this instance.
    /// </summary>
    public override string ToString() {
        return _hasValue
            ? $"Expected[Value: {_value}]"
            : $"Expected[Error: {_errorValue}]";
    }

    /// <summary>
    ///     Transforms the value using a function if the result is a value, otherwise returns the current error.
    /// </summary>
    /// <typeparam name="OutT">The type of the result of the function.</typeparam>
    /// <param name="func">The function to transform the value.</param>
    /// <returns>A new <see cref="Expected{OutT, ErrorType}" /> with the transformed value or the current error.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Expected<OutT, ErrorType> Transform<OutT>(Func<T, OutT> func) {
        return _hasValue
            ? Expected.MakeExpected<OutT, ErrorType>(func(_value!))
            : Expected.MakeUnexpected<OutT, ErrorType>(_errorValue!);
    }

    /// <summary>
    ///     Transforms the error using a function if the result is an error, otherwise returns the current value.
    /// </summary>
    /// <typeparam name="OutErrorType">The type of the result of the function.</typeparam>
    /// <param name="func">The function to transform the error.</param>
    /// <returns>A new <see cref="Expected{T, OutErrorType}" /> with the transformed error or the current value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Expected<T, OutErrorType> TransformError<OutErrorType>(Func<ErrorType, OutErrorType> func) {
        return !_hasValue
            ? Expected.MakeUnexpected<T, OutErrorType>(func(_errorValue!))
            : Expected.MakeExpected<T, OutErrorType>(_value!);
    }

    /// <summary>
    ///     Gets the value if the result is a value, otherwise returns the specified default value.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the result is an error.</param>
    /// <returns>The value if the result is a value, otherwise the specified default value.</returns>
    public T ValueOr(T defaultValue) => _hasValue ? _value! : defaultValue;

    /// <summary>
    ///     Gets the value if the result is a value, otherwise returns the result of the specified function.
    /// </summary>
    /// <param name="defaultValueFactory">The function to call if the result is an error.</param>
    /// <returns>The value if the result is a value, otherwise the result of the function.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ValueOr(Func<T> defaultValueFactory) => _hasValue ? _value! : defaultValueFactory();

    /// <summary>
    ///     Gets the value if the result is a value, otherwise returns the result of the specified function that takes the error.
    /// </summary>
    /// <param name="defaultValueFactory">The function to call with the error if the result is an error.</param>
    /// <returns>The value if the result is a value, otherwise the result of the function.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T ValueOr(Func<ErrorType, T> defaultValueFactory) => _hasValue ? _value! : defaultValueFactory(_errorValue!);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowInvalidTypesException() {
        throw new InvalidOperationException($"{typeof(T).Name} and {typeof(ErrorType).Name} cannot be convertible from one another");
    }

    /// <summary>
    ///     Validate that <typeparamref name="T" /> and <typeparamref name="ErrorType" /> are not assignable to each other.
    /// </summary>
    /// <returns>True if types are not convertible to one another</returns>
    private static bool ValidateTypes() => !TypeValidationCache.AreTypesConvertible;

    /// <summary>
    ///     Validates types and throws if they are invalid.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if <typeparamref name="T" /> and <typeparamref name="ErrorType" /> are convertible from one another.</exception>
    private static void ValidateTypesThrow() {
        if (!ValidateTypes()) {
            ThrowInvalidTypesException();
        }
    }

    /// <summary>
    ///     Cached type validation to avoid repeated reflection calls.
    /// </summary>
    private static class TypeValidationCache {

        public static readonly bool AreTypesConvertible =
            typeof(T).IsAssignableTo(typeof(ErrorType)) || typeof(ErrorType).IsAssignableTo(typeof(T));
    }
}