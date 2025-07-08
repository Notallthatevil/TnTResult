using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using TnTResult.AspNetCore.Http.Ext;
using TnTResult.Exceptions;
using Xunit;
using AwesomeAssertions;
using TnTResult;

namespace TnTResult_Tests.AspNetCore.Http;

public class IResultExtTests {

    [Fact]
    public void ToIResult_FailedResult_ForbiddenException_ReturnsForbid() {
        // Arrange
        var result = Substitute.For<ITnTResult>();
        result.IsSuccessful.Returns(false);
        result.Error.Returns(new ForbiddenException());
        result.ErrorMessage.Returns("forbidden");

        // Act
        var iResult = result.ToIResult();

        // Assert
        iResult.Should().BeOfType<ForbidHttpResult>();
    }

    [Fact]
    public void ToIResult_FailedResult_NotFoundException_ReturnsNotFound() {
        // Arrange
        var result = Substitute.For<ITnTResult>();
        result.IsSuccessful.Returns(false);
        result.Error.Returns(new NotFoundException("not found"));
        result.ErrorMessage.Returns("not found");

        // Act
        var iResult = result.ToIResult();

        // Assert
        iResult.Should().BeOfType<NotFound<string>>();
    }

    [Fact]
    public void ToIResult_FailedResult_OperationCanceledException_Returns408() {
        // Arrange
        var result = Substitute.For<ITnTResult>();
        result.IsSuccessful.Returns(false);
        result.Error.Returns(new OperationCanceledException());
        result.ErrorMessage.Returns("canceled");

        // Act
        var iResult = result.ToIResult();

        // Assert
        iResult.Should().BeOfType<StatusCodeHttpResult>();
        ((StatusCodeHttpResult)iResult).StatusCode.Should().Be(StatusCodes.Status408RequestTimeout);
    }

    [Fact]
    public void ToIResult_FailedResult_ReturnsBadRequest() {
        // Arrange
        var result = Substitute.For<ITnTResult>();
        result.IsSuccessful.Returns(false);
        result.Error.Returns(new Exception("fail"));
        result.ErrorMessage.Returns("fail");

        // Act
        var iResult = result.ToIResult();

        // Assert
        iResult.Should().BeOfType<BadRequest<string>>();
    }

    [Fact]
    public void ToIResult_FailedResult_TimeoutException_Returns408() {
        // Arrange
        var result = Substitute.For<ITnTResult>();
        result.IsSuccessful.Returns(false);
        result.Error.Returns(new TaskCanceledException());
        result.ErrorMessage.Returns("timeout");

        // Act
        var iResult = result.ToIResult();

        // Assert
        iResult.Should().BeOfType<StatusCodeHttpResult>();
        ((StatusCodeHttpResult)iResult).StatusCode.Should().Be(StatusCodes.Status408RequestTimeout);
    }

    [Fact]
    public void ToIResult_FailedResult_UnauthorizedException_ReturnsUnauthorized() {
        // Arrange
        var result = Substitute.For<ITnTResult>();
        result.IsSuccessful.Returns(false);
        result.Error.Returns(new UnauthorizedAccessException());
        result.ErrorMessage.Returns("unauthorized");

        // Act
        var iResult = result.ToIResult();

        // Assert
        iResult.Should().BeOfType<UnauthorizedHttpResult>();
    }

    [Fact]
    public void ToIResult_Stream_SuccessfulResult_ReturnsFile() {
        // Arrange
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var result = Substitute.For<ITnTResult<Stream>>();
        result.IsSuccessful.Returns(true);
        result.Value.Returns(stream);

        // Act
        var iResult = result.ToIResult("application/octet-stream", "file.bin");

        // Assert
        iResult.Should().BeOfType<FileStreamHttpResult>();
    }

    [Fact]
    public void ToIResult_SuccessfulResult_AcceptedStatusCode() {
        // Arrange
        var result = Substitute.For<ITnTResult>();
        result.IsSuccessful.Returns(true);

        // Act
        var iResult = result.ToIResult(content: new { id = 1 }, uri: "/accepted", successStatusCode: HttpStatusCode.Accepted);

        // Assert
        iResult.Should().BeOfType<Accepted<object>>();
    }

    [Fact]
    public void ToIResult_SuccessfulResult_CreatedStatusCode() {
        // Arrange
        var result = Substitute.For<ITnTResult>();
        result.IsSuccessful.Returns(true);

        // Act
        var iResult = result.ToIResult(content: new { id = 1 }, uri: "/created", successStatusCode: HttpStatusCode.Created);

        // Assert
        iResult.Should().BeOfType<Created<object>>();
    }

    [Fact]
    public void ToIResult_SuccessfulResult_JsonStatusCode() {
        // Arrange
        var result = Substitute.For<ITnTResult>();
        result.IsSuccessful.Returns(true);

        // Act
        var iResult = result.ToIResult(content: new { id = 1 }, successStatusCode: (HttpStatusCode)299);

        // Assert
        iResult.Should().BeOfType<JsonHttpResult<object>>();
    }

    [Fact]
    public void ToIResult_SuccessfulResult_RedirectStatusCode() {
        // Arrange
        var result = Substitute.For<ITnTResult>();
        result.IsSuccessful.Returns(true);

        // Act
        var iResult = result.ToIResult(content: null, uri: "/redirect", successStatusCode: HttpStatusCode.Redirect);

        // Assert
        iResult.Should().BeOfType<RedirectHttpResult>();
    }

