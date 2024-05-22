using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TnTResult;

/// <summary>
/// Represents an optional value that may or may not have a value.
/// </summary>
/// <typeparam name="OptType">The type of the optional value.</typeparam>
public readonly struct Optional<OptType> {
    /// <summary>
    /// Gets an instance of <see cref="Optional{OptType}"/> that represents an empty optional.
    /// </summary>
    public static Optional<OptType> NullOpt => new();

    /// <summary>
    /// Gets a value indicating whether this optional has a value.
    /// </summary>
    public readonly bool HasValue => _value is not null;

    /// <summary>
    /// Gets the value of the optional. Throws an <see cref="InvalidOperationException"/> if the optional is empty.
    /// </summary>
    public readonly OptType Value => _value ?? throw new InvalidOperationException("Attempted to obtain the value of an optional, but this optional is empty");

    private readonly OptType? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Optional{OptType}"/> struct with the specified value.
    /// </summary>
    /// <param name="value">The value of the optional.</param>
    private Optional(OptType value) {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        _value = value;
    }

    /// <summary>
    /// Creates an instance of <see cref="Optional{OptType}"/> with the specified value.
    /// </summary>
    /// <param name="optional">The value of the optional.</param>
    /// <returns>An instance of <see cref="Optional{OptType}"/>.</returns>
    public static Optional<OptType> MakeOptional(OptType optional) => new(optional);

    /// <summary>
    /// Performs an action on the value of the optional if it has a value.
    /// </summary>
    /// <param name="action">The action to perform on the value.</param>
    /// <returns>The current instance of <see cref="Optional{OptType}"/>.</returns>
    public Optional<OptType> AndThen(Action<OptType> action) {
        if (HasValue) {
            action(Value);
        }
        return this;
    }

    /// <summary>
    /// Performs an action if the optional does not have a value.
    /// </summary>
    /// <param name="action">The action to perform.</param>
    /// <returns>The current instance of <see cref="Optional{OptType}"/>.</returns>
    public Optional<OptType> OrElse(Action action) {
        if (!HasValue) {
            action();
        }
        return this;
    }

    /// <summary>
    /// Transforms the value of the optional using the specified function.
    /// </summary>
    /// <typeparam name="NewOptType">The type of the transformed optional value.</typeparam>
    /// <param name="func">The function to transform the value.</param>
    /// <returns>An instance of <see cref="Optional{NewOptType}"/> with the transformed value, or an empty optional if the transformation result is null.</returns>
    public Optional<NewOptType> Transform<NewOptType>(Func<OptType, NewOptType?> func) {
        if (HasValue) {
            var newValue = func(Value);
            if (newValue is not null) {
                return new Optional<NewOptType>(newValue);
            }
        }
        return new Optional<NewOptType>();
    }

    /// <summary>
    /// Gets the value of the optional, or a default value if the optional is empty.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the optional is empty.</param>
    /// <returns>The value of the optional, or the default value if the optional is empty.</returns>
    public OptType ValueOr(OptType defaultValue = default!) {
        ArgumentNullException.ThrowIfNull(defaultValue, nameof(defaultValue));
        return HasValue ? Value : defaultValue;
    }
}
