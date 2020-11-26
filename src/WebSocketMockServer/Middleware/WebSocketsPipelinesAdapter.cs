using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

using Nito.AsyncEx;

namespace WebSocketMockServer.Middleware
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
        /// <param name="socketGuard"><see cref="AsyncLock"/> that could lock ReceiveAsync to sync with some other operations. CloseAsync for insance.</param>
        /// <param name="minimumBufferSize">Initial memory buffer size for reading from socket</param>
        /// <param name="ct">Token for cancellation</param>
        public WebSocketsPipelinesAdapter(WebSocket webSocket, AsyncLock socketGuard, int minimumBufferSize, CancellationToken ct = default)
            : this(webSocket, minimumBufferSize, ct)
        {
            _socketGuard = socketGuard ?? throw new ArgumentNullException(nameof(socketGuard));
        }

        /// <summary>
        /// Creates web sockets pipeline adapter
        /// </summary>
        /// <param name="webSocket">Web socket that will provide data for pipeline</param>
        /// <param name="socketGuard"><see cref="AsyncLock"/> that could lock ReceiveAsync to sync with some other operations. CloseAsync for insance.</param>
        /// <param name="ct">Token for cancellation</param>
        public WebSocketsPipelinesAdapter(WebSocket webSocket, AsyncLock socketGuard, CancellationToken ct = default)
            : this(webSocket, socketGuard, DEFAULT_PIPE_BUFFER_SIZE, ct)
        {
        }

        /// <summary>
        /// Creates web sockets pipeline adapter
        /// </summary>
        /// <param name="webSocket">Web socket that will provide data for pipeline</param>
        /// <param name="ct">Token for cancellation</param>
        public WebSocketsPipelinesAdapter(WebSocket webSocket, CancellationToken ct = default)
            : this(webSocket, DEFAULT_PIPE_BUFFER_SIZE, ct)
        {
        }

        /// <summary>
        /// Creates web sockets pipeline adapter
        /// </summary>
        /// <param name="webSocket">Web socket that will provide data for pipeline</param>
        /// <param name="minimumBufferSize">Initial memory buffer size for reading from socket</param>
        /// <param name="ct">Token for cancellation</param>
        public WebSocketsPipelinesAdapter(WebSocket webSocket, int minimumBufferSize, CancellationToken ct = default)
        {
            _webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));

            if (minimumBufferSize <= 0)
                throw new ArgumentException("Buffer size <= 0.");

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
                ReadResult result = await _pipe.Reader.ReadAsync(_ct).ConfigureAwait(false);

                ReadOnlySequence<byte> buffer = result.Buffer;

                var data = buffer.ToArray();

                yield return data;

                _pipe.Reader.AdvanceTo(buffer.End);

                if (result.IsCompleted)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Asynchronously start reading data from socket.
        /// </summary>
        public async Task StartAsync()
        {
            while (_webSocket.State == WebSocketState.Open)
            {
                Memory<byte> memory = _pipe.Writer.GetMemory(_minimumBufferSize);

                _ct.ThrowIfCancellationRequested();

                ValueWebSocketReceiveResult receiveResult;

                try
                {
                    if (_socketGuard != null)
                    {
                        using (await _socketGuard.LockAsync())
                        {
                            receiveResult = await _webSocket.ReceiveAsync(memory, _ct).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        receiveResult = await _webSocket.ReceiveAsync(memory, _ct).ConfigureAwait(false);
                    }

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                    else
                    {
                        _pipe.Writer.Advance(receiveResult.Count);

                        if (receiveResult.EndOfMessage)
                        {
                            FlushResult result = await _pipe.Writer.FlushAsync(_ct).ConfigureAwait(false);
                            if (result.IsCompleted)
                            {
                                break;
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            await _pipe.Writer.CompleteAsync().ConfigureAwait(false);
        }

        private readonly WebSocket _webSocket;

        private readonly int _minimumBufferSize;

        private readonly Pipe _pipe;

        private readonly AsyncLock? _socketGuard;

        private readonly CancellationToken _ct;

        private const int DEFAULT_PIPE_BUFFER_SIZE = 512;
    }
}
