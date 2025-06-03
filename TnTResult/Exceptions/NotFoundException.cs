using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TnTResult.Exceptions;

/// <summary>
///     Represents an exception that is thrown when a requested entity is not found.
/// </summary>
public class NotFoundException : Exception {

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotFoundException" /> class.
    /// </summary>
    public NotFoundException() {
    }

    /// <inheritdoc />
    public NotFoundException(string? message) : base(message) {
    }

    /// <inheritdoc />
    public NotFoundException(string? message, Exception? innerException) : base(message, innerException) {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NotFoundException" /> class with the specified entity type and key.
    /// </summary>
    /// <param name="entityType">The type of the entity that was not found.</param>
    /// <param name="key">       The key of the entity that was not found.</param>
    public NotFoundException(Type entityType, object key) : base($"Failed to find {entityType.Name} with primary key {key}") {
    }
}