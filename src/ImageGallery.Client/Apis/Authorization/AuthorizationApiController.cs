using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ImageGallery.Client.Apis.Authorization
{
    /// <summary>
    /// Authorization Api Controller.
    /// </summary>
    [Route(ApiRoutes.AuthorizationRoute)]
    public class AuthorizationApiController : BaseController
    {
        private readonly IHttpClientFactory _clientFactory;

        private readonly ILogger<AuthorizationApiController> _logger;

        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationApiController"/> class.
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public AuthorizationApiController(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor, IOptions<ApplicationOptions> settings, ILogger<AuthorizationApiController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Revocation Token on Logout.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("logout")]
        public async Task Logout()
        {
            var client = _clientFactory.CreateClient(HttpClientConstants.NavigatorIdentityApiHttpClient);

            // Get Discovery Document
            var dc = await client.GetDiscoveryDocumentAsync(ApplicationSettings
                .OpenIdConnectConfiguration.Authority);

            _logger.LogInformation($"StatusCode:{dc.StatusCode}|Error{dc.Error}|");
            _logger.LogInformation($"TokenEndpoint:{dc.TokenEndpoint}|RevocationEndpoint{dc.RevocationEndpoint}|");

            // Get Access Token
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            // Revoke Access Token
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                var revokeAccessTokenResponse = await client.RevokeTokenAsync(new TokenRevocationRequest
                {
                    Address = dc.RevocationEndpoint,
                    ClientId = ApplicationSettings.OpenIdConnectConfiguration.ClientId,
                    ClientSecret = ApplicationSettings.OpenIdConnectConfiguration.ClientSecret,
                    Token = accessToken,
                });
                if (revokeAccessTokenResponse.IsError)
                    throw new Exception("Problem encountered while revoking the access token.", revokeAccessTokenResponse.Exception);
            }
        }
    }
}
