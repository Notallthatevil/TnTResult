using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TnTResult;

/// <summary>
/// Represents an expected value that can either hold a value of type <typeparamref name="T" /> or
/// an error of type <typeparamref name="ErrorType" />.
/// </summary>
/// <typeparam name="T">The type of the expected value.</typeparam>
/// <typeparam name="ErrorType">The type of the error.</typeparam>
public readonly struct Expected<T, ErrorType> {

    /// <summary>
    /// Gets the error value if the expected value holds an error.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The held type is not <typeparamref name="ErrorType" />.
    /// </exception>
    public readonly ErrorType Error {
        get {
            if (_heldType == typeof(ErrorType) || _heldType.IsAssignableTo(typeof(ErrorType))) {
                return (ErrorType)_value!;
            }
            else {
                throw new InvalidOperationException($"Attempted to access error, but the held type is {typeof(T).Name}");
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the expected value holds a value of type <typeparamref
    /// name="T" />.
    /// </summary>
    public readonly bool HasValue => _heldType == typeof(T);

    /// <summary>
    /// Gets the value if the expected value holds a value of type <typeparamref name="T" />.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The held type is not <typeparamref name="T" />.
    /// </exception>
    public readonly T? Value {
        get {
            if (_heldType == typeof(T)) {
                return (T?)_value;
            }
            else {
                throw new InvalidOperationException($"Attempted to access value, but the held type is {typeof(ErrorType).Name}");
            }
        }
    }

    private readonly Type _heldType;
    private readonly object? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Expected{T, ErrorType}" /> class with a value
    /// of type <typeparamref name="T" />.
    /// </summary>
    /// <param name="value">The value of type <typeparamref name="T" />.</param>
    /// <exception cref="InvalidOperationException">
    /// <typeparamref name="T" /> and <typeparamref name="ErrorType" /> cannot be inherited from one another.
    /// </exception>
    private Expected(T? value) {
        if (!ValidateTypes()) {
            throw new InvalidOperationException($"{typeof(T).Name} and {typeof(ErrorType).Name} cannot be inherited from one another");
        }
        _heldType = typeof(T);
        _value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Expected{T, ErrorType}" /> class with an error
    /// of type <typeparamref name="ErrorType" />.
    /// </summary>
    /// <param name="error">The error of type <typeparamref name="ErrorType" />.</param>
    /// <exception cref="InvalidOperationException">
    /// <typeparamref name="T" /> and <typeparamref name="ErrorType" /> cannot be inherited from one another.
    /// </exception>
    private Expected(ErrorType error) {
        ArgumentNullException.ThrowIfNull(error, nameof(error));
        if (!ValidateTypes()) {
            throw new InvalidOperationException($"{typeof(T).Name} and {typeof(ErrorType).Name} cannot be inherited from one another");
        }
        _heldType = typeof(ErrorType);
        _value = error;
    }

    /// <summary>
    /// Creates an instance of <see cref="Expected{T, ErrorType}" /> with a value of type
    /// <typeparamref name="T" />.
    /// </summary>
    /// <param name="t">The value of type <typeparamref name="T" />.</param>
    /// <returns>
    /// An instance of <see cref="Expected{T, ErrorType}" /> holding the value of type <typeparamref
    /// name="T" />.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// <typeparamref name="T" /> and <typeparamref name="ErrorType" /> cannot be inherited from one another.
    /// </exception>
    public static Expected<T, ErrorType> MakeExpected(T? t) {
        return new Expected<T, ErrorType>(t);
    }

    /// <summary>
    /// Creates an instance of <see cref="Expected{T, ErrorType}" /> with an error of type
    /// <typeparamref name="ErrorType" />.
    /// </summary>
    /// <param name="error">The error of type <typeparamref name="ErrorType" />.</param>
    /// <returns>
    /// An instance of <see cref="Expected{T, ErrorType}" /> holding the error of type <typeparamref
    /// name="ErrorType" />.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// <typeparamref name="T" /> and <typeparamref name="ErrorType" /> cannot be inherited from one another.
    /// </exception>
    public static Expected<T, ErrorType> MakeUnexpected(ErrorType error) {
        return new Expected<T, ErrorType>(error);
    }

    /// <summary>
    /// Performs an action on the value if the expected value holds a value of type <typeparamref
    /// name="T" />.
    /// </summary>
    /// <param name="action">The action to perform on the value.</param>
    /// <returns>The current instance of <see cref="Expected{T, ErrorType}" />.</returns>
    public Expected<T, ErrorType> AndThen(Action<T?> action) {
        return AndThenAsync((value) => {
            action(value);
            return Task.CompletedTask;
        }).Result;
    }

    /// <summary>
    /// Performs an asynchronous action on the value if the expected value holds a value of type
    /// <typeparamref name="T" />.
    /// </summary>
    /// <param name="func">The asynchronous action to perform on the value.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns the current instance of <see
    /// cref="Expected{T, ErrorType}" />.
    /// </returns>
    public async Task<Expected<T, ErrorType>> AndThenAsync(Func<T?, Task> func) {
        if (HasValue) {
            await func(Value);
        }
        return this;
    }

    /// <summary>
    /// Performs an action on the error if the expected value holds an error.
    /// </summary>
    /// <param name="action">The action to perform on the error.</param>
    /// <returns>The current instance of <see cref="Expected{T, ErrorType}" />.</returns>
    public Expected<T, ErrorType> OrElse(Action<ErrorType> action) {
        return OrElseAsync((error) => {
            action(error);
            return Task.CompletedTask;
        }).Result;
    }

    /// <summary>
    /// Performs an asynchronous action on the error if the expected value holds an error.
    /// </summary>
    /// <param name="func">The asynchronous action to perform on the error.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns the current instance of <see
    /// cref="Expected{T, ErrorType}" />.
    /// </returns>
    public async Task<Expected<T, ErrorType>> OrElseAsync(Func<ErrorType, Task> func) {
        if (!HasValue) {
            await func(Error);
        }
        return this;
    }

    /// <summary>
    /// Transforms the value of type <typeparamref name="T" /> to a new value of type <typeparamref
    /// name="U" /> if the expected value holds a value of type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="U">The type of the transformed value.</typeparam>
    /// <param name="func">The function to transform the value.</param>
    /// <returns>
    /// An instance of <see cref="Expected{U, ErrorType}" /> holding the transformed value of type
    /// <typeparamref name="U" /> or the error.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// <typeparamref name="U" /> and <typeparamref name="ErrorType" /> cannot be inherited from one another.
    /// </exception>
    public Expected<U, ErrorType> Transform<U>(Func<T?, U> func) {
        return TransformAsync((value) => Task.FromResult(func(value))).Result;
    }

    /// <summary>
    /// Transforms the value of type <typeparamref name="T" /> to a new value of type <typeparamref
    /// name="U" /> if the expected value holds a value of type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="U">The type of the transformed value.</typeparam>
    /// <param name="func">The asynchronous function to transform the value.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns an instance of <see
    /// cref="Expected{U, ErrorType}" /> holding the transformed value of type <typeparamref
    /// name="U" /> or the error.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// <typeparamref name="U" /> and <typeparamref name="ErrorType" /> cannot be inherited from one another.
    /// </exception>
    public async Task<Expected<U, ErrorType>> TransformAsync<U>(Func<T?, Task<U>> func) {
        if (HasValue) {
            var newValue = await func(Value);
            return new Expected<U, ErrorType>(newValue);
        }
        else {
            return new Expected<U, ErrorType>(Error);
        }
    }

    /// <summary>
    /// Transforms the error of type <typeparamref name="ErrorType" /> to a new error of type
    /// <typeparamref name="E" /> if the expected value holds an error.
    /// </summary>
    /// <typeparam name="E">The type of the transformed error.</typeparam>
    /// <param name="func">The function to transform the error.</param>
    /// <returns>
    /// An instance of <see cref="Expected{T, E}" /> holding the value or the transformed error of
    /// type <typeparamref name="E" />.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// <typeparamref name="T" /> and <typeparamref name="E" /> cannot be inherited from one another.
    /// </exception>
    public Expected<T, E> TransformError<E>(Func<ErrorType, E> func) {
        return TransformErrorAsync((error) => Task.FromResult(func(error))).Result;
    }

    /// <summary>
    /// Transforms the error of type <typeparamref name="ErrorType" /> to a new error of type
    /// <typeparamref name="E" /> if the expected value holds an error.
    /// </summary>
    /// <typeparam name="E">The type of the transformed error.</typeparam>
    /// <param name="func">The asynchronous function to transform the error.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns an instance of <see
    /// cref="Expected{T, E}" /> holding the value or the transformed error of type <typeparamref
    /// name="E" />.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// <typeparamref name="T" /> and <typeparamref name="E" /> cannot be inherited from one another.
    /// </exception>
    public async Task<Expected<T, E>> TransformErrorAsync<E>(Func<ErrorType, Task<E>> func) {
        if (!HasValue) {
            var newError = await func(Error);
            return new Expected<T, E>(newError);
        }
        else {
            return new Expected<T, E>(Value);
        }
    }

    /// <summary>
    /// Gets the value if the expected value holds a value of type <typeparamref name="T" />;
    /// otherwise, returns the specified default value.
    /// </summary>
    /// <param name="defaultValue">
    /// The default value to return if the expected value does not hold a value of type
    /// <typeparamref name="T" />.
    /// </param>
    /// <returns>
    /// The value of type <typeparamref name="T" /> if the expected value holds a value of type
    /// <typeparamref name="T" />; otherwise, the specified default value.
    /// </returns>
    public T? ValueOr(T? defaultValue = default) {
        if (HasValue) {
            return Value;
        }
        else {
            return defaultValue;
        }
    }

    private static bool ValidateTypes() {
        return !(typeof(T).IsAssignableTo(typeof(ErrorType)) || typeof(ErrorType).IsAssignableTo(typeof(T)));
    }
}