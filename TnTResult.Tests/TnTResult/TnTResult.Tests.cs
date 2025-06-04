using System;
using Xunit;
using AwesomeAssertions;
using global::TnTResult;

namespace TnTResult.Tests.TnTResult;

public class TnTResultTests {

    [Fact]
    public void Error_WithFailedResult_ShouldReturnError() {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        var result = global::TnTResult.TnTResult.Failure<string>(exception);

        // Act & Assert
        result.Error.Should().BeSameAs(exception);
    }

    [Fact]
    public void Error_WithSuccessfulResult_ShouldThrowException() {
        // Arrange
        var result = global::TnTResult.TnTResult.Success("test");

        // Act & Assert
        var thrownException = Assert.Throws<InvalidOperationException>(() => result.Error);
        thrownException.Message.Should().Be("Attempted to access error, but the result contains a value");
    }

    [Fact]
    public void ErrorMessage_WithFailedResult_ShouldReturnErrorMessage() {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        var result = global::TnTResult.TnTResult.Failure<string>(exception);

        // Act & Assert
        result.ErrorMessage.Should().Be("Test error");
    }

    [Fact]
    public void Failure_WithException_ShouldCreateFailedResult() {
        // Arrange
        var exception = new InvalidOperationException("Test error");

        // Act
        var result = global::TnTResult.TnTResult.Failure(exception);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.HasFailed.Should().BeTrue();
        result.Error.Should().BeSameAs(exception);
        result.ErrorMessage.Should().Be("Test error");
    }

    [Fact]
    public void FailureGeneric_WithException_ShouldCreateFailedResultWithType() {
        // Arrange
        var exception = new InvalidOperationException("Test error");

        // Act
        var result = global::TnTResult.TnTResult.Failure<string>(exception);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.HasFailed.Should().BeTrue();
        result.Error.Should().BeSameAs(exception);
        result.ErrorMessage.Should().Be("Test error");
    }

    [Fact]
    public void Finally_ShouldAlwaysExecuteAction() {
        // Arrange
        var successResult = global::TnTResult.TnTResult.Successful;
        var failureResult = global::TnTResult.TnTResult.Failure(new InvalidOperationException("Test error"));
        var successExecuted = false;
        var failureExecuted = false;

        // Act
        var successReturnedResult = successResult.Finally(() => successExecuted = true);
        var failureReturnedResult = failureResult.Finally(() => failureExecuted = true);

        // Assert
        successExecuted.Should().BeTrue();
        failureExecuted.Should().BeTrue();
        successReturnedResult.Should().Be(successResult);
        failureReturnedResult.Should().Be(failureResult);
    }

    [Fact]
    public void FinallyGeneric_ShouldAlwaysExecuteAction() {
        // Arrange
        var successResult = global::TnTResult.TnTResult.Success("test");
        var failureResult = global::TnTResult.TnTResult.Failure<string>(new InvalidOperationException("Test error"));
        var successExecuted = false;
        var failureExecuted = false;

        // Act
        var successReturnedResult = successResult.Finally(() => successExecuted = true);
        var failureReturnedResult = failureResult.Finally(() => failureExecuted = true);

        // Assert
        successExecuted.Should().BeTrue();
        failureExecuted.Should().BeTrue();
        successReturnedResult.Should().Be(successResult);
        failureReturnedResult.Should().Be(failureResult);
    }

    [Fact]
    public void GenericResultAsNonGeneric_ShouldWorkThroughInterface() {
        // Arrange
        ITnTResult result = global::TnTResult.TnTResult.Success("test");
        var executed = false;

        // Act
        result.OnSuccess(() => executed = true);

        // Assert
        executed.Should().BeTrue();
        result.IsSuccessful.Should().BeTrue();
        result.HasFailed.Should().BeFalse();
    }

    [Fact]
    public void ITnTResultGenericMethods_ShouldWorkThroughInterface() {
        // Arrange
        ITnTResult<string> successResult = global::TnTResult.TnTResult.Success("test");
        ITnTResult<string> failureResult = global::TnTResult.TnTResult.Failure<string>(new InvalidOperationException("Test error"));
        var successExecuted = false;
        var failureExecuted = false;

        // Act
        successResult.OnSuccess(value => successExecuted = true);
        failureResult.OnFailure(error => failureExecuted = true);

        // Assert
        successExecuted.Should().BeTrue();
        failureExecuted.Should().BeTrue();
    }

