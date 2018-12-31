using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Apis.Constants;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.HttpClients;
using ImageGallery.Client.ValidationRules.Constants;
using ImageGallery.Client.ViewModels;
using ImageGallery.Model.Models.Albums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ImageGallery.Client.Apis
{
    /// <summary>
    /// Album Api CommandController.
    /// </summary>
    [Route(AlbumRoutes.AlbumsRoute)]
    [Authorize(Roles = "PayingUser, FreeUser")]
    [ApiController]
    public class AlbumApiCommandController : BaseController
    {
        private const string InternalAlbumsRoute = "api/albums";

        private readonly ImageGalleryHttpClient _imageGalleryClient;

        private readonly ILogger<AlbumApiCommandController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumApiCommandController"/> class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="imageGalleryClient"></param>
        /// <param name="logger"></param>
        public AlbumApiCommandController(ImageGalleryHttpClient imageGalleryClient, IOptions<ApplicationOptions> settings, ILogger<AlbumApiCommandController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger;
            _imageGalleryClient = imageGalleryClient ?? throw new ArgumentNullException(nameof(imageGalleryClient));
        }

        /// <summary>
        /// Patch Album Name/Value.
        /// </summary>
        /// <param name="id">Album Id.</param>
        /// <param name="patchDtos">Property Name/Value Pair.</param>
        /// <returns></returns>
        [HttpPatch("{id}", Name = "PatchAlbum")]
        public async Task<IActionResult> PatchAlbum(Guid id, [FromBody] List<PatchDto> patchDtos)
        {
            if (id == Guid.Empty)
            {
                ModelState.AddModelError("id", ValidationConstants.InvalidGuidMessage);
                return BadRequest(ModelState);
            }

            // call the API
            var albumRoute = $"{InternalAlbumsRoute}/{id}";

            var response = await _imageGalleryClient.Instance.GetAsync(albumRoute).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var imageAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var deserializedAlbum = JsonConvert.DeserializeObject<AlbumProperitesUpdate>(imageAsString);

                var result = ApplyPatch(deserializedAlbum, patchDtos);
                var serializedAlbumForUpdate = JsonConvert.SerializeObject(result);
                var responsePut = await _imageGalleryClient.Instance.PutAsync(
                        $"{InternalAlbumsRoute}/{id}",
                        new StringContent(serializedAlbumForUpdate, Encoding.Unicode, "application/json"))
                    .ConfigureAwait(false);

                if (responsePut.IsSuccessStatusCode)
                    return Ok();
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

        /// <summary>
        /// Delete Album.
        /// </summary>
        /// <param name="id">Album Id.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbum(Guid id)
        {
            _logger.LogInformation($"Delete image by Id {id}");

            // call the API
            var response = await _imageGalleryClient.Instance.DeleteAsync($"{InternalAlbumsRoute}/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
                return Ok();

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
