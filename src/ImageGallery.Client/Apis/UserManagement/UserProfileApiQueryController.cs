using System;
using System.Net;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Apis.Constants;
using ImageGallery.Client.HttpClients;
using ImageGallery.Client.ViewModels.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        private readonly UserManagementHttpClient _userManagementClient;
        private readonly ILogger<UserProfileApiQueryController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileApiQueryController"/> class.
        /// </summary>
        /// <param name="userManagementClient"></param>
        /// <param name="logger"></param>
        public UserProfileApiQueryController(
            UserManagementHttpClient userManagementClient,
            ILogger<UserProfileApiQueryController> logger)
        {
            _userManagementClient = userManagementClient ?? throw new ArgumentNullException(nameof(userManagementClient));
            _logger = logger;
        }

        /// <summary>
        /// Get User Properties.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(UserProfileViewModel))]
        [ProducesResponseType(typeof(UserProfileViewModel), 200)]
        public async Task<IActionResult> Get()
        {
            var response = await _userManagementClient.Instance.GetAsync(InternalUserProfileRoute).ConfigureAwait(false);

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
