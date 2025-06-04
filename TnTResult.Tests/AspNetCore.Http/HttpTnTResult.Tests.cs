using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TnTResult.AspNetCore.Http;
using TnTResult.Exceptions;
using Xunit;
using AwesomeAssertions;

namespace TnTResult_Tests.AspNetCore.Http;

public class HttpTnTResultTests {

    [Fact]
    public void Accepted_ShouldReturnAcceptedResult() {
        // Act
        var result = HttpTnTResult.Accepted();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Result.Should().BeOfType<Accepted>();
    }

    [Fact]
    public void BadRequest_ShouldReturnBadRequestResult() {
        // Act
        var result = HttpTnTResult.BadRequest("bad");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().BeOfType<ArgumentException>();
        result.Result.Should().BeOfType<BadRequest<string>>();
    }

    [Fact]
    public void Created_ShouldReturnCreatedResult() {
        // Act
        var result = HttpTnTResult.Created();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Result.Should().BeOfType<Created>();
    }

    [Fact]
    public async Task ExecuteAsync_DelegatesToResult() {
        // Arrange
        var httpContext = Substitute.For<HttpContext>();
        var innerResult = Substitute.For<IResult>();
        innerResult.ExecuteAsync(httpContext).Returns(Task.CompletedTask);
        var result = HttpTnTResult.CustomResult(innerResult);

        // Act
        await result.ExecuteAsync(httpContext);

        // Assert
        await innerResult.Received(1).ExecuteAsync(httpContext);
    }

    [Fact]
    public void Failure_WithException_ShouldSetError() {
        // Arrange
        var ex = new InvalidOperationException("fail");

        // Act
        var result = HttpTnTResult.Failure(ex);

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().Be(ex);
        result.ErrorMessage.Should().Be("fail");
    }

    [Fact]
    public void Finally_InvokesAction_Always() {
        // Arrange
        var result = HttpTnTResult.Successful;
        bool called = false;

        // Act
        result.Finally(() => called = true);

        // Assert
        called.Should().BeTrue();
    }

    [Fact]
    public void Forbid_ShouldReturnForbidResult() {
        // Act
        var result = HttpTnTResult.Forbid();

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().BeOfType<ForbiddenException>();
        result.Result.Should().BeOfType<ForbidHttpResult>();
    }

    [Fact]
    public void NoContent_ShouldReturnNoContentResult() {
        // Act
        var result = HttpTnTResult.NoContent();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public void NotFound_ShouldReturnNotFoundResult() {
        // Act
        var result = HttpTnTResult.NotFound("missing");

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().BeOfType<NotFoundException>();
        result.Result.Should().BeOfType<NotFound<string>>();
    }

    [Fact]
    public void OnFailure_InvokesAction_WhenFailed() {
        // Arrange
        var ex = new Exception("fail");
        var result = HttpTnTResult.Failure(ex);
        bool called = false;

        // Act
        result.OnFailure(e => { called = true; e.Should().Be(ex); });

        // Assert
        called.Should().BeTrue();
    }

    [Fact]
    public void OnSuccess_InvokesAction_WhenSuccessful() {
        // Arrange
        var result = HttpTnTResult.Successful;
        bool called = false;

        // Act
        result.OnSuccess(() => called = true);

        // Assert
        called.Should().BeTrue();
    }

    [Fact]
    public void Redirect_WithString_ShouldReturnRedirectResult() {
        // Act
        var result = HttpTnTResult.Redirect("/path");

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Result.Should().BeOfType<RedirectHttpResult>();
    }

    [Fact]
    public void Redirect_WithUri_ShouldReturnRedirectResult() {
        // Arrange
        var uri = new Uri("https://example.com");

        // Act
        var result = HttpTnTResult.Redirect(uri);

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Result.Should().BeOfType<RedirectHttpResult>();
    }

    [Fact]
    public void Successful_ShouldBeSuccessful() {
        // Arrange & Act
        var result = HttpTnTResult.Successful;

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.HasFailed.Should().BeFalse();
    }

    [Fact]
    public void Unauthorized_ShouldReturnUnauthorizedResult() {
        // Act
        var result = HttpTnTResult.Unauthorized();

        // Assert
        result.IsSuccessful.Should().BeFalse();
        result.Error.Should().BeOfType<UnauthorizedAccessException>();
        result.Result.Should().BeOfType<UnauthorizedHttpResult>();
    }
}