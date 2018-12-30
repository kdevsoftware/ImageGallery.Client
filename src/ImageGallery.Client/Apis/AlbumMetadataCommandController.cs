using System;
using System.Net;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Apis.Constants;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.HttpClients;
using ImageGallery.Client.ViewModels.Album;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ImageGallery.Client.Apis
{
    /// <summary>
    ///  Album Metadata Command Controller.
    /// </summary>
    [Route(AlbumRoutes.AlbumsRoute)]
    [Authorize(Roles = "PayingUser, FreeUser")]
    [ApiController]
    public class AlbumMetadataCommandController : BaseController
    {
        private const string InternalAlbumsRoute = "api/albums";

        private readonly ImageGalleryHttpClient _imageGalleryClient;

        private readonly ILogger<AlbumMetadataCommandController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumMetadataCommandController"/> class.
        /// </summary>
        /// <param name="imageGalleryClient"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public AlbumMetadataCommandController(ImageGalleryHttpClient imageGalleryClient, IOptions<ApplicationOptions> settings, ILogger<AlbumMetadataCommandController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger;
            _imageGalleryClient = imageGalleryClient ?? throw new ArgumentNullException(nameof(imageGalleryClient));
        }

        private ApplicationOptions ApplicationSettings { get; }

        /// <summary>
        ///  Create Album Tag.
        /// </summary>
        /// <param name="id">Album Id.</param>
        /// <param name="model">Album Tag ViewModel.</param>
        /// <returns></returns>
        [HttpPost("metadata/tags")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateTag(Guid id, [FromBody]AlbumTagViewModel model)
        {
            await WriteOutIdentityInformation();

            var route = $"{InternalAlbumsRoute}/{id}/tags?tag={model.Tag}";
            var response = await _imageGalleryClient.Instance.PostAsync(route, null).ConfigureAwait(false);

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
        ///  Delte Album Tag.
        /// </summary>
        /// <param name="id">Album Id.</param>
        /// <param name="tagId">Tag Id.</param>
        /// <returns></returns>
        [HttpDelete("metadata/tags/{id}")]
        public async Task<IActionResult> DeleteTag(Guid id, Guid tagId)
        {
            _logger.LogInformation($"Delete image by Id {id}");

            var response = await _imageGalleryClient.Instance
                .DeleteAsync($"{InternalAlbumsRoute}/{id}/tags/{tagId}").ConfigureAwait(false);

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
