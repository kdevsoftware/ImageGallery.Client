using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Apis.Constants;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.HttpClients;
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
        /// <param name="tag">Tag Name.</param>
        [HttpPost("metadata/tags")]
        [Consumes("application/json")]
        public void CreateTag(Guid id, [FromBody]string tag)
        {

        }

        /// <summary>
        ///  Delte Album Tag.
        /// </summary>
        /// <param name="id">Album Id.</param>
        /// <param name="tagId">Tag Id.</param>
        [HttpDelete("metadata/tags/{id}")]
        public void DeleteTag(Guid id, Guid tagId)
        {

        }

    }
}
