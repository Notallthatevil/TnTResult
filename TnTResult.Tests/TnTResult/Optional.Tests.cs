using System;
using System.Threading.Tasks;
using Xunit;
using AwesomeAssertions;
using TnTResult;

namespace TnTResult_Tests;

public class OptionalTests {

    [Fact]
    public void AndThen_WithNullAction_ShouldThrowArgumentNullException() {
        // Arrange
        var optional = Optional.MakeOptional("test");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => optional.AndThen(null!)); // No direct AwesomeAssertions equivalent for Throws
    }

    [Fact]
    public void AndThen_WithoutValue_ShouldNotExecuteAction() {
        // Arrange
        var optional = Optional<string>.NullOpt;
        var executed = false;

        // Act
        var result = optional.AndThen(value => executed = true);

        // Assert
        executed.Should().BeFalse();
        result.Should().Be(optional);
    }

    [Fact]
    public void AndThen_WithValue_ShouldExecuteAction() {
        // Arrange
        var optional = Optional.MakeOptional("test");
        var executed = false;
        var capturedValue = string.Empty;

        // Act
        var result = optional.AndThen(value => {
            executed = true;
            capturedValue = value;
        });            // Assert
        executed.Should().BeTrue();
        capturedValue.Should().Be("test");
        result.Should().Be(optional);
    }

    [Fact]
    public async Task AndThenAsync_WithNullAction_ShouldThrowArgumentNullException() {
        // Arrange
        var optional = Optional.MakeOptional("test");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => optional.AndThenAsync(null!)); // No direct AwesomeAssertions equivalent for ThrowsAsync
    }

    [Fact]
    public async Task AndThenAsync_WithoutValue_ShouldNotExecuteAction() {
        // Arrange
        var optional = Optional<string>.NullOpt;
        var executed = false;

        // Act
        var result = await optional.AndThenAsync(async value => {
            await Task.Delay(1);
            executed = true;
        });

        // Assert
        executed.Should().BeFalse();
        result.Should().Be(optional);
    }

    [Fact]
    public async Task AndThenAsync_WithValue_ShouldExecuteAction() {
        // Arrange
        var optional = Optional.MakeOptional("test");
        var executed = false;
        var capturedValue = string.Empty;

        // Act
        var result = await optional.AndThenAsync(async value => {
            await Task.Delay(1);
            executed = true;
            capturedValue = value;
        });

        // Assert
        executed.Should().BeTrue();
        capturedValue.Should().Be("test");
        result.Should().Be(optional);
    }

    [Fact]
    public void ImplicitConversion_FromNull_ShouldCreateEmptyOptional() {
        // Arrange & Act
        Optional<string> optional = null!;

        // Assert
        optional.HasValue.Should().BeFalse();
        optional.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_FromValue_ShouldCreateOptionalWithValue() {
        // Arrange & Act
        Optional<string> optional = "test";

        // Assert
        optional.HasValue.Should().BeTrue();
        optional.Value.Should().Be("test");
    }

    [Fact]
    public void MakeOptional_WithNull_ShouldThrowArgumentNullException() {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => Optional.MakeOptional<string>(null!)); // No direct AwesomeAssertions equivalent for Throws
    }

    [Fact]
    public void MakeOptional_WithValue_ShouldCreateOptionalWithValue() {
        // Act
        var result = Optional.MakeOptional("test");

        // Assert
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be("test");
    }

    [Fact]
    public void NullOpt_ShouldCreateEmptyOptional() {
        // Act
        var result = Optional.NullOpt<string>();

        // Assert
        result.HasValue.Should().BeFalse();
        result.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void Optional_NullOpt_ShouldBeEmpty() {
        // Arrange & Act
        var optional = Optional.NullOpt<string>();

        // Assert
        optional.HasValue.Should().BeFalse();
        optional.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void Optional_StaticNullOpt_ShouldBeEmpty() {
        // Arrange & Act
        var optional = Optional<string>.NullOpt;

        // Assert
        optional.HasValue.Should().BeFalse();
        optional.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void Optional_WithNullValue_ShouldBeEmpty() {
        // Arrange & Act
        Optional<string> optional = null!;

        // Assert
        optional.HasValue.Should().BeFalse();
        optional.IsEmpty.Should().BeTrue();
    }

    [Fact]
    public void Optional_WithValue_ShouldHaveValue() {
        // Arrange & Act
        var optional = Optional.MakeOptional("test");

        // Assert
        optional.HasValue.Should().BeTrue();
        optional.IsEmpty.Should().BeFalse();
        optional.Value.Should().Be("test");
    }

    [Fact]
    public void OrElse_WithNullAction_ShouldThrowArgumentNullException() {
        // Arrange
        var optional = Optional.MakeOptional("test");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => optional.OrElse(null!)); // No direct AwesomeAssertions equivalent for Throws
    }

    [Fact]
    public void OrElse_WithoutValue_ShouldExecuteAction() {
        // Arrange
        var optional = Optional<string>.NullOpt;
        var executed = false;

        // Act
        var result = optional.OrElse(() => executed = true);

        // Assert
        executed.Should().BeTrue();
        result.Should().Be(optional);
    }

    [Fact]
    public void OrElse_WithValue_ShouldNotExecuteAction() {
        // Arrange
        var optional = Optional.MakeOptional("test");
        var executed = false;

        // Act
        var result = optional.OrElse(() => executed = true);            // Assert
        executed.Should().BeFalse();
        result.Should().Be(optional);
    }

    [Fact]
    public async Task OrElseAsync_WithNullAction_ShouldThrowArgumentNullException() {
        // Arrange
        var optional = Optional.MakeOptional("test");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => optional.OrElseAsync(null!)); // No direct AwesomeAssertions equivalent for ThrowsAsync
    }

    [Fact]
    public async Task OrElseAsync_WithoutValue_ShouldExecuteAction() {
        // Arrange
        var optional = Optional<string>.NullOpt;
        var executed = false;

        // Act
        var result = await optional.OrElseAsync(async () => {
            await Task.Delay(1);
            executed = true;
        });

        // Assert
        executed.Should().BeTrue();
        result.Should().Be(optional);
    }

    [Fact]
    public async Task OrElseAsync_WithValue_ShouldNotExecuteAction() {
        // Arrange
        var optional = Optional.MakeOptional("test");
        var executed = false;

        // Act
        var result = await optional.OrElseAsync(async () => {
            await Task.Delay(1);
            executed = true;
        });

        // Assert
        executed.Should().BeFalse();
        result.Should().Be(optional);
    }

    [Fact]
    public void ToString_WithoutValue_ShouldReturnEmptyString() {
        // Arrange
        var optional = Optional<string>.NullOpt;

        // Act
        var result = optional.ToString();

        // Assert
        result.Should().Be("Optional(Empty)");
    }

    [Fact]
    public void ToString_WithValue_ShouldReturnFormattedString() {
        // Arrange
        var optional = Optional.MakeOptional("test");

        // Act
        var result = optional.ToString();

        // Assert
        result.Should().Be("Optional(test)");
    }

    [Fact]
    public void Transform_WithNullFunction_ShouldThrowArgumentNullException() {
        // Arrange
        var optional = Optional.MakeOptional("test");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => optional.Transform<int>(null!)); // No direct AwesomeAssertions equivalent for Throws
    }

    [Fact]
    public void Transform_WithoutValue_ShouldReturnEmpty() {
        // Arrange
        var optional = Optional<string>.NullOpt;

        // Act
        var result = optional.Transform(value => value.ToUpper());

        // Assert
        result.HasValue.Should().BeFalse();
    }

    [Fact]
    public void Transform_WithValue_ShouldTransformValue() {
        // Arrange
        var optional = Optional.MakeOptional("test");

        // Act
        var result = optional.Transform(value => value.Length);

        // Assert
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be(4);
    }

    [Fact]
    public void Transform_WithValueToNull_ShouldReturnEmpty() {
        // Arrange
        var optional = Optional.MakeOptional("test");

        // Act
        var result = optional.Transform(value => (string?)null);

        // Assert
        result.HasValue.Should().BeFalse();
    }

    [Fact]
    public async Task TransformAsync_WithNullFunction_ShouldThrowArgumentNullException() {
        // Arrange
        var optional = Optional.MakeOptional("test");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => optional.TransformAsync<int>(null!)); // No direct AwesomeAssertions equivalent for ThrowsAsync
    }

    [Fact]
    public async Task TransformAsync_WithoutValue_ShouldReturnEmpty() {
        // Arrange
        var optional = Optional<string>.NullOpt;

        // Act
        var result = await optional.TransformAsync(async value => {
            await Task.Delay(1);
            return value.ToUpper();
        });

        // Assert
        result.HasValue.Should().BeFalse();
    }

    [Fact]
    public async Task TransformAsync_WithValue_ShouldTransformValue() {
        // Arrange
        var optional = Optional.MakeOptional("test");

        // Act
        var result = await optional.TransformAsync(async value => {
            await Task.Delay(1);
            return value.Length;
        });

        // Assert
        result.HasValue.Should().BeTrue();
        result.Value.Should().Be(4);
    }

    [Fact]
    public async Task TransformAsync_WithValueToNull_ShouldReturnEmpty() {
        // Arrange
        var optional = Optional.MakeOptional("test");

        // Act
        var result = await optional.TransformAsync(async value => {
            await Task.Delay(1);
            return (string?)null;
        });

        // Assert
        result.HasValue.Should().BeFalse();
    }

    [Fact]
    public void TryGetValue_WithoutValue_ShouldReturnFalseAndDefault() {
        // Arrange
        var optional = Optional<string>.NullOpt;

        // Act
        var result = optional.TryGetValue(out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGetValue_WithValue_ShouldReturnTrueAndValue() {
        // Arrange
        var optional = Optional.MakeOptional("test");

        // Act
        var result = optional.TryGetValue(out var value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be("test");
    }

    [Fact]
    public void Value_WithoutValue_ShouldThrowInvalidOperationException() {
        // Arrange
        var optional = Optional<string>.NullOpt;

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => optional.Value); // No direct AwesomeAssertions equivalent for Throws
        exception.Message.Should().Be("Attempted to obtain the value of an optional, but this optional is empty");
    }

    [Fact]
    public void Value_WithValue_ShouldReturnValue() {
        // Arrange
        var optional = Optional.MakeOptional(42);

        // Act & Assert
        optional.Value.Should().Be(42);
    }

    [Fact]
    public void ValueOr_WithNullDefaultValue_ShouldThrowArgumentNullException() {
        // Arrange
        var optional = Optional<string>.NullOpt;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => optional.ValueOr(null!)); // No direct AwesomeAssertions equivalent for Throws
    }

    [Fact]
    public void ValueOr_WithoutValue_ShouldReturnDefaultValue() {
        // Arrange
        var optional = Optional<string>.NullOpt;

        // Act
        var result = optional.ValueOr("default");

        // Assert
        result.Should().Be("default");
    }

    [Fact]
    public void ValueOr_WithValue_ShouldReturnValue() {
        // Arrange
        var optional = Optional.MakeOptional("test");

        // Act
        var result = optional.ValueOr("default");

        // Assert
        result.Should().Be("test");
    }
}