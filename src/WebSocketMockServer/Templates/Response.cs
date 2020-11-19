using System;

namespace WebSocketMockServer.Templates
{
    public class Response
    {
        public string Result { get; }

        public int Delay => _delay!.Value;

        public bool IsNotification => _delay.HasValue;

        public Response(string result)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            Result = result;
        }

        public Response(string result, int delay)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            if (delay <= 0)
                throw new ArgumentException("deley should be positive", nameof(delay));

            Result = result;
            _delay = delay;
        }

        private int? _delay;
    }
}
