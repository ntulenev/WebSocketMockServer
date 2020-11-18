using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

using Nito.AsyncEx;
namespace SimpleWsApp
{
    public class WebSocketsPipelinesAdapter
    {
        public WebSocketsPipelinesAdapter(WebSocket webSocket, AsyncLock socketGuard, int minimumBufferSize, CancellationToken ct = default)
            : this(webSocket, minimumBufferSize, ct)
        {
            _socketGuard = socketGuard ?? throw new ArgumentNullException(nameof(socketGuard));
        }

        private const int DEFAULT_PIPE_BUFFER_SIZE = 512;

        public WebSocketsPipelinesAdapter(WebSocket webSocket, CancellationToken ct = default)
            : this(webSocket, DEFAULT_PIPE_BUFFER_SIZE, ct)
        {
        }

        public WebSocketsPipelinesAdapter(WebSocket webSocket, int minimumBufferSize, CancellationToken ct = default)
        {
            _webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));

            if (minimumBufferSize <= 0)
                throw new ArgumentException("Buffer size <= 0.");

            _minimumBufferSize = minimumBufferSize;
            _ct = ct;
            _pipe = new Pipe();
        }

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
    }
}
