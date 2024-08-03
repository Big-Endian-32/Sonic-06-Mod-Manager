using System.Threading.Tasks;
using SonicNextModManager.Networking;

namespace SonicNextModManager.Helpers
{
    public static class JsonHelper
    {
        /// <summary>
        /// Deserializes the JSON web string to the specified .NET type asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="url">The URL to the JSON to deserialize.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static async Task<T> DeserializeWebObjectAsync<T>(string url)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(await Client.Get().GetStringAsync(url));
            }
            catch
            {
                return default;
            }
        }
    }
}
