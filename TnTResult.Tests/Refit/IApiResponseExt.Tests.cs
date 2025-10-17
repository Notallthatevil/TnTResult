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
    public void ToTnTResultT_ReturnsLocation_OnRedirectWithLocation_String() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse<string>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.StatusCode.Returns(HttpStatusCode.Found); // 302
        var responseMessage = new HttpResponseMessage(HttpStatusCode.Found);
        responseMessage.Headers.Location = new Uri("https://test/redirect");
        mockResponse.Headers.Returns(responseMessage.Headers);
        mockResponse.Content.Returns((string)null!);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be("https://test/redirect");
    }

    [Fact]
    public void ToTnTResultT_ReturnsLocation_OnRedirectWithLocation_Uri() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse<Uri>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.StatusCode.Returns(HttpStatusCode.Found); // 302
        var responseMessage = new HttpResponseMessage(HttpStatusCode.Found);
        responseMessage.Headers.Location = new Uri("https://test/redirect");
        mockResponse.Headers.Returns(responseMessage.Headers);
        mockResponse.Content.Returns((Uri)null!);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be(new Uri("https://test/redirect"));
    }

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

    // New tests for IApiResponse<TnTFileDownload> overload

    [Fact]
    public void ToTnTResult_TnTFileDownload_ReturnsSuccess_WithByteArrayContents() {
        // Arrange
        var file = new TnTResult.TnTFileDownload {
            Filename = "test.bin",
            ContentType = "application/octet-stream",
            Contents = new byte[] { 1, 2, 3 }
        };
        var mockResponse = Substitute.For<IApiResponse<TnTResult.TnTFileDownload>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns(file);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().BeSameAs(file);
    }

    [Fact]
    public void ToTnTResult_TnTFileDownload_Throws_WhenContentsIsStream() {
        // Arrange
        var stream = new MemoryStream(new byte[] { 4, 5, 6 });
        var file = new TnTResult.TnTFileDownload {
            Filename = "stream.bin",
            ContentType = "application/octet-stream",
            Contents = stream // implicit conversion to FileContents (IsStream = true)
        };
        var mockResponse = Substitute.For<IApiResponse<TnTResult.TnTFileDownload>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns(file);

        // Act
        Action act = () => mockResponse.ToTnTResult();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Use the IApiResponse<Stream> overload instead.*");
    }

    [Fact]
    public void ToTnTResult_TnTFileDownload_ReturnsFailure_WhenContentNull() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse<TnTResult.TnTFileDownload>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns((TnTResult.TnTFileDownload)null!);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.HasFailed.Should().BeTrue();
        result.Error.Message.Should().Contain("No content in the response");
    }

    [Fact]
    public async Task ToTnTResult_TnTFileDownload_ReturnsFailure_OnErrorStatusCode() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse<TnTResult.TnTFileDownload>>();
        mockResponse.IsSuccessStatusCode.Returns(false);
        mockResponse.StatusCode.Returns(HttpStatusCode.BadRequest);
        var request = new HttpRequestMessage(HttpMethod.Get, "http://test/file");
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
            Content = new StringContent("file error")
        };
        var apiEx = await ApiException.Create(
            "file error",
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
        // The current implementation falls back to status code text rather than the error content for this scenario
        result.Error.Message.Should().Contain("BadRequest");
    }

    // Stream disposal tests

    [Fact]
    public void ToTnTResult_Stream_StreamRemainsAccessibleAfterReturn() {
        // Arrange
        var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5 });
        var httpContent = new StreamContent(stream);
        httpContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "test.bin" };
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        var mockResponse = Substitute.For<IApiResponse<Stream>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns(stream);
        mockResponse.ContentHeaders.Returns(httpContent.Headers);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Contents.IsStream.Should().BeTrue();
        
        // Verify stream is still accessible
        var resultStream = result.Value.Contents.Stream;
        resultStream.Should().NotBeNull();
        resultStream!.CanRead.Should().BeTrue("stream should remain accessible after ToTnTResult returns");
        
        // Verify we can read from the stream
        var buffer = new byte[5];
        var bytesRead = resultStream.Read(buffer, 0, 5);
        bytesRead.Should().Be(5);
        buffer.Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public void ToTnTResult_Stream_StreamIsDisposedWhenTnTFileDownloadIsDisposed() {
        // Arrange
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var httpContent = new StreamContent(stream);
        httpContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "test.bin" };
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        var mockResponse = Substitute.For<IApiResponse<Stream>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns(stream);
        mockResponse.ContentHeaders.Returns(httpContent.Headers);

        var result = mockResponse.ToTnTResult();
        result.IsSuccessful.Should().BeTrue();
        
        var resultStream = result.Value.Contents.Stream;
        resultStream.Should().NotBeNull();
        resultStream!.CanRead.Should().BeTrue();

        // Act - Dispose the TnTFileDownload
        result.Value.Dispose();

        // Assert - Stream should now be disposed
        resultStream.CanRead.Should().BeFalse("stream should be disposed when TnTFileDownload is disposed");
    }

    [Fact]
    public void ToTnTResult_Stream_ApiResponseIsDisposedWhenTnTFileDownloadIsDisposed() {
        // Arrange
        var stream = new MemoryStream(new byte[] { 1, 2, 3 });
        var httpContent = new StreamContent(stream);
        httpContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "test.bin" };
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        var mockResponse = Substitute.For<IApiResponse<Stream>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns(stream);
        mockResponse.ContentHeaders.Returns(httpContent.Headers);
        
        var disposeCallCount = 0;
        mockResponse.When(x => x.Dispose()).Do(_ => disposeCallCount++);

        var result = mockResponse.ToTnTResult();
        result.IsSuccessful.Should().BeTrue();
        
        // Assert - IApiResponse should NOT be disposed yet
        disposeCallCount.Should().Be(0, "IApiResponse should not be disposed immediately after ToTnTResult");

        // Act - Dispose the TnTFileDownload
        result.Value.Dispose();

        // Assert - IApiResponse should now be disposed
        disposeCallCount.Should().Be(1, "IApiResponse should be disposed when TnTFileDownload is disposed");
    }

    [Fact]
    public async Task ToTnTResult_Stream_ErrorCaseDisposesApiResponseImmediately() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse<Stream>>();
        mockResponse.IsSuccessStatusCode.Returns(false);
        mockResponse.StatusCode.Returns(HttpStatusCode.NotFound);
        var request = new HttpRequestMessage(HttpMethod.Get, "http://test/file");
        var response = new HttpResponseMessage(HttpStatusCode.NotFound) {
            Content = new StringContent("file not found")
        };
        var apiEx = await ApiException.Create(
            "file not found",
            request,
            HttpMethod.Get,
            response,
            new RefitSettings(),
            null
        );
        mockResponse.Error.Returns(apiEx);
        
        var disposeCallCount = 0;
        mockResponse.When(x => x.Dispose()).Do(_ => disposeCallCount++);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.HasFailed.Should().BeTrue();
        disposeCallCount.Should().Be(1, "IApiResponse should be disposed immediately on error");
    }

    [Fact]
    public void ToTnTResult_Stream_NullContentErrorDisposesApiResponseImmediately() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse<Stream>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns((Stream?)null);
        
        var disposeCallCount = 0;
        mockResponse.When(x => x.Dispose()).Do(_ => disposeCallCount++);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.HasFailed.Should().BeTrue();
        result.Error.Message.Should().Contain("No content");
        disposeCallCount.Should().Be(1, "IApiResponse should be disposed immediately when content is null");
    }

    [Fact]
    public async Task ToTnTResultAsync_Stream_StreamRemainsAccessibleAfterReturn() {
        // Arrange
        var stream = new MemoryStream(new byte[] { 7, 8, 9, 10 });
        var httpContent = new StreamContent(stream);
        httpContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "async.bin" };
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
        result.Value.Contents.IsStream.Should().BeTrue();
        
        var resultStream = result.Value.Contents.Stream;
        resultStream.Should().NotBeNull();
        resultStream!.CanRead.Should().BeTrue("stream should remain accessible after async ToTnTResult returns");
        
        // Verify we can read from the stream
        var buffer = new byte[4];
        var bytesRead = await resultStream.ReadAsync(buffer.AsMemory(0, 4), TestContext.Current.CancellationToken);
        bytesRead.Should().Be(4);
        buffer.Should().BeEquivalentTo(new byte[] { 7, 8, 9, 10 });
    }

    [Fact]
    public async Task ToTnTResultAsync_Stream_DisposalWorksCorrectly() {
        // Arrange
        var stream = new MemoryStream(new byte[] { 11, 12, 13 });
        var httpContent = new StreamContent(stream);
        httpContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "async-dispose.bin" };
        httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        var mockResponse = Substitute.For<IApiResponse<Stream>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns(stream);
        mockResponse.ContentHeaders.Returns(httpContent.Headers);
        
        var disposeCallCount = 0;
        mockResponse.When(x => x.Dispose()).Do(_ => disposeCallCount++);
        
        var task = Task.FromResult(mockResponse);

        // Act
        var result = await task.ToTnTResultAsync();
        result.IsSuccessful.Should().BeTrue();
        
        var resultStream = result.Value.Contents.Stream;
        disposeCallCount.Should().Be(0, "IApiResponse should not be disposed immediately after async ToTnTResult");

        // Dispose the TnTFileDownload
        result.Value.Dispose();

        // Assert
        disposeCallCount.Should().Be(1, "IApiResponse should be disposed when TnTFileDownload is disposed (async case)");
        resultStream!.CanRead.Should().BeFalse("stream should be disposed when TnTFileDownload is disposed (async case)");
    }

    // Tests for general disposable content handling

    [Fact]
    public void ToTnTResultT_DisposableContent_ApiResponseNotDisposedImmediately() {
        // Arrange - Use MemoryStream as the content type (not the Stream->TnTFileDownload overload)
        var stream = new MemoryStream(new byte[] { 20, 21, 22 });
        var mockResponse = Substitute.For<IApiResponse<MemoryStream>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns(stream);
        
        var disposeCallCount = 0;
        mockResponse.When(x => x.Dispose()).Do(_ => disposeCallCount++);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().BeSameAs(stream);
        disposeCallCount.Should().Be(0, "IApiResponse should not be disposed when content is IDisposable");
        stream.CanRead.Should().BeTrue("Stream should still be usable");
    }

    [Fact]
    public void ToTnTResultT_NonDisposableContent_ApiResponseDisposedImmediately() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse<string>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns("test-content");
        
        var disposeCallCount = 0;
        mockResponse.When(x => x.Dispose()).Do(_ => disposeCallCount++);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be("test-content");
        disposeCallCount.Should().Be(1, "IApiResponse should be disposed immediately when content is not IDisposable");
    }

    [Fact]
    public async Task ToTnTResultT_DisposableContent_ErrorCaseDisposesImmediately() {
        // Arrange - Use MemoryStream as the content type
        var mockResponse = Substitute.For<IApiResponse<MemoryStream>>();
        mockResponse.IsSuccessStatusCode.Returns(false);
        mockResponse.StatusCode.Returns(HttpStatusCode.InternalServerError);
        var request = new HttpRequestMessage(HttpMethod.Get, "http://test/stream");
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError) {
            Content = new StringContent("Server error")
        };
        var apiEx = await ApiException.Create(
            "Server error",
            request,
            HttpMethod.Get,
            response,
            new RefitSettings(),
            null
        );
        mockResponse.Error.Returns(apiEx);
        
        var disposeCallCount = 0;
        mockResponse.When(x => x.Dispose()).Do(_ => disposeCallCount++);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.HasFailed.Should().BeTrue();
        disposeCallCount.Should().Be(1, "IApiResponse should be disposed immediately on error even for disposable content types");
    }

    [Fact]
    public void ToTnTResultT_DisposableContent_LocationHeaderStillDisposesResponse() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse<Stream>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.StatusCode.Returns(HttpStatusCode.Found);
        var responseMessage = new HttpResponseMessage(HttpStatusCode.Found);
        responseMessage.Headers.Location = new Uri("https://test/redirect");
        mockResponse.Headers.Returns(responseMessage.Headers);
        mockResponse.Content.Returns(new MemoryStream());
        
        var disposeCallCount = 0;
        mockResponse.When(x => x.Dispose()).Do(_ => disposeCallCount++);

        // Act - This will return the content (Stream) not the location since TSuccess is Stream not string
        var result = mockResponse.ToTnTResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().NotBeNull();
        disposeCallCount.Should().Be(0, "IApiResponse should not be disposed when returning disposable content");
    }

    public class DisposableTestContent : IDisposable {
        public bool IsDisposed { get; private set; }
        public string Data { get; set; } = "test-data";

        public void Dispose() {
            IsDisposed = true;
        }
    }

    [Fact]
    public void ToTnTResult_CustomDisposableContent_NotDisposedImmediately() {
        // Arrange
        var disposableContent = new DisposableTestContent();
        var mockResponse = Substitute.For<IApiResponse<DisposableTestContent>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns(disposableContent);

        var responseDisposed = false;
        mockResponse.When(x => x.Dispose()).Do(_ => responseDisposed = true);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().BeSameAs(disposableContent);
        disposableContent.IsDisposed.Should().BeFalse("Content should not be disposed by ToTnTResult");
        responseDisposed.Should().BeFalse("IApiResponse should not be disposed when content is IDisposable");

        // Verify content is still usable
        result.Value.Data.Should().Be("test-data");
    }

    [Fact]
    public void ToTnTResult_CustomDisposableContent_CallerOwnsDisposal() {
        // Arrange
        var disposableContent = new DisposableTestContent();
        var mockResponse = Substitute.For<IApiResponse<DisposableTestContent>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns(disposableContent);

        var result = mockResponse.ToTnTResult();
        result.IsSuccessful.Should().BeTrue();

        // Act - Caller disposes the content
        result.Value.Dispose();

        // Assert
        disposableContent.IsDisposed.Should().BeTrue("Caller should be able to dispose the content");
    }

    [Fact]
    public void ToTnTResult_MemoryStream_NotDisposedImmediately() {
        // Arrange
        var stream = new MemoryStream(new byte[] { 1, 2, 3, 4, 5 });
        var mockResponse = Substitute.For<IApiResponse<MemoryStream>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns(stream);

        var responseDisposed = false;
        mockResponse.When(x => x.Dispose()).Do(_ => responseDisposed = true);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().BeSameAs(stream);
        stream.CanRead.Should().BeTrue("Stream should remain accessible");
        responseDisposed.Should().BeFalse("IApiResponse should not be disposed for disposable content");

        // Verify we can still read from the stream
        var buffer = new byte[5];
        var bytesRead = stream.Read(buffer, 0, 5);
        bytesRead.Should().Be(5);
        buffer.Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4, 5 });
    }

    [Fact]
    public void ToTnTResult_StringContent_DisposedImmediately() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse<string>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns("non-disposable content");

        var responseDisposed = false;
        mockResponse.When(x => x.Dispose()).Do(_ => responseDisposed = true);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be("non-disposable content");
        responseDisposed.Should().BeTrue("IApiResponse should be disposed immediately for non-disposable content");
    }

    [Fact]
    public void ToTnTResult_IntContent_DisposedImmediately() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse<int>>();
        mockResponse.IsSuccessStatusCode.Returns(true);
        mockResponse.Content.Returns(42);

        var responseDisposed = false;
        mockResponse.When(x => x.Dispose()).Do(_ => responseDisposed = true);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
        result.Value.Should().Be(42);
        responseDisposed.Should().BeTrue("IApiResponse should be disposed immediately for value types");
    }

    [Fact]
    public async Task ToTnTResult_DisposableContent_ErrorStillDisposesResponse() {
        // Arrange
        var mockResponse = Substitute.For<IApiResponse<DisposableTestContent>>();
        mockResponse.IsSuccessStatusCode.Returns(false);
        mockResponse.StatusCode.Returns(HttpStatusCode.BadRequest);
        var request = new HttpRequestMessage(HttpMethod.Get, "http://test");
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
            Content = new StringContent("Bad request")
        };
        var apiEx = await ApiException.Create(
            "Bad request",
            request,
            HttpMethod.Get,
            response,
            new RefitSettings(),
            null
        );
        mockResponse.Error.Returns(apiEx);

        var responseDisposed = false;
        mockResponse.When(x => x.Dispose()).Do(_ => responseDisposed = true);

        // Act
        var result = mockResponse.ToTnTResult();

        // Assert
        result.HasFailed.Should().BeTrue();
        responseDisposed.Should().BeTrue("IApiResponse should be disposed on error even for disposable content types");
    }
}