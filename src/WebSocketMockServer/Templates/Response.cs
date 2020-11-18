using System;

namespace WebSocketMockServer.Templates
{
    public class Response
    {
        public string Result { get; }

        public Response(string result)
        {
            if (result is null)
                throw new ArgumentNullException(nameof(result));

            Result = result;
        }
    }
}
