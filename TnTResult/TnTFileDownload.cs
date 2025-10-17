using System.Text.Json;
using System.Text.Json.Serialization;

namespace TnTResult;

/// <summary>
///     Represents a file download with metadata and content data that can be a stream, byte array, or URL. Implements proper disposal pattern for resource management, especially for stream-based content.
/// </summary>
/// <remarks>
///     This record provides a unified way to handle different types of file content while ensuring proper resource cleanup. The Contents property can hold streams, byte arrays, or URLs, and the class
///     will automatically dispose of streams when the object is disposed.
/// </remarks>
public record TnTFileDownload : IDisposable {
    /// <summary>
    ///     Gets the filename of the file to be downloaded.
    /// </summary>
    /// <value>The name of the file including extension.</value>
    public required string Filename { get; init; }

    /// <summary>
    ///     Gets the MIME content type of the file.
    /// </summary>
    /// <value>The content type (e.g., "application/pdf", "image/jpeg", "text/plain").</value>
    public required string ContentType { get; init; }

    /// <summary>
    ///     Gets the file contents which can be a stream, byte array, or URL.
    /// </summary>
    /// <value>A <see cref="FileContents" /> instance containing the actual file data.</value>
    public required FileContents Contents { get; init; }

    /// <summary>
    ///     Gets the disposable content associated with this instance.
    /// </summary>
    internal IDisposable? DisposableContent { get; init; }

    private bool _disposed;

