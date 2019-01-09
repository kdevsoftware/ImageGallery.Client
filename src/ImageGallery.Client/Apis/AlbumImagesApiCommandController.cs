using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Constants;
using ImageGallery.Client.HttpClients;
using ImageGallery.Client.ViewModels.Album;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ImageGallery.Client.Apis
{
    /// <summary>
    /// Album Images Api CommandController.
    /// </summary>
    [ApiController]
    [Route(ApiRoutes.AlbumsRoute)]
    [Authorize(Roles = "PayingUser, FreeUser")]
    public class AlbumImagesApiCommandController : BaseController
    {
        private const string InternalAlbumsRoute = "api/albums";

        private readonly ImageGalleryHttpClient _imageGalleryClient;

        private readonly ILogger<AlbumImagesApiCommandController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumImagesApiCommandController"/> class.
        /// </summary>
        /// <param name="imageGalleryClient"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public AlbumImagesApiCommandController(ImageGalleryHttpClient imageGalleryClient, IOptions<ApplicationOptions> settings, ILogger<AlbumImagesApiCommandController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger;
            _imageGalleryClient = imageGalleryClient ?? throw new ArgumentNullException(nameof(imageGalleryClient));
        }

        /// <summary>
        ///  Delete Image from Album.
        /// </summary>
        /// <param name="id">Album Id.</param>
        /// <param name="imageId">Image Id.</param>
        /// <exception cref="Exception"></exception>
        /// <returns></returns>
        [HttpDelete("{id}/{imageId}")]
        public async Task<IActionResult> Delete(Guid id, Guid imageId)
        {
            // TODO Add Rule to Validate Image is Album Owner
            await WriteOutIdentityInformation();
            _logger.LogInformation($"Delete Album image  AlbumId:{id}|ImageId:{imageId}");

            // call the API
            var requestUri = $"{InternalAlbumsRoute}/images/{id}?imageId={imageId}";
            var response = await _imageGalleryClient.Instance.DeleteAsync(requestUri).ConfigureAwait(false);

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

        /// <summary>
        ///  Update Album Image Sort List.
        /// </summary>
        /// <param name="id">Album Id.</param>
        /// <param name="items">Image Item Sorted List.</param>
        /// <returns></returns>
        [HttpPut("{id}/sort")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateAlbumSort(Guid id, [FromBody] List<AlbumImageSortItem> items)
        {
            await WriteOutIdentityInformation();

            var serializeAlbumImagesForUpdate = JsonConvert.SerializeObject(items);

            var route = $"{InternalAlbumsRoute}/images/{id}";
            var response = await _imageGalleryClient.Instance.PutAsync(
                    route,
                    new StringContent(serializeAlbumImagesForUpdate, Encoding.Unicode, "application/json"))
                .ConfigureAwait(false);

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
