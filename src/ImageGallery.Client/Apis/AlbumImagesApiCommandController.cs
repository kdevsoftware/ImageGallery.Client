﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Apis.Constants;
using ImageGallery.Client.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ImageGallery.Client.Apis
{
    /// <summary>
    /// Album Images Api CommandController.
    /// </summary>
    [ApiController]
    [Route(AlbumRoutes.AlbumsRoute)]
    [Authorize(Roles = "PayingUser, FreeUser")]
    public class AlbumImagesApiCommandController : BaseController
    {
        private const string InternalAlbumsRoute = "api/albums";

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ILogger<AlbumImagesApiQueryController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumImagesApiCommandController"/> class.
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public AlbumImagesApiCommandController(IHttpClientFactory httpClientFactory, IOptions<ApplicationOptions> settings, ILogger<AlbumImagesApiQueryController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger;
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        private ApplicationOptions ApplicationSettings { get; }

        /// <summary>
        ///  Delete Image from Album.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="imageId"></param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        [HttpDelete("{id}/{imageId}")]
        public async Task<IActionResult> Delete(Guid id, Guid imageId)
        {
            // TODO Add Rule to Validate Image is Album Owner
            _logger.LogInformation($"Delete Album image  AlbumId:{id}|ImageId:{imageId}");

            // call the API
            var httpClient = _httpClientFactory.CreateClient("imagegallery-api");

            var requestUri = $"{InternalAlbumsRoute}/images/{id}?imageId={imageId}";
            var response = await httpClient.DeleteAsync(requestUri).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
                return Ok();

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
