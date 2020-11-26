namespace WebSocketMockServer.Configuration
{
    /// <summary>
    /// Sub-configuration class for <see cref="RequestMappingTemplate"/> that represents Response mapping.
    /// </summary>
    public class ResponseMappingTemplate
    {
        /// <summary>
        /// Response file name.
        /// </summary>
        public string? File { get; set; }

        /// <summary>
        /// Delay before we need to send the response. 
        /// </summary>
        public int? Delay { get; set; }
    }
}
