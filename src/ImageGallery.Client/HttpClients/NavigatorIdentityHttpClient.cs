using System.Net.Http;
using ImageGallery.Client.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ImageGallery.Client.HttpClients
{
    /// <summary>
    /// Instance of NavigatorIdentity HttpClient.
    /// </summary>
    public class NavigatorIdentityHttpClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatorIdentityHttpClient"/> class.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="settings"></param>
        /// <param name="httpContextAccessor"></param>
        public NavigatorIdentityHttpClient(HttpClient client, IOptions<ApplicationOptions> settings, IHttpContextAccessor httpContextAccessor)
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
