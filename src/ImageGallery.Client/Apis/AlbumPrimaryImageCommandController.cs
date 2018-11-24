using System;
using System.Net;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Apis.Constants;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Services;
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
    [Route(AlbumRoutes.AlbumsRoute)]
    [Authorize(Roles = "PayingUser, FreeUser")]
    public class AlbumPrimaryImageCommandController : BaseController
    {
        private const string InternalAlbumsRoute = "api/albums";

        private readonly IImageGalleryHttpClient _imageGalleryHttpClient;

        private readonly ILogger<AlbumPrimaryImageCommandController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumPrimaryImageCommandController"/> class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="imageGalleryHttpClient"></param>
        /// <param name="logger"></param>
        public AlbumPrimaryImageCommandController(IImageGalleryHttpClient imageGalleryHttpClient, IOptions<ApplicationOptions> settings, ILogger<AlbumPrimaryImageCommandController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger;
            _imageGalleryHttpClient = imageGalleryHttpClient ?? throw new ArgumentNullException(nameof(imageGalleryHttpClient));
        }

        private ApplicationOptions ApplicationSettings { get; }

        /// <summary>
        /// Update Album Primary Image.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="imageId"></param>
        /// <returns></returns>
        [HttpPut("primaryimage/{id}")]
        public async Task<IActionResult> Put(Guid id, [FromQuery] Guid imageId)
        {
            await WriteOutIdentityInformation();

            var httpClient = await _imageGalleryHttpClient.GetClient();

            var route = $"{InternalAlbumsRoute}/primaryImage/{id}?imageId={imageId}";
            var response = await httpClient.PutAsync(route, null).ConfigureAwait(false);

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
