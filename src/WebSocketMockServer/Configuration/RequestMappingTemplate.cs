namespace WebSocketMockServer.Configuration
{
    /// <summary>
    /// Sub-configuration class for <see cref="FileLoaderConfiguration"/> that represents Request mapping.
    /// </summary>
    public class RequestMappingTemplate
    {
        /// <summary>
        /// Request file name.
        /// </summary>
        public string? File { get; set; }

        /// <summary>
        /// Reactions for request
        /// </summary>
        public IEnumerable<ReactionMappingTemplate>? Reactions { get; set; }
    }
}
