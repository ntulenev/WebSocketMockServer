using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketMockServer.WebSockets
{
    public interface IWebSocketProxy
    {
        public Task CloseAsync(CancellationToken ct);

        public Task SendMessageAsync(string msg, CancellationToken ct);

        public ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken);

        public WebSocketState State { get; }
    }
}
