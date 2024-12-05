using System.Text.Json;
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
        public Type? Type => Data?.GetType();
        [JsonIgnore]
        public bool IsStream => Data is Stream;
        [JsonIgnore]
        public bool IsByteArray => Data is byte[];
        [JsonIgnore]
        public bool IsUrl => Data is string;
        [JsonIgnore]
        public Stream? Stream => Data as Stream;
        [JsonIgnore]
        public byte[]? ByteArray => Data as byte[];
        [JsonIgnore]
        public string? Url => Data as string;

        [JsonConstructor]
        internal FileContents() { }

        public static implicit operator FileContents(string url) => new() { Data = url };

        public static implicit operator FileContents(Stream data) => new() { Data = data };

        public static implicit operator FileContents(byte[] data) => new() { Data = data };
    }
}
