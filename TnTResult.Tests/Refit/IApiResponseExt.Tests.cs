using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using Refit;
using TnTResult.Refit.Ext;
using Xunit;

namespace TnTResult_Tests.Refit;

public class IApiResponseExtTests {
    [Fact]
    public async Task ToTnTResultAsync_ThrowsOnCancelledTask() {
        // Arrange
        var tcs = new TaskCompletionSource<IApiResponse>();
        tcs.SetCanceled(TestContext.Current.CancellationToken);

        // Act
        var result = await tcs.Task.ToTnTResultAsync();

        // Assert
        result.HasFailed.Should().BeTrue();
        result.Error.Should().BeOfType<OperationCanceledException>();
    }

    [Fact]
    public async Task ToTnTResultAsyncT_ThrowsOnCancelledTask() {
        // Arrange
        var tcs = new TaskCompletionSource<IApiResponse<string>>();
        tcs.SetCanceled(TestContext.Current.CancellationToken);

        // Act
        var result = await tcs.Task.ToTnTResultAsync();

        // Assert
        result.HasFailed.Should().BeTrue();
        result.Error.Should().BeOfType<OperationCanceledException>();
    }

    [Fact]
    public async Task ToTnTResultAsync_Stream_ThrowsOnCancelledTask() {
        // Arrange
        var tcs = new TaskCompletionSource<IApiResponse<Stream>>();
        tcs.SetCanceled(TestContext.Current.CancellationToken);

        // Act
        var result = await tcs.Task.ToTnTResultAsync();

        // Assert
        result.HasFailed.Should().BeTrue();
        result.Error.Should().BeOfType<OperationCanceledException>();
    }

    [Fact]
    public async Task ToTnTResult_ReturnsFailure_OnErrorStatusCode() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse>();
        mockResponse.IsSuccessStatusCode.Returns(false);
        mockResponse.StatusCode.Returns(HttpStatusCode.BadRequest);
        var request = new HttpRequestMessage(HttpMethod.Get, "http://test");
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
            Content = new StringContent("error")
        };
        var apiEx = await ApiException.Create(
            "error",
            request,
            HttpMethod.Get,
            response,
            new RefitSettings(),
            null
        );
        mockResponse.Error.Returns(apiEx);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.HasFailed.Should().BeTrue();
        result.Error.Message.Should().Contain("error");
    }

    [Fact]
    public void ToTnTResult_ReturnsSuccess_OnSuccessStatusCode() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse>();
        mockResponse.IsSuccessStatusCode.Returns(true);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ToTnTResult_Stream_ReturnsFailure_OnNoContent() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse<Stream>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns((Stream?)null);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.HasFailed.Should().BeTrue();
        result.Error.Message.Should().Contain("No content");
    }

    [Fact]
    public void ToTnTResult_Stream_ReturnsFileDownload_OnSuccess() {
        // Arrange
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var httpContent = new StreamContent(stream);
        httpContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "file.txt" };
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        var mockResponse = Substitute.For<IApiResponse<Stream>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns(stream);
        mockResponse.ContentHeaders.Returns(httpContent.Headers);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Filename.Should().Be("file.txt");
        result.Value.ContentType.Should().Be("text/plain");
    }

    [Fact]
    public async Task ToTnTResultAsync_ReturnsSuccess_OnSuccessStatusCode() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        var task = Task.FromResult(mockResponse);

        // Act
        var result = await task.ToTnTResultAsync();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public async Task ToTnTResultAsync_Stream_ReturnsFileDownload_OnSuccess() {
        // Arrange
        var stream = new MemoryStream(new byte[] { 4, 5, 6 });
        var httpContent = new StreamContent(stream);
        httpContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "asyncfile.txt" };
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        var mockResponse = Substitute.For<IApiResponse<Stream>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns(stream);
        mockResponse.ContentHeaders.Returns(httpContent.Headers);
        var task = Task.FromResult(mockResponse);

        // Act
        var result = await task.ToTnTResultAsync();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Filename.Should().Be("asyncfile.txt");
    }

    [Fact]
    public async Task ToTnTResultAsyncT_ReturnsSuccess_OnSuccessStatusCode() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse<string>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns("async-ok");
        var task = Task.FromResult(mockResponse);

        // Act
        var result = await task.ToTnTResultAsync();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be("async-ok");
    }

    [Fact]
    public async Task ToTnTResultT_ReturnsFailure_OnErrorStatusCode() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse<string>>();
        mockResponse.IsSuccessStatusCode.Returns(false);
        mockResponse.StatusCode.Returns(HttpStatusCode.BadRequest);
        var request = new HttpRequestMessage(HttpMethod.Get, "http://test");
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
            Content = new StringContent("bad request")
        };
        var apiEx = await ApiException.Create(
            "bad request",
            request,
            HttpMethod.Get,
            response,
            new RefitSettings(),
            null
        );
        mockResponse.Error.Returns(apiEx);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.HasFailed.Should().BeTrue();
        result.Error.Message.Should().Contain("BadRequest");
    }

    [Fact]
    public void ToTnTResultT_ReturnsLocation_OnCreatedWithLocation() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse<string>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.StatusCode.Returns(HttpStatusCode.Created);
        var responseMessage = new HttpResponseMessage(HttpStatusCode.Created);
        responseMessage.Headers.Location = new Uri("https://test/location");
        mockResponse.Headers.Returns(responseMessage.Headers);
        mockResponse.Content.Returns((string)null!);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be("https://test/location");
    }

    [Fact]
    public void ToTnTResultT_ReturnsSuccess_OnSuccessStatusCode() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse<string>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns("ok");

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be("ok");
    }
}