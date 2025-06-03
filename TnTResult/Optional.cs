using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TnTResult;

/// <summary>
///     Provides static methods for creating optional values.
/// </summary>
public static class Optional {

    /// <summary>
    ///     Creates an instance of <see cref="Optional{OptType}" /> with the specified value.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <param name="optional">The value of the optional.</param>
    /// <returns>An instance of <see cref="Optional{OptType}" />.</returns>
    public static Optional<OptType> MakeOptional<OptType>(OptType optional) => Optional<OptType>.MakeOptional(optional);

    /// <summary>
    ///     Creates an empty instance of <see cref="Optional{OptType}" />.
    /// </summary>
    /// <typeparam name="OptType">The type of the optional value.</typeparam>
    /// <returns>An empty instance of <see cref="Optional{OptType}" />.</returns>
    public static Optional<OptType> NullOpt<OptType>() => Optional<OptType>.NullOpt;
}

/// <summary>
///     Represents an optional value that may or may not have a value.
/// </summary>
/// <typeparam name="OptType">The type of the optional value.</typeparam>
public readonly struct Optional<OptType> {

    /// <summary>
    ///     Gets an instance of <see cref="Optional{OptType}" /> that represents an empty optional.
    /// </summary>
    public static Optional<OptType> NullOpt => new();

    /// <summary>
    ///     Gets a value indicating whether this optional has a value.
    /// </summary>
    public readonly bool HasValue => _value is not null;

    /// <summary>
    ///     Gets a value indicating whether this optional is empty.
    /// </summary>
    public readonly bool IsEmpty => !HasValue;

    /// <summary>
    ///     Gets the value of the optional. Throws an <see cref="InvalidOperationException" /> if the optional is empty.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the optional is empty.</exception>
    public readonly OptType Value => _value ?? throw new InvalidOperationException("Attempted to obtain the value of an optional, but this optional is empty");

    private readonly OptType? _value;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Optional{OptType}" /> struct with the specified value.
    /// </summary>
    /// <param name="value">The value of the optional.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value" /> is null.</exception>
    private Optional(OptType value) {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        _value = value;
    }

    /// <summary>
    ///     Implicitly converts a value to an optional.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Optional<OptType>(OptType value) => value is not null ? new(value) : NullOpt;

    /// <summary>
    ///     Creates an instance of <see cref="Optional{OptType}" /> with the specified value.
    /// </summary>
    /// <param name="optional">The value of the optional.</param>
    /// <returns>An instance of <see cref="Optional{OptType}" />.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="optional" /> is null.</exception>
    public static Optional<OptType> MakeOptional(OptType optional) => new(optional);

    /// <summary>
    ///     Performs an action on the value of the optional if it has a value.
    /// </summary>
    /// <param name="action">The action to perform on the value.</param>
    /// <returns>The current instance of <see cref="Optional{OptType}" />.</returns>
    public Optional<OptType> AndThen(Action<OptType> action) {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        if (HasValue) {
            action(Value);
        }
        return this;
    }

    /// <summary>
    ///     Performs an async action on the value of the optional if it has a value.
    /// </summary>
    /// <param name="asyncAction">The async action to perform on the value.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<Optional<OptType>> AndThenAsync(Func<OptType, Task> asyncAction) {
        ArgumentNullException.ThrowIfNull(asyncAction, nameof(asyncAction));

        if (HasValue) {
            await asyncAction(Value).ConfigureAwait(false);
        }
        return this;
    }

    /// <summary>
    ///     Performs an action if the optional does not have a value.
    /// </summary>
    /// <param name="action">The action to perform.</param>
    /// <returns>The current instance of <see cref="Optional{OptType}" />.</returns>
    public Optional<OptType> OrElse(Action action) {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        if (!HasValue) {
            action();
        }
        return this;
    }

    /// <summary>
    ///     Performs an async action if the optional does not have a value.
    /// </summary>
    /// <param name="asyncAction">The async action to perform.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<Optional<OptType>> OrElseAsync(Func<Task> asyncAction) {
        ArgumentNullException.ThrowIfNull(asyncAction, nameof(asyncAction));

        if (!HasValue) {
            await asyncAction().ConfigureAwait(false);
        }
        return this;
    }

    /// <summary>
    ///     Returns a string representation of the optional.
    /// </summary>
    /// <returns>A string representation of the optional.</returns>
    public override string ToString() => HasValue ? $"Optional({Value})" : "Optional(Empty)";

    /// <summary>
    ///     Transforms the value of the optional using the specified function.
    /// </summary>
    /// <typeparam name="NewOptType">The type of the transformed optional value.</typeparam>
    /// <param name="func">The function to transform the value.</param>
    /// <returns>An instance of <see cref="Optional{NewOptType}" /> with the transformed value, or an empty optional if the transformation result is null or the original optional is empty.</returns>
    public Optional<NewOptType> Transform<NewOptType>(Func<OptType, NewOptType?> func) {
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        if (HasValue) {
            var newValue = func(Value);
            if (newValue is not null) {
                return new Optional<NewOptType>(newValue);
            }
        }
        return Optional<NewOptType>.NullOpt;
    }

    /// <summary>
    ///     Asynchronously transforms the value of the optional using the specified function.
    /// </summary>
    /// <typeparam name="NewOptType">The type of the transformed optional value.</typeparam>
    /// <param name="asyncFunc">The async function to transform the value.</param>
    /// <returns>
    ///     A task containing an instance of <see cref="Optional{NewOptType}" /> with the transformed value, or an empty optional if the transformation result is null or the original optional is empty.
    /// </returns>
    public async Task<Optional<NewOptType>> TransformAsync<NewOptType>(Func<OptType, Task<NewOptType?>> asyncFunc) {
        ArgumentNullException.ThrowIfNull(asyncFunc, nameof(asyncFunc));

        if (HasValue) {
            var newValue = await asyncFunc(Value).ConfigureAwait(false);
            if (newValue is not null) {
                return new Optional<NewOptType>(newValue);
            }
        }
        return Optional<NewOptType>.NullOpt;
    }

    /// <summary>
    ///     Tries to get the value of the optional.
    /// </summary>
    /// <param name="value">The value of the optional, if it has a value; otherwise, default.</param>
    /// <returns>True if the optional has a value; otherwise, false.</returns>
    public bool TryGetValue([MaybeNullWhen(false)] out OptType? value) {
        value = _value;
        return HasValue;
    }

    /// <summary>
    ///     Gets the value of the optional, or a default value if the optional is empty.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the optional is empty.</param>
    /// <returns>The value of the optional, or the default value if the optional is empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="defaultValue" /> is null.</exception>
    public OptType ValueOr(OptType defaultValue) {
        ArgumentNullException.ThrowIfNull(defaultValue, nameof(defaultValue));
        return HasValue ? Value : defaultValue;
    }
}