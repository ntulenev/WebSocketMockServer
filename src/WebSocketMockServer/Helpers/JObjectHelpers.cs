using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebSocketMockServer.Helpers
{
    public static class JObjectHelpers
    {
        public static string ReconvertWithJson(this string str)
        {
            var jObj = JObject.Parse(str);
            return jObj.ToString(Formatting.Indented);
        }
    }
}
