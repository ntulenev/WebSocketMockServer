using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace WebSocketMockServer.Helpers
{
    public static class JObjectHelpers
    {
        public static string ReconvertWithJson(this string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("Value is not set properly", nameof(str));
            }

            var jObj = JObject.Parse(str);
            return jObj.ToString(Formatting.Indented);
        }
    }
}
