using System;
using System.Threading.Tasks;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Services;
using ImageGallery.Client.ViewModels.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ImageGallery.Client.Apis
{
    [Authorize]
    [Route("api/UserProfile")]
    public class UserProfileApiQueryController : Controller
    {
        private const string InternalUserProfileRoute = "api/UserProfile";

        private readonly IOptions<ApplicationOptions> _settings;
        private readonly IImageGalleryHttpClient _imageGalleryHttpClient;
        private readonly ILogger<UserProfileApiQueryController> _logger;

        public UserProfileApiQueryController(
            IOptions<ApplicationOptions> settings,
            IImageGalleryHttpClient imageGalleryHttpClient,
            ILogger<UserProfileApiQueryController> logger)
        {
            _settings = settings;
            _imageGalleryHttpClient = imageGalleryHttpClient;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserProfileViewModel), 200)]
        public async Task<IActionResult> Get()
        {
            var httpClient = await _imageGalleryHttpClient.GetClient(_settings.Value.UserManagementApiConfiguration.ApiUri);

            var response = await httpClient.GetAsync(InternalUserProfileRoute).ConfigureAwait(false);

            _logger.LogInformation($"Call {InternalUserProfileRoute} return {response.StatusCode}.");

            if (response.IsSuccessStatusCode)
            {
                var imagesAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var userProfileViewModel = JsonConvert.DeserializeObject<UserProfileViewModel>(imagesAsString);

                return Ok(userProfileViewModel);
            }

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.Unauthorized:
                    return Unauthorized();

                case System.Net.HttpStatusCode.Forbidden:
                    return new ForbidResult();
            }

            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }
    }
}