    /// <summary>
    ///     Releases all resources used by the <see cref="TnTFileDownload" />.
    /// </summary>
    /// <remarks>This method disposes of the underlying <see cref="Contents" /> which will automatically dispose of any streams it contains.</remarks>
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Releases the unmanaged resources used by the <see cref="TnTFileDownload" /> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing) {
        if (!_disposed && disposing) {
            _disposed = true;
            Contents?.Dispose();
            DisposableContent?.Dispose();
        }
    }
    /// <summary>
    ///     Represents the contents of a file that can be stored as a stream, byte array, or URL reference. Provides type-safe access to the underlying data and handles proper disposal of stream resources.
    /// </summary>
    /// <remarks>
    ///     This class uses an internal enum to track the content type for optimal performance, avoiding repeated type checking and casting operations. It supports JSON serialization through the Data
    ///     property while providing strongly-typed access through specific properties.
    /// </remarks>
    [JsonConverter(typeof(FileContentsJsonConverter))]
    public sealed class FileContents : IDisposable {
        private readonly object? _data;
        private readonly FileContentType _contentType;
        private readonly Type? _cachedType;
        private bool _disposed;

        // Optimized static instances for common empty cases
        private static readonly FileContents _emptyByteArray = new(Array.Empty<byte>(), FileContentType.ByteArray);

        /// <summary>
        ///     Gets a singleton instance representing an empty byte array for optimization.
        /// </summary>
        /// <value>A cached <see cref="FileContents" /> instance with an empty byte array.</value>
        /// <remarks>Use this property to avoid allocations when working with empty byte arrays.</remarks>
        [JsonIgnore]
        public static FileContents EmptyByteArray => _emptyByteArray;

        /// <summary>
        ///     Gets the runtime type of the underlying data.
        /// </summary>
        /// <value>The <see cref="Type" /> of the data, or <see langword="null" /> if data is null.</value>
        /// <remarks>This property is cached for performance to avoid repeated type lookups.</remarks>
        [JsonIgnore]
        public Type? Type => _cachedType;

        /// <summary>
        ///     Gets a value indicating whether the content is a stream.
        /// </summary>
        /// <value><see langword="true" /> if the content is a <see cref="Stream" />; otherwise, <see langword="false" />.</value>
        [JsonIgnore]
        public bool IsStream => _contentType == FileContentType.Stream;

        /// <summary>
        ///     Gets a value indicating whether the content is a byte array.
        /// </summary>
        /// <value><see langword="true" /> if the content is a byte array; otherwise, <see langword="false" />.</value>
        [JsonIgnore]
        public bool IsByteArray => _contentType == FileContentType.ByteArray;

        /// <summary>
        ///     Gets a value indicating whether the content is a URL string.
        /// </summary>
        /// <value><see langword="true" /> if the content is a URL string; otherwise, <see langword="false" />.</value>
        [JsonIgnore]
        public bool IsUrl => _contentType == FileContentType.Url;

        /// <summary>
        ///     Gets the content as a stream if the content type is a stream.
        /// </summary>
        /// <value>The <see cref="Stream" /> content, or <see langword="null" /> if the content is not a stream.</value>
        [JsonIgnore]
        public Stream? Stream => _contentType == FileContentType.Stream ? (Stream?)_data : null;

        /// <summary>
        ///     Gets the content as a byte array if the content type is a byte array.
        /// </summary>
        /// <value>The byte array content, or <see langword="null" /> if the content is not a byte array.</value>
        [JsonIgnore]
        public byte[]? ByteArray => _contentType == FileContentType.ByteArray ? (byte[]?)_data : null;

        /// <summary>
        ///     Gets the content as a URL string if the content type is a URL.
        /// </summary>
        /// <value>The URL string content, or <see langword="null" /> if the content is not a URL.</value>
        [JsonIgnore]
        public string? Url => _contentType == FileContentType.Url ? (string?)_data : null;

        /// <summary>
        ///     Gets or sets the raw data content for JSON serialization.
        /// </summary>
        /// <value>The underlying data object which can be a <see cref="Stream" />, byte array, or string URL.</value>
        /// <remarks>
        ///     This property is primarily used for JSON serialization. When setting the value, the content type is automatically determined based on the runtime type of the data. For type-safe
        ///     access, use the specific properties like <see cref="Stream" />, <see cref="ByteArray" />, or <see cref="Url" />.
        /// </remarks>
        public object? Data {
            get => _data;
            init {
                _data = value;
                _cachedType = value?.GetType();
                _contentType = value switch {
                    Stream _ => FileContentType.Stream,
                    byte[] _ => FileContentType.ByteArray,
                    string _ => FileContentType.Url,
                    _ => FileContentType.Unknown
                };
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileContents" /> class for JSON deserialization.
        /// </summary>
        /// <remarks>This constructor is used internally by the JSON serializer. Use the implicit operators or the Data property setter for normal object creation.</remarks>
        [JsonConstructor]
        internal FileContents() { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileContents" /> class with the specified data and content type.
        /// </summary>
        /// <param name="data">       The file content data.</param>
        /// <param name="contentType">The type of the content data.</param>
        private FileContents(object? data, FileContentType contentType) {
            _data = data;
            _cachedType = data?.GetType();
            _contentType = contentType;
        }

        /// <summary>
        ///     Implicitly converts a string URL to a <see cref="FileContents" /> instance.
        /// </summary>
        /// <param name="url">The URL string to convert.</param>
        /// <returns>A new <see cref="FileContents" /> instance containing the URL.</returns>
        /// <example>
        ///     <code>
        ///FileContents contents = "https://example.com/file.pdf";
        ///     </code>
        /// </example>
        public static implicit operator FileContents(string url) => new(url, FileContentType.Url);

        /// <summary>
        ///     Implicitly converts a stream to a <see cref="FileContents" /> instance.
        /// </summary>
        /// <param name="data">The stream to convert.</param>
        /// <returns>A new <see cref="FileContents" /> instance containing the stream.</returns>
        /// <remarks>The caller is responsible for ensuring the stream remains valid for the lifetime of the FileContents instance. The stream will be disposed when the FileContents is disposed.</remarks>
        /// <example>
        ///     <code>
        ///var fileStream = File.OpenRead("example.pdf");
        ///FileContents contents = fileStream;
        ///     </code>
        /// </example>
        public static implicit operator FileContents(Stream data) => new(data, FileContentType.Stream);

        /// <summary>
        ///     Implicitly converts a byte array to a <see cref="FileContents" /> instance.
        /// </summary>
        /// <param name="data">The byte array to convert.</param>
        /// <returns>A new <see cref="FileContents" /> instance containing the byte array.</returns>
        /// <example>
        ///     <code>
        ///byte[] fileBytes = File.ReadAllBytes("example.pdf");
        ///FileContents contents = fileBytes;
        ///FileContents empty = FileContents.EmptyByteArray;
        ///     </code>
        /// </example>
        public static implicit operator FileContents(byte[] data) =>
            // Optimization: return cached instance for empty arrays
            data.Length == 0 ? _emptyByteArray : new(data, FileContentType.ByteArray);

        /// <summary>
        ///     Releases all resources used by the <see cref="FileContents" />.
        /// </summary>
        /// <remarks>This method will dispose of any underlying stream resources. Byte arrays and URLs do not require disposal.</remarks>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases the unmanaged resources used by the <see cref="FileContents" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
        private void Dispose(bool disposing) {
            if (!_disposed && disposing) {
                if (_contentType == FileContentType.Stream && _data is Stream stream) {
                    stream.Dispose();
                }
                _disposed = true;
            }
        }

        /// <summary>
        ///     Defines the types of content that can be stored in a <see cref="FileContents" /> instance.
        /// </summary>
        /// <remarks>This enum is used internally for performance optimization to avoid repeated type checking. The byte underlying type minimizes memory usage.</remarks>
        private enum FileContentType : byte {
            /// <summary>
            ///     Unknown or unsupported content type.
            /// </summary>
            Unknown = 0,
            /// <summary>
            ///     Content is a <see cref="Stream" />.
            /// </summary>
            Stream = 1,
            /// <summary>
            ///     Content is a byte array.
            /// </summary>
            ByteArray = 2,
            /// <summary>
            ///     Content is a URL string.
            /// </summary>
            Url = 3
        }
    }

    public class FileContentsJsonConverter : JsonConverter<TnTFileDownload.FileContents> {
        public override TnTFileDownload.FileContents? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            if (reader.TokenType != JsonTokenType.StartObject) {
                throw new JsonException();
            }

            string? type = null;
            object? data = null;
            while (reader.Read()) {
                if (reader.TokenType == JsonTokenType.EndObject) {
                    break;
                }

                if (reader.TokenType != JsonTokenType.PropertyName) {
                    throw new JsonException();
                }

                var propName = reader.GetString();
                reader.Read();
                if (propName == "Type") {
                    type = reader.GetString();
                }
                else if (propName == "Data") {
                    if (type == typeof(byte[]).FullName) {
                        data = JsonSerializer.Deserialize<byte[]>(ref reader, options);
                    }
                    else if (type == typeof(string).FullName) {
                        data = reader.TokenType == JsonTokenType.Null ? null : reader.GetString();
                    }
                    else {
                        data = null;
                    }
                }
            }
            if (type == typeof(byte[]).FullName) {
                return (byte[])(data ?? Array.Empty<byte>());
            }

            if (type == typeof(string).FullName) {
                return (string?)data ?? string.Empty;
            }
            return new TnTFileDownload.FileContents();
        }

        public override void Write(Utf8JsonWriter writer, TnTFileDownload.FileContents value, JsonSerializerOptions options) {
            writer.WriteStartObject();
            if (value.IsByteArray) {
                writer.WriteString("Type", typeof(byte[]).FullName);
                writer.WritePropertyName("Data");
                JsonSerializer.Serialize(writer, value.ByteArray, options);
            }
            else if (value.IsUrl) {
                writer.WriteString("Type", typeof(string).FullName);
                writer.WritePropertyName("Data");
                writer.WriteStringValue(value.Url);
            }
            else {
                writer.WritePropertyName("Type");
                writer.WriteNullValue();
                writer.WritePropertyName("Data");
                writer.WriteNullValue();
            }
            writer.WriteEndObject();
        }
    }
}