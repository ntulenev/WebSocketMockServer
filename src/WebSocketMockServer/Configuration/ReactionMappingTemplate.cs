namespace WebSocketMockServer.Configuration;

/// <summary>
/// Sub-configuration class for <see cref="RequestMappingTemplate"/> that represents Reactions mapping.
/// </summary>
public class ReactionMappingTemplate
{
    /// <summary>
    /// Response or notification file name.
    /// </summary>
    public required string File { get; init; }

    /// <summary>
    /// Delay before we need to send the Notification. 
    /// </summary>
    public int? Delay { get; init; }
}
