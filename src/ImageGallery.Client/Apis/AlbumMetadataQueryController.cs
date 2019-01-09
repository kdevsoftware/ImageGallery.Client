using System;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Constants;
using ImageGallery.Client.HttpClients;
using ImageGallery.Model.Models.Albums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ImageGallery.Client.Apis
{
    /// <summary>
    ///  Album Metadata Controller.
    /// </summary>
    [Route(ApiRoutes.AlbumsRoute)]
    [Authorize(Roles = "PayingUser, FreeUser")]
    [ApiController]
    public class AlbumMetadataQueryController : BaseController
    {
        private const string InternalAlbumsRoute = "api/albums";

        private readonly ImageGalleryHttpClient _imageGalleryClient;

        private readonly ILogger<AlbumMetadataQueryController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumMetadataQueryController"/> class.
        /// </summary>
        /// <param name="imageGalleryClient"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public AlbumMetadataQueryController(ImageGalleryHttpClient imageGalleryClient, IOptions<ApplicationOptions> settings, ILogger<AlbumMetadataQueryController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger;
            _imageGalleryClient = imageGalleryClient ?? throw new ArgumentNullException(nameof(imageGalleryClient));
        }

        /// <summary>
        /// Get Album Metadata.
        /// </summary>
        /// <param name="id">Album Id.</param>
        /// <returns></returns>
        [HttpGet("metadata")]
        [Produces("application/json", Type = typeof(AlbumMetaData))]
        public async Task<IActionResult> GetAlbumMetadata(Guid id)
        {
            var route = $"{InternalAlbumsRoute}/{id}/metadata";
            var response = await _imageGalleryClient.Instance.GetAsync(route).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                var albumMetadataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var deserializedAlbum = JsonConvert.DeserializeObject<AlbumMetaData>(albumMetadataAsString);

                return Ok(deserializedAlbum);
            }

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.Unauthorized:
                    return Unauthorized();

                case System.Net.HttpStatusCode.Forbidden:
                    return new ForbidResult();
            }

            return UnprocessableEntity(response.ReasonPhrase);
        }
    }
}
