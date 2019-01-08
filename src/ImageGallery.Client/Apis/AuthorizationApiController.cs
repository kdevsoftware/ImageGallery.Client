using System;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            }
        }
    }
}
