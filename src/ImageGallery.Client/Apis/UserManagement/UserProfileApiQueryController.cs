using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Apis.Constants;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.ViewModels.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ImageGallery.Client.Apis.UserManagement
{
    /// <summary>
    /// User Profile Query Controller.
    /// </summary>
    [Authorize]
    [Route(UserManagementRoutes.UserProfile)]
    public class UserProfileApiQueryController : BaseController
    {
        private const string InternalUserProfileRoute = "api/UserProfile";

        private readonly IOptions<ApplicationOptions> _settings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UserProfileApiQueryController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileApiQueryController"/> class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
        public UserProfileApiQueryController(
            IOptions<ApplicationOptions> settings,
            IHttpClientFactory httpClientFactory,
            ILogger<UserProfileApiQueryController> logger)
        {
            _settings = settings;
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger;
        }

        /// <summary>
        /// Get User Properties.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(UserProfileViewModel), 200)]
        public async Task<IActionResult> Get()
        {
            var httpClient = _httpClientFactory.CreateClient("user-management");

            var response = await httpClient.GetAsync(InternalUserProfileRoute).ConfigureAwait(false);

            _logger.LogInformation($"Call {InternalUserProfileRoute} return {response.StatusCode}.");

            if (response.IsSuccessStatusCode)
            {
                var profileAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var userProfileViewModel = JsonConvert.DeserializeObject<UserProfileViewModel>(profileAsString);

                return Ok(userProfileViewModel);
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return Unauthorized();

                case HttpStatusCode.Forbidden:
                    return new ForbidResult();
            }

            return UnprocessableEntity(response.ReasonPhrase);
        }
    }
}
