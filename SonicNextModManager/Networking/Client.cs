using SonicNextModManager.Interop;
using System.Net.Http;

namespace SonicNextModManager.Networking
{
    public class Client
    {
        public static HttpClient? HttpClient { get; private set; }

        public static HttpClient Get()
        {
            if (!Singleton.HasInstance<HttpClient>())
            {
                // Create new client.
                HttpClient = new();

                // Set up client user agent.
                HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", Properties.Resources.Web_UserAgent);

                // Create client singleton.
                Singleton.SetInstance(HttpClient);
            }

            return Singleton.GetInstance<HttpClient>();
        }
    }
}
