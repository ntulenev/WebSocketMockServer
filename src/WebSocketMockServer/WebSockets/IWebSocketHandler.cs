namespace WebSocketMockServer.WebSockets;

/// <summary>
/// Interface for handling web sockets work
/// </summary>
public interface IWebSocketHandler
{
    /// <summary>
    /// Handles web socket operations
    /// </summary>
    public Task HandleAsync(IWebSocketProxy wsProxy, CancellationToken ct);
}
