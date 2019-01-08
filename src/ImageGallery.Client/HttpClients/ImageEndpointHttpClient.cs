using System.Net.Http;
using ImageGallery.Client.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ImageGallery.Client.HttpClients
{
    /// <summary>
    /// Instance of NavigatorIdentity HttpClient.
    /// </summary>
    public class ImageEndpointHttpClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageEndpointHttpClient"/> class.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="settings"></param>
        /// <param name="httpContextAccessor"></param>
        public ImageEndpointHttpClient(HttpClient client, IOptions<ApplicationOptions> settings, IHttpContextAccessor httpContextAccessor)
        {
            Instance = client;
            ApplicationSettings = settings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Instance of HttpClient.
        /// </summary>
        public HttpClient Instance { get; }

        private ApplicationOptions ApplicationSettings { get; }
    }
}
