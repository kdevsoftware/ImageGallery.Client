using System;
using System.Threading.Tasks;
using IdentityModel.Client;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.HttpClients;
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
        private readonly NavigatorIdentityHttpClient _navigatorIdentityClient;

        private readonly ILogger<AuthorizationApiController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationApiController"/> class.
        /// </summary>
        /// <param name="navigatorIdentityClient"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public AuthorizationApiController(NavigatorIdentityHttpClient navigatorIdentityClient, IOptions<ApplicationOptions> settings, ILogger<AuthorizationApiController> logger)
        {
            _navigatorIdentityClient = navigatorIdentityClient ?? throw new ArgumentNullException(nameof(logger));
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

            //var discoveryClient = new DiscoveryClient(ApplicationSettings.OpenIdConnectConfiguration.Authority);
            //var metaDataResponse = await discoveryClient.GetAsync();

            //Console.WriteLine(metaDataResponse.TokenEndpoint);
            //Console.WriteLine(metaDataResponse.StatusCode);
            //Console.WriteLine(metaDataResponse.Error);

            var dc = await _navigatorIdentityClient.Instance.GetDiscoveryDocumentAsync(ApplicationSettings
                .OpenIdConnectConfiguration.Authority);

            Console.WriteLine(dc.TokenEndpoint);
            Console.WriteLine(dc.StatusCode);
            Console.WriteLine(dc.Error);
            Console.WriteLine(dc.RevocationEndpoint);

            // get the access token to revoke
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var _token =
                await _navigatorIdentityClient.Instance.RequestTokenAsync(OpenIdConnectParameterNames.AccessToken);




            // create a TokenRevocationClient
            var revocationClient = new TokenRevocationClient(
                dc.RevocationEndpoint,
                ApplicationSettings.OpenIdConnectConfiguration.ClientId,
                ApplicationSettings.OpenIdConnectConfiguration.ClientSecret);


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
