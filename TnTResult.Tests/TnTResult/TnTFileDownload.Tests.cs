using System.Text.Json;
using System.Text.Json.Serialization;
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

        [Fact]
    public void FileContents_DataProperty_And_TypeProperty_WorkForAllTypes() {
        var stream = new System.IO.MemoryStream();
        TnTFileDownload.FileContents fcStream = stream;
        fcStream.Data.Should().BeSameAs(stream);
        fcStream.Type.Should().Be(typeof(System.IO.MemoryStream));
        fcStream.IsStream.Should().BeTrue();
        fcStream.IsByteArray.Should().BeFalse();
        fcStream.IsUrl.Should().BeFalse();
        fcStream.Stream.Should().BeSameAs(stream);
        fcStream.ByteArray.Should().BeNull();
        fcStream.Url.Should().BeNull();
        fcStream.Dispose();

        byte[] arr = { 4, 5 };
        TnTFileDownload.FileContents fcBytes = arr;
        fcBytes.Data.Should().BeSameAs(arr);
        fcBytes.Type.Should().Be(typeof(byte[]));
        fcBytes.IsStream.Should().BeFalse();
        fcBytes.IsByteArray.Should().BeTrue();
        fcBytes.IsUrl.Should().BeFalse();
        fcBytes.Stream.Should().BeNull();
        fcBytes.ByteArray.Should().BeSameAs(arr);
        fcBytes.Url.Should().BeNull();

        string url = "http://x";
        TnTFileDownload.FileContents fcUrl = url;
        fcUrl.Data.Should().Be(url);
        fcUrl.Type.Should().Be(typeof(string));
        fcUrl.IsStream.Should().BeFalse();
        fcUrl.IsByteArray.Should().BeFalse();
        fcUrl.IsUrl.Should().BeTrue();
        fcUrl.Stream.Should().BeNull();
        fcUrl.ByteArray.Should().BeNull();
        fcUrl.Url.Should().Be(url);
    }

    [Fact]
    public void FileContents_ImplicitOperator_EmptyByteArray_ReturnsSingleton() {
        byte[] empty = Array.Empty<byte>();
        var fc = (TnTFileDownload.FileContents)empty;
        fc.Should().BeSameAs(TnTFileDownload.FileContents.EmptyByteArray);
    }

    [Fact]
    public void FileContents_Dispose_CanBeCalledTwiceSafely() {
        var ms = new System.IO.MemoryStream();
        TnTFileDownload.FileContents fc = ms;
        fc.Dispose();
        fc.Dispose(); // Should not throw
    }

    [Fact]
    public void TnTFileDownload_Dispose_CanBeCalledTwiceSafely() {
        var ms = new System.IO.MemoryStream(new byte[] { 1 });
        var file = new TnTFileDownload {
            Filename = "b.txt",
            ContentType = "text/plain",
            Contents = ms
        };
        file.Dispose();
        file.Dispose(); // Should not throw
    }

    [Fact]
    public void FileContents_JsonConstructor_AllowsInit() {
        var fc = (TnTFileDownload.FileContents)Activator.CreateInstance(typeof(TnTFileDownload.FileContents), true)!;
        fc.Should().NotBeNull();
        // Data is null by default
        fc.Data.Should().BeNull();
        fc.Type.Should().BeNull();
        fc.IsStream.Should().BeFalse();
        fc.IsByteArray.Should().BeFalse();
        fc.IsUrl.Should().BeFalse();
    }

    [Fact]
    public void FileContents_Dispose_NonStreamType_DoesNotThrow() {
        TnTFileDownload.FileContents fc = new byte[] { 1, 2 };
        fc.Dispose();
        fc.Dispose(); // Should not throw
        TnTFileDownload.FileContents fc2 = "url";
        fc2.Dispose();
        fc2.Dispose(); // Should not throw
    }

    [Fact]
    public void TnTFileDownload_Json_SerializeDeserialize_ByteArray()
    {
        // Arrange
        var original = new TnTFileDownload
        {
            Filename = "file.bin",
            ContentType = "application/octet-stream",
            Contents = new byte[] { 1, 2, 3, 4 }
        };
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Act
        var json = JsonSerializer.Serialize(original, options);
        var deserialized = JsonSerializer.Deserialize<TnTFileDownload>(json, options)!;

        // Assert
        deserialized.Filename.Should().Be(original.Filename);
        deserialized.ContentType.Should().Be(original.ContentType);
        deserialized.Contents.IsByteArray.Should().BeTrue();
        deserialized.Contents.ByteArray.Should().BeEquivalentTo(original.Contents.ByteArray);
    }

    [Fact]
    public void TnTFileDownload_Json_SerializeDeserialize_Url()
    {
        // Arrange
        var original = new TnTFileDownload
        {
            Filename = "file.txt",
            ContentType = "text/plain",
            Contents = "https://example.com/file.txt"
        };
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Act
        var json = JsonSerializer.Serialize(original, options);
        var deserialized = JsonSerializer.Deserialize<TnTFileDownload>(json, options)!;

        // Assert
        deserialized.Filename.Should().Be(original.Filename);
        deserialized.ContentType.Should().Be(original.ContentType);
        deserialized.Contents.IsUrl.Should().BeTrue();
        deserialized.Contents.Url.Should().Be(original.Contents.Url);
    }

    [Fact]
    public void TnTFileDownload_Json_SerializeDeserialize_EmptyByteArray()
    {
        // Arrange
        var original = new TnTFileDownload
        {
            Filename = "empty.bin",
            ContentType = "application/octet-stream",
            Contents = Array.Empty<byte>()
        };
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Act
        var json = JsonSerializer.Serialize(original, options);
        var deserialized = JsonSerializer.Deserialize<TnTFileDownload>(json, options)!;

        // Assert
        deserialized.Filename.Should().Be(original.Filename);
        deserialized.ContentType.Should().Be(original.ContentType);
        deserialized.Contents.IsByteArray.Should().BeTrue();
        deserialized.Contents.ByteArray.Should().BeEquivalentTo(Array.Empty<byte>());
    }
}