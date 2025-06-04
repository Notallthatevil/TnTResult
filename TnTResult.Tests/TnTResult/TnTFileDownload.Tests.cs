using TnTResult;

namespace TnTResult_Tests;

public class TnTFileDownloadTests {

    [Fact]
    public void FileContents_EmptyByteArray_IsSingleton() {
        var empty1 = TnTFileDownload.FileContents.EmptyByteArray;
        var empty2 = TnTFileDownload.FileContents.EmptyByteArray;
        empty1.Should().BeSameAs(empty2);
        empty1.IsByteArray.Should().BeTrue();
        empty1.ByteArray.Should().BeEquivalentTo(Array.Empty<byte>());
    }

    [Fact]
    public void FileContents_ImplicitOperator_Stream_DisposesStream() {
        var ms = new System.IO.MemoryStream(new byte[] { 1, 2, 3 });
        TnTFileDownload.FileContents contents = ms;
        contents.IsStream.Should().BeTrue();
        contents.Stream.Should().BeSameAs(ms);
        contents.Dispose();
        ((Action)(() => ms.ReadByte())).Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public void FileContents_ImplicitOperators_ByteArrayAndUrl() {
        byte[] data = { 1, 2, 3 };
        TnTFileDownload.FileContents contents = data;
        contents.IsByteArray.Should().BeTrue();
        contents.ByteArray.Should().BeEquivalentTo(data);

        string url = "https://example.com/file";
        contents = url;
        contents.IsUrl.Should().BeTrue();
        contents.Url.Should().Be(url);
    }

    [Fact]
    public void TnTFileDownload_Dispose_DisposesContents() {
        var ms = new System.IO.MemoryStream(new byte[] { 1 });
        var file = new TnTFileDownload {
            Filename = "a.txt",
            ContentType = "text/plain",
            Contents = ms
        };
        file.Dispose();
        ((Action)(() => ms.ReadByte())).Should().Throw<ObjectDisposedException>();
    }
}