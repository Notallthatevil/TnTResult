using System;
using Xunit;
using AwesomeAssertions;

namespace TnTResult.Tests.TnTResult;

public class ExpectedTests {

    [Fact]
    public void AndThen_WithError_ShouldPreserveError() {
        // Arrange
        var error = new InvalidOperationException("Test error");
        var expected = Expected.MakeUnexpected<string, Exception>(error);

        // Act
        var result = expected.AndThen(value => value.Length);

        // Assert
        result.HasValue.Should().BeFalse();
        result.Error.Should().BeSameAs(error);
    }

    [Fact]
    public void AndThen_WithValue_ShouldApplyFunction() {
        // Arrange
        var expected = Expected.MakeExpected<string, Exception>("test");

        // Act
        var result = expected.AndThen(value => value.Length);

        // Assert
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be(4);
    }

    [Fact]
    public void Constructor_WithConvertibleTypes_ShouldThrowInvalidOperationException() {
        // This test verifies that types that can be converted to each other are rejected String and object would be convertible, so we expect an exception
        var action = () => Expected.MakeExpected<string, object>("test");
        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Constructor_WithError_ShouldCreateExpectedWithError() {
        // Arrange
        var error = new InvalidOperationException("Test error");

        // Act
        Expected<string, Exception> expected = error;

        // Assert
        expected.HasValue.Should().BeFalse();
        expected.Error.Should().BeSameAs(error);
    }

    [Fact]
    public void Constructor_WithNullError_ShouldThrowArgumentNullException() {
        // Act & Assert
        var action = () => Expected.MakeUnexpected<string, Exception>(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithValue_ShouldCreateExpectedWithValue() {
        // Arrange & Act
        Expected<string, Exception> expected = "test";

        // Assert
        expected.HasValue.Should().BeTrue();
        expected.Value.Should().Be("test");
    }

    [Fact]
    public void Equals_DifferentValuesExpected_ShouldReturnFalse() {
        // Arrange
        var expected1 = Expected.MakeExpected<string, Exception>("test1");
        var expected2 = Expected.MakeExpected<string, Exception>("test2");

        // Act & Assert
        expected1.Equals(expected2).Should().BeFalse();
        (expected1 == expected2).Should().BeFalse();
        (expected1 != expected2).Should().BeTrue();
    }

    [Fact]
    public void Equals_SameErrorsExpected_ShouldReturnTrue() {
        // Arrange
        var error = new InvalidOperationException("Test error");
        var expected1 = Expected.MakeUnexpected<string, Exception>(error);
        var expected2 = Expected.MakeUnexpected<string, Exception>(error);

        // Act & Assert
        expected1.Equals(expected2).Should().BeTrue();
        (expected1 == expected2).Should().BeTrue();
        (expected1 != expected2).Should().BeFalse();
    }

    [Fact]
    public void Equals_SameValuesExpected_ShouldReturnTrue() {
        // Arrange
        var expected1 = Expected.MakeExpected<string, Exception>("test");
        var expected2 = Expected.MakeExpected<string, Exception>("test");        // Act & Assert
        expected1.Equals(expected2).Should().BeTrue();
        (expected1 == expected2).Should().BeTrue();
        (expected1 != expected2).Should().BeFalse();
    }

    [Fact]
    public void Equals_ValueAndError_ShouldReturnFalse() {
        // Arrange
        var expectedValue = Expected.MakeExpected<string, Exception>("test");
        var expectedError = Expected.MakeUnexpected<string, Exception>(new InvalidOperationException("Test error"));

        // Act & Assert
        expectedValue.Equals(expectedError).Should().BeFalse();
        (expectedValue == expectedError).Should().BeFalse();
        (expectedValue != expectedError).Should().BeTrue();
    }

    [Fact]
    public void Error_WithError_ShouldReturnError() {
        // Arrange
        var error = new InvalidOperationException("Test error");
        var expected = Expected.MakeUnexpected<string, Exception>(error);

        // Act & Assert
        expected.Error.Should().BeSameAs(error);
    }

    [Fact]
    public void Error_WithValue_ShouldThrowInvalidOperationException() {
        // Arrange
        var expected = Expected.MakeExpected<string, Exception>("test");

        // Act & Assert
        var action = () => expected.Error;
        var exception = action.Should().Throw<InvalidOperationException>().Subject.Single();
        exception.Message.Should().Be("Attempted to access error, but the result contains a value");
    }

    [Fact]
    public void GetHashCode_SameErrors_ShouldReturnSameHashCode() {
        // Arrange
        var error = new InvalidOperationException("Test error");
        var expected1 = Expected.MakeUnexpected<string, Exception>(error);
        var expected2 = Expected.MakeUnexpected<string, Exception>(error);

        // Act & Assert
        expected1.GetHashCode().Should().Be(expected2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_SameValues_ShouldReturnSameHashCode() {
        // Arrange
        var expected1 = Expected.MakeExpected<string, Exception>("test");
        var expected2 = Expected.MakeExpected<string, Exception>("test");

        // Act & Assert
        expected1.GetHashCode().Should().Be(expected2.GetHashCode());
    }

    [Fact]
    public void HasValue_WithError_ShouldReturnFalse() {
        // Arrange
        var error = new InvalidOperationException("Test error");
        var expected = Expected.MakeUnexpected<string, Exception>(error);

        // Act & Assert
        expected.HasValue.Should().BeFalse();
    }

    [Fact]
    public void HasValue_WithValue_ShouldReturnTrue() {
        // Arrange
        var expected = Expected.MakeExpected<string, Exception>("test");

        // Act & Assert
        expected.HasValue.Should().BeTrue();
    }

    [Fact]
    public void MakeExpected_WithValue_ShouldCreateExpectedWithValue() {
        // Act
        var expected = Expected.MakeExpected<string, Exception>("test");

        // Assert
        expected.HasValue.Should().BeTrue();
        expected.Value.Should().Be("test");
    }

    [Fact]
    public void MakeUnexpected_WithError_ShouldCreateExpectedWithError() {
        // Arrange
        var error = new InvalidOperationException("Test error");

        // Act
        var expected = Expected.MakeUnexpected<string, Exception>(error);

        // Assert
        expected.HasValue.Should().BeFalse();
        expected.Error.Should().BeSameAs(error);
    }

    [Fact]
    public void OrElse_WithError_ShouldApplyFunction() {
        // Arrange
        var error = new InvalidOperationException("Test error");
        var expected = Expected.MakeUnexpected<string, Exception>(error);

        // Act
        var result = expected.OrElse(err => new ArgumentException(err.Message));

        // Assert
        result.HasValue.Should().BeFalse();
        result.Error.Should().BeOfType<ArgumentException>();
        result.Error.Message.Should().Be("Test error");
    }

    [Fact]
    public void OrElse_WithValue_ShouldPreserveValue() {
        // Arrange
        var expected = Expected.MakeExpected<string, Exception>("test");

        // Act
        var result = expected.OrElse(error => new ArgumentException(error.Message));

        // Assert
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be("test");
    }

    [Fact]
    public void ToString_WithError_ShouldReturnFormattedString() {
        // Arrange
        var error = new InvalidOperationException("Test error");
        var expected = Expected.MakeUnexpected<string, Exception>(error);

        // Act
        var result = expected.ToString();

        // Assert
        result.Should().Be($"Expected[Error: {error}]");
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnFormattedString() {
        // Arrange
        var expected = Expected.MakeExpected<string, Exception>("test");

        // Act
        var result = expected.ToString();

        // Assert
        result.Should().Be("Expected[Value: test]");
    }

    [Fact]
    public void Transform_WithError_ShouldPreserveError() {
        // Arrange
        var error = new InvalidOperationException("Test error");
        var expected = Expected.MakeUnexpected<string, Exception>(error);

        // Act
        var result = expected.Transform(value => value.Length);

        // Assert
        result.HasValue.Should().BeFalse();
        result.Error.Should().BeSameAs(error);
    }

    [Fact]
    public void Transform_WithValue_ShouldTransformValue() {
        // Arrange
        var expected = Expected.MakeExpected<string, Exception>("test");

        // Act
        var result = expected.Transform(value => value.Length);

        // Assert
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be(4);
    }

    [Fact]
    public void TransformError_WithError_ShouldTransformError() {
        // Arrange
        var error = new InvalidOperationException("Test error");
        var expected = Expected.MakeUnexpected<string, Exception>(error);

        // Act
        var result = expected.TransformError(err => new ArgumentException(err.Message));

        // Assert
        result.HasValue.Should().BeFalse();
        result.Error.Should().BeOfType<ArgumentException>();
        result.Error.Message.Should().Be("Test error");
    }

    [Fact]
    public void TransformError_WithValue_ShouldPreserveValue() {
        // Arrange
        var expected = Expected.MakeExpected<string, Exception>("test");

        // Act
        var result = expected.TransformError(error => new ArgumentException(error.Message));

        // Assert
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be("test");
    }

    [Fact]
    public void Value_WithError_ShouldThrowInvalidOperationException() {
        // Arrange
        var error = new InvalidOperationException("Test error");
        var expected = Expected.MakeUnexpected<string, Exception>(error);

        // Act & Assert
        var action = () => expected.Value;
        var exception = action.Should().Throw<InvalidOperationException>().Subject.Single();
        exception.Message.Should().Be("Attempted to access the expected value, but the result contains an error");
    }

    [Fact]
    public void Value_WithValue_ShouldReturnValue() {
        // Arrange
        var expected = Expected.MakeExpected<string, Exception>("test");

        // Act & Assert
        expected.Value.Should().Be("test");
    }

    [Fact]
    public void ValueOr_WithError_ShouldReturnDefaultValue() {
        // Arrange
        var error = new InvalidOperationException("Test error");
        var expected = Expected.MakeUnexpected<string, Exception>(error);

        // Act
        var result = expected.ValueOr("default");

        // Assert
        result.Should().Be("default");
    }

    [Fact]
    public void ValueOr_WithValue_ShouldReturnValue() {
        // Arrange
        var expected = Expected.MakeExpected<string, Exception>("test");

        // Act
        var result = expected.ValueOr("default");

        // Assert
        result.Should().Be("test");
    }

    [Fact]
    public void ValueOrWithErrorFactory_WithError_ShouldCallFactoryWithErrorAndReturnResult() {
        // Arrange
        var error = new InvalidOperationException("Test error");
        var expected = Expected.MakeUnexpected<string, Exception>(error);
        var factoryCalled = false;
        Exception? capturedError = null;

        // Act
        var result = expected.ValueOr(err => {
            factoryCalled = true;
            capturedError = err;
            return "default";
        });

        // Assert
        result.Should().Be("default");
        factoryCalled.Should().BeTrue();
        capturedError.Should().BeSameAs(error);
    }

    [Fact]
    public void ValueOrWithErrorFactory_WithValue_ShouldReturnValueAndNotCallFactory() {
        // Arrange
        var expected = Expected.MakeExpected<string, Exception>("test");
        var factoryCalled = false;

        // Act
        var result = expected.ValueOr(error => {
            factoryCalled = true;
            return "default";
        });

        // Assert
        result.Should().Be("test");
        factoryCalled.Should().BeFalse();
    }

    [Fact]
    public void ValueOrWithFactory_WithError_ShouldCallFactoryAndReturnResult() {
        // Arrange
        var error = new InvalidOperationException("Test error");
        var expected = Expected.MakeUnexpected<string, Exception>(error);
        var factoryCalled = false;

        // Act
        var result = expected.ValueOr(() => {
            factoryCalled = true;
            return "default";
        });

        // Assert
        result.Should().Be("default");
        factoryCalled.Should().BeTrue();
    }

    [Fact]
    public void ValueOrWithFactory_WithValue_ShouldReturnValueAndNotCallFactory() {
        // Arrange
        var expected = Expected.MakeExpected<string, Exception>("test");
        var factoryCalled = false;

        // Act
        var result = expected.ValueOr(() => {
            factoryCalled = true;
            return "default";
        });

        // Assert
        result.Should().Be("test");
        factoryCalled.Should().BeFalse();
    }
}