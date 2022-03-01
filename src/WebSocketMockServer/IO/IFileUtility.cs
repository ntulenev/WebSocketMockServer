namespace WebSocketMockServer.IO
{
    /// <summary>
    /// File IO Contract.
    /// </summary>
    public interface IFileUtility
    {
        /// <summary>
        /// Reads file from specific folder
        /// </summary>
        /// <param name="folder">File folder.</param>
        /// <param name="name">File name.</param>
        /// <param name="ct">Token.</param>
        public Task<string> ReadFileAsync(string folder, string name, CancellationToken ct);
    }
}
