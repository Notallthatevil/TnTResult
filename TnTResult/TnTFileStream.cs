namespace TnTResult;

/// <summary>
/// Represents a file stream for TnTResult.
/// </summary>
public class TnTFileStream : IDisposable {

    /// <summary>
    /// Gets the content type of the file.
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    /// Gets the filename of the file.
    /// </summary>
    public string? Filename { get; init; }

    /// <summary>
    /// Gets the stream associated with the file.
    /// </summary>
    public Stream? Stream { get; init; }

    public void Dispose() {
        Stream?.Dispose();
    }
}