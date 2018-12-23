using System.Net.Http;

namespace ImageGallery.Client.HttpClients
{
    /// <summary>
    /// Instance of UserManagementHttpClient.
    /// </summary>
    public class UserManagementHttpClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagementHttpClient"/> class.
        /// </summary>
        /// <param name="client"></param>
        public UserManagementHttpClient(HttpClient client)
        {
            Instance = client;
        }

        /// <summary>
        /// Instance of HttpClient.
        /// </summary>
        public HttpClient Instance { get; }
    }
}
