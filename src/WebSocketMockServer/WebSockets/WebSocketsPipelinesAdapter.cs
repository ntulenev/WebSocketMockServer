using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

using Nito.AsyncEx;

namespace WebSocketMockServer.WebSockets
{
    /// <summary>
    /// Pipelines adapter for web sockets
    /// </summary>
    public class WebSocketsPipelinesAdapter
    {

        /// <summary>
        /// Creates web sockets pipeline adapter
        /// </summary>
        /// <param name="webSocket">Web socket that will provide data for pipeline</param>
        /// <param name="ct">Token for cancellation</param>
        public WebSocketsPipelinesAdapter(IWebSocketProxy webSocket, CancellationToken ct = default)
            : this(webSocket, DEFAULT_PIPE_BUFFER_SIZE, ct)
        {
        }

        /// <summary>
        /// Creates web sockets pipeline adapter
        /// </summary>
        /// <param name="webSocket">Web socket that will provide data for pipeline</param>
        /// <param name="minimumBufferSize">Initial memory buffer size for reading from socket</param>
        /// <param name="ct">Token for cancellation</param>
        public WebSocketsPipelinesAdapter(IWebSocketProxy webSocket, int minimumBufferSize, CancellationToken ct = default)
        {
            _webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));

            if (minimumBufferSize <= 0)
            {
                throw new ArgumentException("Buffer size <= 0.");
            }

            _minimumBufferSize = minimumBufferSize;
            _ct = ct;
            _pipe = new Pipe();
        }

        /// <summary>
        /// Returns async iterator for reading data from socket.
        /// </summary>
        /// <returns></returns>
        public async IAsyncEnumerable<byte[]> ReadDataAsync()
        {
            while (!_ct.IsCancellationRequested)
            {
                var result = await _pipe.Reader.ReadAsync(_ct).ConfigureAwait(false);

                var buffer = result.Buffer;

                var data = buffer.ToArray();

                yield return data;

                _pipe.Reader.AdvanceTo(buffer.End);

                if (result.IsCompleted)
                {
                    break;
                }

                _pipeReadMessageGuard.Set();
            }
        }

        /// <summary>
        /// Asynchronously start reading data from socket.
        /// </summary>
        public async Task StartAsync()
        {
            try
            {
                while (_webSocket.State == WebSocketState.Open)
                {
                    var memory = _pipe.Writer.GetMemory(_minimumBufferSize);

                    _ct.ThrowIfCancellationRequested();

                    ValueWebSocketReceiveResult receiveResult;

                    receiveResult = await _webSocket.ReceiveAsync(memory, _ct).ConfigureAwait(false);

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                    else
                    {
                        _pipe.Writer.Advance(receiveResult.Count);

                        if (receiveResult.EndOfMessage)
                        {
                            var result = await _pipe.Writer.FlushAsync(_ct).ConfigureAwait(false);
                            if (result.IsCompleted)
                            {
                                break;
                            }

                            // Crutch to fix incorrect pipe state when reader in slower then writer
                            await _pipeReadMessageGuard.WaitAsync(_ct);
                        }
                    }
                }

                await _pipe.Writer.CompleteAsync().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Skip
            }
        }

        private readonly IWebSocketProxy _webSocket;
        private readonly AsyncAutoResetEvent _pipeReadMessageGuard = new();
        private readonly int _minimumBufferSize;
        private readonly Pipe _pipe;
        private readonly CancellationToken _ct;
        private const int DEFAULT_PIPE_BUFFER_SIZE = 512;
    }
}
