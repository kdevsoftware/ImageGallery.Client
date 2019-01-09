using System;
using System.Net;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Constants;
using ImageGallery.Client.HttpClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ImageGallery.Client.Apis
{
    /// <summary>
    ///
    /// </summary>
    [ApiController]
    [Route(ApiRoutes.AlbumsRoute)]
    [Authorize(Roles = "PayingUser, FreeUser")]
    public class AlbumPrimaryImageCommandController : BaseController
    {
        private const string InternalAlbumsRoute = "api/albums";

        private readonly ImageGalleryHttpClient _imageGalleryClient;

        private readonly ILogger<AlbumPrimaryImageCommandController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumPrimaryImageCommandController"/> class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="imageGalleryClient"></param>
        /// <param name="logger"></param>
        public AlbumPrimaryImageCommandController(ImageGalleryHttpClient imageGalleryClient, IOptions<ApplicationOptions> settings, ILogger<AlbumPrimaryImageCommandController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger;
            _imageGalleryClient = imageGalleryClient ?? throw new ArgumentNullException(nameof(imageGalleryClient));
        }

        /// <summary>
        /// Update Album Primary Image.
        /// </summary>
        /// <param name="id">Album Id.</param>
        /// <param name="imageId">Image Id.</param>
        /// <returns></returns>
        [HttpPut("primaryimage/{id}")]
        public async Task<IActionResult> Put(Guid id, [FromQuery] Guid imageId)
        {
            await WriteOutIdentityInformation();

            var route = $"{InternalAlbumsRoute}/primaryImage/{id}?imageId={imageId}";
            var response = await _imageGalleryClient.Instance.PutAsync(route, null).ConfigureAwait(false);

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