    [Fact]
    public void ITnTResultMethods_ShouldWorkThroughInterface() {
        // Arrange
        ITnTResult successResult = global::TnTResult.TnTResult.Successful;
        ITnTResult failureResult = global::TnTResult.TnTResult.Failure(new InvalidOperationException("Test error"));
        var successExecuted = false;
        var failureExecuted = false;

        // Act
        successResult.OnSuccess(() => successExecuted = true);
        failureResult.OnFailure(error => failureExecuted = true);

        // Assert
        successExecuted.Should().BeTrue();
        failureExecuted.Should().BeTrue();
    }

    [Fact]
    public void MethodChaining_ShouldWorkCorrectly() {
        // Arrange
        var result = global::TnTResult.TnTResult.Success("test");
        var onSuccessExecuted = false;
        var finallyExecuted = false;

        // Act
        var chainedResult = result
            .OnSuccess(value => onSuccessExecuted = true)
            .Finally(() => finallyExecuted = true);

        // Assert
        onSuccessExecuted.Should().BeTrue();
        finallyExecuted.Should().BeTrue();
        chainedResult.Should().Be(result);
    }

    [Fact]
    public void MethodChaining_WithFailure_ShouldWorkCorrectly() {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        var result = global::TnTResult.TnTResult.Failure<string>(exception);
        var onSuccessExecuted = false;
        var onFailureExecuted = false;
        var finallyExecuted = false;

        // Act
        var chainedResult = result
            .OnSuccess(value => onSuccessExecuted = true)
            .OnFailure(error => onFailureExecuted = true)
            .Finally(() => finallyExecuted = true);

        // Assert
        onSuccessExecuted.Should().BeFalse();
        onFailureExecuted.Should().BeTrue();
        finallyExecuted.Should().BeTrue();
        chainedResult.Should().Be(result);
    }

    [Fact]
    public void OnFailure_WithFailedResult_ShouldExecuteActionWithError() {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        var result = global::TnTResult.TnTResult.Failure(exception);
        var executed = false;
        Exception? capturedError = null;

        // Act
        var returnedResult = result.OnFailure(error => {
            executed = true;
            capturedError = error;
        });

        // Assert
        executed.Should().BeTrue();
        capturedError.Should().BeSameAs(exception);
        returnedResult.Should().Be(result);
    }

    [Fact]
    public void OnFailure_WithSuccessfulResult_ShouldNotExecuteAction() {
        // Arrange
        var result = global::TnTResult.TnTResult.Successful;
        var executed = false;
        Exception? capturedError = null;

        // Act
        var returnedResult = result.OnFailure(error => {
            executed = true;
            capturedError = error;
        });

        // Assert
        executed.Should().BeFalse();
        capturedError.Should().BeNull();
        returnedResult.Should().Be(result);
    }

    [Fact]
    public void OnFailureGeneric_WithFailedResult_ShouldExecuteActionWithError() {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        var result = global::TnTResult.TnTResult.Failure<string>(exception);
        var executed = false;
        Exception? capturedError = null;

        // Act
        var returnedResult = result.OnFailure(error => {
            executed = true;
            capturedError = error;
        });

        // Assert
        executed.Should().BeTrue();
        capturedError.Should().BeSameAs(exception);
        returnedResult.Should().Be(result);
    }

    [Fact]
    public void OnFailureGeneric_WithSuccessfulResult_ShouldNotExecuteAction() {
        // Arrange
        var result = global::TnTResult.TnTResult.Success("test");
        var executed = false;
        Exception? capturedError = null;

        // Act
        var returnedResult = result.OnFailure(error => {
            executed = true;
            capturedError = error;
        });

        // Assert
        executed.Should().BeFalse();
        capturedError.Should().BeNull();
        returnedResult.Should().Be(result);
    }

    [Fact]
    public void OnSuccess_WithFailedResult_ShouldNotExecuteAction() {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        var result = global::TnTResult.TnTResult.Failure(exception);
        var executed = false;

        // Act
        var returnedResult = result.OnSuccess(() => executed = true);

        // Assert
        executed.Should().BeFalse();
        returnedResult.Should().Be(result);
    }

