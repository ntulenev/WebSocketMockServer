namespace WebSocketMockServer.Configuration;

/// <summary>
/// Service configuration that is expected by <see cref="Loader.FileLoader"/>.
/// </summary>
public class FileLoaderConfiguration
{
    /// <summary>
    /// Request/Reactions data.
    /// </summary>
    public required IEnumerable<RequestMappingTemplate> Mapping { get; init; }

    /// <summary>
    /// Root folder for tempalte files.
    /// </summary>
    public required string Folder { get; init; }
}

