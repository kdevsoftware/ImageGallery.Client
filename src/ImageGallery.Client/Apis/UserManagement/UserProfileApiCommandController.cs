﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Constants;
using ImageGallery.Client.HttpClients;
using ImageGallery.Client.ViewModels.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ImageGallery.Client.Apis.UserManagement
{
    /// <summary>
    /// User Properties.
    /// </summary>
    [Authorize]
    [Route(ApiRoutes.UserProfileRoute)]
    public class UserProfileApiCommandController : BaseController
    {
        private const string InternalUserProfileRoute = "api/UserProfile";

        private readonly UserManagementHttpClient _userManagementClient;
        private readonly ILogger<UserProfileApiCommandController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileApiCommandController"/> class.
        /// </summary>
        /// <param name="userManagementClient"></param>
        /// <param name="logger"></param>
        public UserProfileApiCommandController(
            UserManagementHttpClient userManagementClient,
            ILogger<UserProfileApiCommandController> logger)
        {
            _userManagementClient = userManagementClient ?? throw new ArgumentNullException(nameof(userManagementClient));
            _logger = logger;
        }

        /// <summary>
        /// Update User Properties.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(UserProfileUpdateViewModel), 200)]
        public async Task<IActionResult> Put([FromBody] [Required] UserProfileUpdateViewModel model)
        {
            var serializedUserProfileForUpdate = JsonConvert.SerializeObject(model);

            var response = await _userManagementClient.Instance.PutAsync(
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

            return UnprocessableEntity(response.ReasonPhrase);
        }
    }
}
