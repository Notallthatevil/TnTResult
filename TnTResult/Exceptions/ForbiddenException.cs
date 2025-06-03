using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TnTResult.Exceptions;

/// <summary>
///     Represents an exception that is thrown when a forbidden action is attempted.
/// </summary>
public class ForbiddenException : Exception {

    /// <inheritdoc />
    public ForbiddenException(string? message) : base(message) {
    }

    /// <inheritdoc />
    public ForbiddenException(string? message, Exception? innerException) : base(message, innerException) {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ForbiddenException" /> class.
    /// </summary>
    public ForbiddenException() {
    }
}