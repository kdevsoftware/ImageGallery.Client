using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Services;
using ImageGallery.Client.ViewModels.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ImageGallery.Client.Apis.UserManagement
{
    [Authorize]
    [Route(UserManagementRoutes.UserProfile)]
    public class UserProfileApiCommandController : Controller
    {
        private const string InternalUserProfileRoute = "api/UserProfile";

        private readonly IOptions<ApplicationOptions> _settings;
        private readonly IImageGalleryHttpClient _imageGalleryHttpClient;
        private readonly ILogger<UserProfileApiCommandController> _logger;

        public UserProfileApiCommandController(
            IOptions<ApplicationOptions> settings,
            IImageGalleryHttpClient imageGalleryHttpClient,
            ILogger<UserProfileApiCommandController> logger)
        {
            _settings = settings;
            _imageGalleryHttpClient = imageGalleryHttpClient;
            _logger = logger;
        }

        [HttpPut]
        [ProducesResponseType(typeof(UserProfileUpdateViewModel), 200)]
        public async Task<IActionResult> Put([FromBody] [Required] UserProfileUpdateViewModel model)
        {
            var httpClient =
                await _imageGalleryHttpClient.GetClient(_settings.Value.ClientConfiguration.ApiUserManagementUri);

            var serializedUserProfileForUpdate = JsonConvert.SerializeObject(model);

            var response = await httpClient.PutAsync(
                    $"{InternalUserProfileRoute}",
                    new StringContent(serializedUserProfileForUpdate, Encoding.Unicode, "application/json"))
                .ConfigureAwait(false);

            _logger.LogInformation($"Call {InternalUserProfileRoute} return {response.StatusCode}.");

            if (response.IsSuccessStatusCode)
            {
                var profileAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var userProfileViewModel = JsonConvert.DeserializeObject<UserProfileUpdateViewModel>(profileAsString);

                return Ok(userProfileViewModel);
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return Unauthorized();

                case HttpStatusCode.Forbidden:
                    return new ForbidResult();
            }

            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }
    }
}
