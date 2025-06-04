using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.IO;
using System.Threading.Tasks;
using TnTResult.AspNetCore.Http;
using Xunit;

namespace TnTResult_Tests.AspNetCore.Http;

public class HttpTnTResultGenericTests {

    [Fact]
    public void Accepted_WithValue_ReturnsAcceptedResult() {
        // Arrange & Act
        var result = HttpTnTResult<string>.Accepted("ok");

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be("ok");
        result.Result.Should().BeOfType<Accepted<string>>();
    }

    [Fact]
    public void BadRequest_ReturnsBadRequestResult() {
        // Arrange & Act
        var result = HttpTnTResult<string>.BadRequest("bad");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().BeOfType<ArgumentException>();
    }

    [Fact]
    public void Created_WithUriAndValue_ReturnsCreatedResult() {
        // Arrange & Act
        var result = HttpTnTResult<string>.Created("/uri", "abc");

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be("abc");
        result.Result.Should().BeOfType<Created<string>>();
    }

    [Fact]
    public void Created_WithValue_ReturnsCreatedResult() {
        // Arrange & Act
        var result = HttpTnTResult<string>.Created("abc");

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be("abc");
        result.Result.Should().BeOfType<Created<string>>();
    }

    [Fact]
    public void CustomError_SetsErrorAndResult() {
        // Arrange
        var ex = new InvalidOperationException("fail");
        var badRequest = TypedResults.BadRequest("bad");

        // Act
        var result = HttpTnTResult<string>.CustomError(ex, badRequest);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().Be(ex);
        result.Result.Should().Be(badRequest);
    }

    [Fact]
    public void CustomResult_SetsValueAndResult() {
        // Arrange
        var ok = TypedResults.Ok("ok");

        // Act
        var result = HttpTnTResult<string>.CustomResult("ok", ok);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be("ok");
        result.Result.Should().Be(ok);
    }

    [Fact]
    public async Task ExecuteAsync_DelegatesToResult() {
        // Arrange
        var httpContext = Substitute.For<HttpContext>();
        var innerResult = Substitute.For<IResult>();
        innerResult.ExecuteAsync(httpContext).Returns(Task.CompletedTask);
        var result = HttpTnTResult<int>.CustomResult(0, innerResult);

        // Act
        await result.ExecuteAsync(httpContext);

        // Assert
        await innerResult.Received(1).ExecuteAsync(httpContext);
    }

    [Fact]
    public void Failure_WithException_SetsError() {
        // Arrange
        var ex = new Exception("fail");

        // Act
        var result = HttpTnTResult<string>.Failure(ex);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().Be(ex);
    }

    [Fact]
    public void Failure_WithString_SetsError() {
        // Arrange & Act
        var result = HttpTnTResult<string>.Failure("fail");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Message.Should().Be("fail");
    }

    [Fact]
    public void Finally_InvokesAction_Always() {
        // Arrange
        var result = HttpTnTResult<string>.Success("ok");
        bool called = false;

        // Act
        result.Finally(() => called = true);

        // Assert
        called.Should().BeTrue();
    }

    [Fact]
    public void Forbid_ReturnsForbidResult() {
        // Arrange & Act
        var result = HttpTnTResult<string>.Forbid();

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().BeOfType<TnTResult.Exceptions.ForbiddenException>();
    }

    [Fact]
    public void NotFound_ReturnsNotFoundResult() {
        // Arrange & Act
        var result = HttpTnTResult<string>.NotFound("missing");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().BeOfType<TnTResult.Exceptions.NotFoundException>();
    }

    [Fact]
    public void Ok_WithValue_ReturnsOkResult() {
        // Arrange & Act
        var result = HttpTnTResult<int>.Ok(42);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be(42);
        result.Result.Should().BeOfType<Ok<int>>();
    }

    [Fact]
    public void OnFailure_InvokesAction_WhenFailed() {
        // Arrange
        var result = HttpTnTResult<string>.Failure("fail");
        bool called = false;

        // Act
        result.OnFailure(e => { called = true; e.Message.Should().Be("fail"); });

        // Assert
        called.Should().BeTrue();
    }

    [Fact]
    public void OnSuccess_InvokesAction_WhenSuccessful() {
        // Arrange
        var result = HttpTnTResult<string>.Success("ok");
        bool called = false;

        // Act
        result.OnSuccess(v => { called = true; v.Should().Be("ok"); });

        // Assert
        called.Should().BeTrue();
    }

    [Fact]
    public void Redirect_WithValueAndUri_ReturnsRedirectResult() {
        // Arrange
        var uri = new Uri("https://example.com");

        // Act
        var result = HttpTnTResult<string>.Redirect("ok", uri);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be("ok");
        result.Result.Should().BeOfType<RedirectHttpResult>();
    }

    [Fact]
    public void Success_WithValue_SetsValue() {
        // Arrange & Act
        var result = HttpTnTResult<string>.Success("ok");

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be("ok");
    }

    [Fact]
    public void TryGetValue_ReturnsFalseAndDefault_WhenFailed() {
        // Arrange
        var result = HttpTnTResult<string>.Failure("fail");

        // Act
        var success = result.TryGetValue(out var value);

        // Assert
        success.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGetValue_ReturnsTrueAndValue_WhenSuccessful() {
        // Arrange
        var result = HttpTnTResult<string>.Success("ok");

        // Act
        var success = result.TryGetValue(out var value);

        // Assert
        success.Should().BeTrue();
        value.Should().Be("ok");
    }

    [Fact]
    public void Unauthorized_ReturnsUnauthorizedResult() {
        // Arrange & Act
        var result = HttpTnTResult<string>.Unauthorized();

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().BeOfType<UnauthorizedAccessException>();
    }
}