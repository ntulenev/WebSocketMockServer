namespace WebSocketMockServer.Configuration;

/// <summary>
/// Sub-configuration class for <see cref="FileLoaderConfiguration"/>
/// that represents Request mapping.
/// </summary>
public class RequestMappingTemplate
{
    /// <summary>
    /// Request file name.
    /// </summary>
    public required string File { get; init; }

    /// <summary>
    /// Reactions for request
    /// </summary>
    public required IEnumerable<ReactionMappingTemplate> Reactions { get; init; }
}
