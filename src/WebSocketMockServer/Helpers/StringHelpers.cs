using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebSocketMockServer.Helpers
{
    /// <summary>
    /// Helper class for String / JSON operations
    /// </summary>
    public static class StringHelpers
    {
        /// <summary>
        /// Helper method that gets string and convert to JSON Indented-formatted srting. 
        /// </summary>
        /// <param name="str">input json string.</param>
        /// <exception cref="ArgumentNullException">Throws if string is null or empty.</exception>
        /// <exception cref="ArgumentException">Throws if string contains only spaces.</exception>
        /// <exception cref="InvalidOperationException">Throws if string has incorrect format.</exception>
        public static string ReconvertWithJson(this string str)
        {
            if (str is null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("Value is not set properly.", nameof(str));
            }

            JObject jObj;

            try
            {
                jObj = JObject.Parse(str);
            }
            catch (JsonReaderException ex)
            {
                throw new InvalidOperationException("Incorrect formatting.", ex);
            }

            return jObj.ToString(Formatting.Indented);
        }
    }
}
