using System;
using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ImageGallery.Client.Apis
{
    /// <summary>
    ///
    /// </summary>
    [Route("api/images")]
    public class AuthorizationApiController : BaseController
    {
        private readonly ILogger<AuthorizationApiController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationApiController"/> class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public AuthorizationApiController(IOptions<ApplicationOptions> settings, ILogger<AuthorizationApiController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Logout.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("logout")]
        public async Task Logout()
        {
            #region Revocation Token on Logout

            // get the metadata
            Console.WriteLine("ApplicationSettings.Authority" + ApplicationSettings.OpenIdConnectConfiguration.Authority);

            var discoveryClient = new DiscoveryClient(ApplicationSettings.OpenIdConnectConfiguration.Authority);
            var metaDataResponse = await discoveryClient.GetAsync();

            Console.WriteLine(metaDataResponse.TokenEndpoint);
            Console.WriteLine(metaDataResponse.StatusCode);
            Console.WriteLine(metaDataResponse.Error);

            // create a TokenRevocationClient
            var revocationClient = new TokenRevocationClient(
                metaDataResponse.RevocationEndpoint,
                ApplicationSettings.OpenIdConnectConfiguration.ClientId,
                ApplicationSettings.OpenIdConnectConfiguration.ClientSecret);

            // get the access token to revoke
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                Console.WriteLine("Access Token:" + accessToken);

                var revokeAccessTokenResponse =
                    await revocationClient.RevokeAccessTokenAsync(accessToken);

                if (revokeAccessTokenResponse.IsError)
                    throw new Exception("Problem encountered while revoking the access token.", revokeAccessTokenResponse.Exception);
            }

            // revoke the refresh token as well
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var revokeRefreshTokenResponse =
                    await revocationClient.RevokeRefreshTokenAsync(refreshToken);

                if (revokeRefreshTokenResponse.IsError)
                    throw new Exception("Problem encountered while revoking the refresh token.", revokeRefreshTokenResponse.Exception);
            }

            #endregion
        }
    }
}
