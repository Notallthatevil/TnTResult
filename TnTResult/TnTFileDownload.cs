using System.Text.Json.Serialization;

namespace TnTResult;

public record TnTFileDownload : IDisposable {
    public required string Filename { get; init; }
    public required string ContentType { get; init; }

    public required FileContents Contents { get; init; }

    public void Dispose() {
        if (Contents?.IsStream == true) {
            Contents.Stream?.Dispose();
        }
        GC.SuppressFinalize(this);
    }

    public sealed class FileContents {
        public object? Data { get; init; }

        [JsonIgnore]
        public bool IsStream => Data is Stream;
        [JsonIgnore]
        public bool IsByteArray => Data is byte[];
        [JsonIgnore]
        public bool IsUrl => Data is string;
        [JsonIgnore]
        public Stream Stream => Data as Stream ?? throw new InvalidOperationException("Data is not a stream");
        [JsonIgnore]
        public byte[] ByteArray => Data as byte[] ?? throw new InvalidOperationException("Data is not a byte array");
        [JsonIgnore]
        public string Url => Data as string ?? throw new InvalidOperationException("Data is not a URL");

        private FileContents() { }

        public static implicit operator FileContents(string url) => new FileContents { Data = url };

        public static implicit operator FileContents(Stream data) => new FileContents { Data = data };

        public static implicit operator FileContents(byte[] data) => new FileContents { Data = data };
    }
}