    [Fact]
    public void OnSuccess_WithSuccessfulResult_ShouldExecuteAction() {
        // Arrange
        var result = global::TnTResult.TnTResult.Successful;
        var executed = false;

        // Act
        var returnedResult = result.OnSuccess(() => executed = true);

        // Assert
        executed.Should().BeTrue();
        returnedResult.Should().Be(result);
    }

    [Fact]
    public void OnSuccessGeneric_WithFailedResult_ShouldNotExecuteAction() {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        var result = global::TnTResult.TnTResult.Failure<string>(exception);
        var executed = false;

        // Act
        var returnedResult = result.OnSuccess(() => executed = true);

        // Assert
        executed.Should().BeFalse();
        returnedResult.Should().Be(result);
    }

    [Fact]
    public void OnSuccessGeneric_WithSuccessfulResult_ShouldExecuteAction() {
        // Arrange
        var result = global::TnTResult.TnTResult.Success("test");
        var executed = false;

        // Act
        var returnedResult = result.OnSuccess(() => executed = true);

        // Assert
        executed.Should().BeTrue();
        returnedResult.Should().Be(result);
    }

    [Fact]
    public void OnSuccessWithValue_WithFailedResult_ShouldNotExecuteAction() {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        var result = global::TnTResult.TnTResult.Failure<string>(exception);
        var executed = false;
        string? capturedValue = null;

        // Act
        var returnedResult = result.OnSuccess(value => {
            executed = true;
            capturedValue = value;
        });

        // Assert
        executed.Should().BeFalse();
        capturedValue.Should().BeNull();
        returnedResult.Should().Be(result);
    }

    [Fact]
    public void OnSuccessWithValue_WithSuccessfulResult_ShouldExecuteActionWithValue() {
        // Arrange
        var result = global::TnTResult.TnTResult.Success("test");
        var executed = false;
        string? capturedValue = null;

        // Act
        var returnedResult = result.OnSuccess(value => {
            executed = true;
            capturedValue = value;
        });

        // Assert
        executed.Should().BeTrue();
        capturedValue.Should().Be("test");
        returnedResult.Should().Be(result);
    }

    [Fact]
    public void Success_WithNullValue_ShouldStillBeSuccessful() {
        // Act
        var result = global::TnTResult.TnTResult.Success<string?>(null);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.HasFailed.Should().BeFalse();
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Success_WithValue_ShouldCreateSuccessfulResultWithValue() {
        // Act
        var result = global::TnTResult.TnTResult.Success("test");

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.HasFailed.Should().BeFalse();
        result.Value.Should().Be("test");
    }

    [Fact]
    public void Successful_ShouldCreateSuccessfulResult() {
        // Act
        var result = global::TnTResult.TnTResult.Successful;

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.HasFailed.Should().BeFalse();
    }

    [Fact]
    public void TryGetValue_WithFailedResult_ShouldReturnFalseAndDefault() {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        var result = global::TnTResult.TnTResult.Failure<string>(exception);

        // Act
        var success = result.TryGetValue(out var value);

        // Assert
        success.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGetValue_WithNullValue_ShouldReturnTrueAndNull() {
        // Arrange
        var result = global::TnTResult.TnTResult.Success<string?>(null);

        // Act
        var success = result.TryGetValue(out var value);

        // Assert
        success.Should().BeTrue();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGetValue_WithSuccessfulResult_ShouldReturnTrueAndValue() {
        // Arrange
        var result = global::TnTResult.TnTResult.Success("test");

        // Act
        var success = result.TryGetValue(out var value);

        // Assert
        success.Should().BeTrue();
        value.Should().Be("test");
    }

    [Fact]
    public void Value_WithFailedResult_ShouldThrowException() {
        // Arrange
        var exception = new InvalidOperationException("Test error");
        var result = global::TnTResult.TnTResult.Failure<string>(exception);

        // Act & Assert
        var thrownException = Assert.Throws<InvalidOperationException>(() => result.Value);
        thrownException.Message.Should().Be("Attempted to access the expected value, but the result contains an error");
    }

    [Fact]
    public void Value_WithSuccessfulResult_ShouldReturnValue() {
        // Arrange
        var result = global::TnTResult.TnTResult.Success("test");

        // Act & Assert
        result.Value.Should().Be("test");
    }
}