using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TnTResult.Exceptions;
public class NotFoundException : System.Exception {
    public NotFoundException() : base() {
    }

    public NotFoundException(string? message) : base(message) {
    }

    public NotFoundException(string? message, System.Exception? innerException) : base(message, innerException) {
    }

    public NotFoundException(Type entityType, object key) : base($"Failed to find {entityType.Name} with primary key {key}") {
    }
}

