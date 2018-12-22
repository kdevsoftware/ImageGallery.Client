using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.Client.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ImageGallery.Client.Services
{
    /// <summary>
    ///
    /// </summary>
    public class ImageGalleryHttpClient : IImageGalleryHttpClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageGalleryHttpClient"/> class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="httpContextAccessor"></param>
        public ImageGalleryHttpClient(IOptions<ApplicationOptions> settings, IHttpContextAccessor httpContextAccessor)
        {
            ApplicationSettings = settings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        private ApplicationOptions ApplicationSettings { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="apiUri"></param>
        /// <returns></returns>
        public async Task<HttpClient> GetClient(string apiUri)
        {
            var currentContext = _httpContextAccessor.HttpContext;

            // get access token
            var accessToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                _httpClient.SetBearerToken(accessToken);
            }

            _httpClient.BaseAddress = new Uri(apiUri);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return _httpClient;
        }

        /// <summary>
        ///  Renew Tokens.
        /// </summary>
        /// <returns></returns>
        private async Task<string> RenewTokens()
        {
            // get the current HttpContext to access the tokens
            var currentContext = _httpContextAccessor.HttpContext;

            // get the metadata
            var discoveryClient = new DiscoveryClient(ApplicationSettings.OpenIdConnectConfiguration.Authority);
            var metaDataResponse = await discoveryClient.GetAsync();

            // create a new token client to get new tokens
            var tokenClient = new TokenClient(metaDataResponse.TokenEndpoint, ApplicationSettings.OpenIdConnectConfiguration.ClientId, ApplicationSettings.OpenIdConnectConfiguration.ClientSecret);

            // get the saved refresh token
            var currentRefreshToken = await currentContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            // refresh the tokens
            var tokenResult = await tokenClient.RequestRefreshTokenAsync(currentRefreshToken);

            if (!tokenResult.IsError)
            {
                // Save the tokens.

                // get auth info
                var authenticateInfo = await currentContext.AuthenticateAsync("Cookies");

                // create a new value for expires_at, and save it
                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);
                authenticateInfo.Properties.UpdateTokenValue("expires_at", expiresAt.ToString("o", CultureInfo.InvariantCulture));

                authenticateInfo.Properties.UpdateTokenValue(
                    OpenIdConnectParameterNames.AccessToken,
                    tokenResult.AccessToken);
                authenticateInfo.Properties.UpdateTokenValue(
                    OpenIdConnectParameterNames.RefreshToken,
                    tokenResult.RefreshToken);

                // we're signing in again with the new values.
                await currentContext.SignInAsync("Cookies", authenticateInfo.Principal, authenticateInfo.Properties);

                // return the new access token
                return tokenResult.AccessToken;
            }
            else
            {
                throw new Exception(
                    $"Problem encountered while refreshing tokens.",
                    tokenResult.Exception);
            }
        }
    }
}