    [Fact]
    public void ToIResult_SuccessfulResult_ReturnsNoContent() {
        // Arrange
        var result = Substitute.For<ITnTResult>();
        result.IsSuccessful.Returns(true);

        // Act
        var iResult = result.ToIResult();

        // Assert
        iResult.Should().BeOfType<NoContent>();
    }

    [Fact]
    public void ToIResult_SuccessfulResult_WithContent_ReturnsOk() {
        // Arrange
        var result = Substitute.For<ITnTResult>();
        result.IsSuccessful.Returns(true);

        // Act
        var iResult = result.ToIResult("hello");

        // Assert
        iResult.Should().BeOfType<ContentHttpResult>();
    }

    [Fact]
    public async Task ToIResultAsync_TaskOfITnTResult_ReturnsIResult() {
        // Arrange
        var result = Substitute.For<ITnTResult>();
        result.IsSuccessful.Returns(true);
        var task = Task.FromResult(result);

        // Act
        var iResult = await task.ToIResultAsync();

        // Assert
        iResult.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ToIResultAsync_TaskOfITnTResultStream_ReturnsFile() {
        // Arrange
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var result = Substitute.For<ITnTResult<Stream>>();
        result.IsSuccessful.Returns(true);
        result.Value.Returns(stream);
        var task = Task.FromResult(result);

        // Act
        var iResult = await task.ToIResultAsync("application/pdf", "file.pdf");

        // Assert
        iResult.Should().BeOfType<FileStreamHttpResult>();
    }

    [Fact]
    public async Task ToIResultAsync_TaskOfITnTResultT_ReturnsIResult() {
        // Arrange
        var result = Substitute.For<ITnTResult<string>>();
        result.IsSuccessful.Returns(true);
        result.Value.Returns("async");
        var task = Task.FromResult(result);

        // Act
        var iResult = await task.ToIResultAsync();

        // Assert
        iResult.Should().BeOfType<ContentHttpResult>();
    }

    [Fact]
    public void ToIResultString_SuccessfulResult_ReturnsOkWithValue() {
        // Arrange
        var result = Substitute.For<ITnTResult<string>>();
        result.IsSuccessful.Returns(true);
        result.Value.Returns("abc");

        // Act
        var iResult = result.ToIResult();

        // Assert
        iResult.Should().BeOfType<ContentHttpResult>();
    }

    [Fact]
    public void ToIResultTnTFileDownload_HandlesByteArrayContent() {
        // Arrange
        var fileDownload = new TnTFileDownload {
            Filename = "file.bin",
            ContentType = "application/octet-stream",
            Contents = (TnTFileDownload.FileContents)new byte[] { 1, 2 }
        };
        var fileResult = Substitute.For<ITnTResult<TnTFileDownload>>();
        fileResult.IsSuccessful.Returns(true);
        fileResult.Value.Returns(fileDownload);

        // Act
        var iResult = fileResult.ToIResult();

        // Assert
        iResult.Should().BeOfType<FileContentHttpResult>();
    }

    [Fact]
    public void ToIResultTnTFileDownload_HandlesNullContent() {
        // Arrange
        var fileResult = Substitute.For<ITnTResult<TnTFileDownload>>();
        fileResult.IsSuccessful.Returns(true);
        fileResult.Value.Returns((TnTFileDownload)null!);

        // Act
        var iResult = fileResult.ToIResult();

        // Assert
        iResult.Should().BeOfType<NoContent>();
    }

    [Fact]
    public void ToIResultTnTFileDownload_HandlesStreamContent() {
        // Arrange
        var stream = new MemoryStream(new byte[] { 1, 2 });
        var fileDownload = new TnTFileDownload {
            Filename = "file.pdf",
            ContentType = "application/pdf",
            Contents = (TnTFileDownload.FileContents)stream
        };
        var fileResult = Substitute.For<ITnTResult<TnTFileDownload>>();
        fileResult.IsSuccessful.Returns(true);
        fileResult.Value.Returns(fileDownload);

        // Act
        var iResult = fileResult.ToIResult();

        // Assert
        iResult.Should().BeOfType<FileStreamHttpResult>();
    }

    [Fact]
    public void ToIResultTnTFileDownload_HandlesUrlContent() {
        // Arrange
        var fileDownload = new TnTFileDownload {
            Filename = "file.txt",
            ContentType = "application/octet-stream",
            Contents = (TnTFileDownload.FileContents)"https://file.com/file"
        };
        var fileResult = Substitute.For<ITnTResult<TnTFileDownload>>();
        fileResult.IsSuccessful.Returns(true);
        fileResult.Value.Returns(fileDownload);

        // Act
        var iResult = fileResult.ToIResult();

        // Assert
        iResult.Should().BeOfType<ContentHttpResult>();
    }

    [Fact]
    public void ToIResultType_SuccessfulResult_ReturnsOkWithValue() {
        // Arrange
        var result = Substitute.For<ITnTResult<object>>();
        result.IsSuccessful.Returns(true);
        result.Value.Returns(new object());

        // Act
        var iResult = result.ToIResult();

        // Assert
        iResult.Should().BeOfType<Ok<object>>();
    }
}