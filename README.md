
<p align="center">
  <img src="Logo.svg" alt="TnTResult Logo" width="128" height="128" />
</p>

# TnTResult Solution

TnTResult is a .NET solution providing a set of libraries for handling result types, error handling, and HTTP integration in C# applications. It is designed to improve code clarity, error propagation, and integration with ASP.NET Core and Refit-based APIs.

## Table of Contents
- [Projects Overview](#projects-overview)
- [Build Instructions](#build-instructions)
- [Running Tests](#running-tests)
- [Usage Examples](#usage-examples)
  - [TnTResult](#tntresult)
  - [TnTResult.AspNetCore.Http](#tntresultaspnetcorehttp)
  - [TnTResult.Refit](#tntresultrefit)
- [License](#license)

---

## Projects Overview

### 1. TnTResult
- **Purpose:**
  - Core library for result types (`Expected`, `Optional`, etc.), error handling, and utility extensions.
  - Provides a functional approach to handling success/failure and optional values.
- **Key Files:**
  - `Expected.cs`, `Optional.cs`, `TnTResult.cs`, `TnTFileDownload.cs`
  - Extensions in `Ext/`
  - Custom exceptions in `Exceptions/`

### 2. TnTResult.AspNetCore.Http
- **Purpose:**
  - Integrates TnTResult types with ASP.NET Core HTTP pipeline.
  - Provides helpers for controller responses and HTTP result mapping.
- **Key Files:**
  - `ControllerRepositoryBase.cs`, `HttpTnTResult.cs`
  - Extensions in `Ext/`

### 3. TnTResult.Refit
- **Purpose:**
  - Adds support for using TnTResult types with [Refit](https://github.com/reactiveui/refit) (a REST library for .NET).
  - Provides extension methods for working with `IApiResponse` and mapping API results.
- **Key Files:**
  - Extensions in `Ext/`

### 4. TnTResult.Tests
- **Purpose:**
  - Contains unit tests for all core and extension libraries.
  - Organized by subproject and feature.

---

## Build Instructions

1. **Prerequisites:**
   - [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or newer
   - (Optional) [Visual Studio 2022+](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

2. **Restore dependencies:**
   ```powershell
   dotnet restore TnTResult.sln
   ```

3. **Build the solution:**
   ```powershell
   dotnet build TnTResult.sln --configuration Release
   ```

---

## Running Tests

All tests are located in the `TnTResult.Tests` project. To run all tests:

```powershell
dotnet test TnTResult.sln
```

---

## Usage Examples

### TnTResult

```csharp
using TnTResult;

// --- Expected usage ---
var ok = Expected.MakeExpected<int, string>(42);
if (ok.HasValue)
{
    Console.WriteLine($"Value: {ok.Value}");
}
else
{
    Console.WriteLine($"Error: {ok.Error}");
}

var err = Expected.MakeUnexpected<int, string>("Something went wrong");
if (!err.HasValue)
{
    Console.WriteLine($"Error: {err.Error}");
}

// --- Optional usage ---
var some = Optional.MakeOptional("hello");
if (some.HasValue)
{
    Console.WriteLine($"Optional value: {some.Value}");
}

var none = Optional.NullOpt<string>();
if (none.IsEmpty)
{
    Console.WriteLine("Optional is empty");
}

// --- ITnTResult usage ---
ITnTResult result = TnTResult.Successful;
result
    .OnSuccess(() => Console.WriteLine("Operation succeeded!"))
    .OnFailure(ex => Console.WriteLine($"Operation failed: {ex.Message}"))
    .Finally(() => Console.WriteLine("Operation finished (success or failure)"));

// ITnTResult<T> usage
ITnTResult<string> result2 = TnTResult.Success("Hello");
result2
    .OnSuccess(val => Console.WriteLine($"Success value: {val}"))
    .OnFailure(ex => Console.WriteLine($"Failed: {ex.Message}"))
    .Finally(() => Console.WriteLine("Done!"));

// --- Async usage with extension methods ---
using TnTResult.Ext;
await Task.FromResult(TnTResult.Successful)
    .OnSuccessAsync(() => Console.WriteLine("Async success!"));

await Task.FromResult(TnTResult.Failure(new Exception("fail")))
    .OnFailureAsync(ex => Console.WriteLine($"Async failure: {ex.Message}"));
```

### TnTResult.AspNetCore.Http

```csharp
using TnTResult.AspNetCore.Http;
using TnTResult;
using Microsoft.AspNetCore.Mvc;

// Inherit from TnTResultControllerBase to use static helpers
// Return ITnTResult from your actions—the base class will automatically convert it to an appropriate IResult for ASP.NET Core.
public class MyController : TnTResultControllerBase
{
    [HttpGet("/created")]
    public ITnTResult CreatedExample()
    {
        // Use the static SuccessfullyCreated property
        return SuccessfullyCreated;
    }

    [HttpGet("/forbidden")]
    public ITnTResult ForbiddenExample()
    {
        // Use the static FailureForbidden property
        return FailureForbidden;
    }

    [HttpGet("/conflict")]
    public ITnTResult ConflictExample()
    {
        // Use the static Conflict helper
        return Conflict("A conflict occurred");
    }

    [HttpGet("/custom-created")]
    public ITnTResult CustomCreatedExample()
    {
        // Use the static Created<T> helper for a custom value
        return Created("Resource created!");
    }
}

// Note: TnTResultControllerBase will automatically convert any ITnTResult returned from your action
// into the correct ASP.NET Core IResult (status code, body, etc.) for the HTTP response.
```

### TnTResult.Refit

```csharp
using TnTResult.Refit.Ext;
using Refit;

public interface IMyApi
{
    [Get("/data")]
    Task<IApiResponse<string>> GetDataAsync();
}

// Usage in your code:
var apiResponse = await myApi.GetDataAsync();
var result = apiResponse.ToTnTResult();
if (result.IsSuccess)
{
    Console.WriteLine($"API value: {result.Value}");
}
else
{
    Console.WriteLine($"API error: {result.Error.Message}");
}
```

---

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
