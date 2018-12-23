using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Apis.Constants;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Filters;
using ImageGallery.Client.HttpClients;
using ImageGallery.Client.ViewModels;
using ImageGallery.Client.ViewModels.Album;
using ImageGallery.Model.Models.Albums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ImageGallery.Client.Apis
{
    /// <summary>
    ///
    /// </summary>
    [ApiController]
    [Route(AlbumRoutes.AlbumsRoute)]
    [Authorize(Roles = "PayingUser, FreeUser")]
    public class AlbumApiQueryController : BaseController
    {
        private const string InternalAlbumsRoute = "api/albums";

        private readonly ImageGalleryHttpClient _imageGalleryClient;

        private readonly ILogger<AlbumApiQueryController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumApiQueryController"/> class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="imageGalleryClient"></param>
        /// <param name="logger"></param>
        public AlbumApiQueryController(ImageGalleryHttpClient imageGalleryClient, IOptions<ApplicationOptions> settings, ILogger<AlbumApiQueryController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger;
            _imageGalleryClient = imageGalleryClient ?? throw new ArgumentNullException(nameof(imageGalleryClient));
        }

        private ApplicationOptions ApplicationSettings { get; }

        /// <summary>
        /// Get Albums.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(IEnumerable<AlbumIndexViewModel>))]
        [ProducesResponseType(typeof(IEnumerable<AlbumIndexViewModel>), 200)]
        public async Task<IActionResult> AlbumIndexViewModel()
        {
            await WriteOutIdentityInformation();

            var response = await _imageGalleryClient.Instance.GetAsync(InternalAlbumsRoute).ConfigureAwait(false);

            _logger.LogInformation($"Call {InternalAlbumsRoute} return {response.StatusCode}.");

            if (response.IsSuccessStatusCode)
            {
                var albumsAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var albumIndexViewModel = new AlbumIndexViewModel(
                    JsonConvert.DeserializeObject<IList<Album>>(albumsAsString).ToList(),
                    ApplicationSettings.ImagesUri);

                return Ok(albumIndexViewModel);
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

        /// <summary>
        /// Get Albums Paging.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("list")]
        [Produces("application/json", Type = typeof(IEnumerable<GalleryIndexViewModel>))]
        [ProducesResponseType(typeof(IEnumerable<GalleryIndexViewModel>), 200)]
        public async Task<IActionResult> Get([FromQuery] AlbumRequestModel query, int limit, int page)
        {
            await WriteOutIdentityInformation();

            // call the API
            var route = $"{InternalAlbumsRoute}/{limit}/{page}";
            var response = await _imageGalleryClient.Instance.GetAsync(route).ConfigureAwait(false);
            string inlinecount = response.Headers.GetValues("x-inlinecount").FirstOrDefault();

            _logger.LogInformation($"Call {InternalAlbumsRoute} return {response.StatusCode}.");

            if (response.IsSuccessStatusCode)
            {
                var albumsAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var albumIndexViewModel = new AlbumIndexViewModel(
                    JsonConvert.DeserializeObject<IList<Album>>(albumsAsString).ToList(),
                    ApplicationSettings.ImagesUri);

                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-InlineCount");
                HttpContext.Response.Headers.Add("X-InlineCount", inlinecount);

                return Ok(albumIndexViewModel);
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

        /// <summary>
        /// Get Album.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Produces("application/json", Type = typeof(IEnumerable<AlbumViewModel>))]
        [ProducesResponseType(typeof(IEnumerable<AlbumViewModel>), 200)]
        public async Task<IActionResult> GetAlbum(Guid id)
        {
            var albumsRoute = $"{InternalAlbumsRoute}/{id}";
            var response = await _imageGalleryClient.Instance.GetAsync(albumsRoute).ConfigureAwait(false);

            _logger.LogInformation($"Call {albumsRoute} return {response.StatusCode}.");

            if (response.IsSuccessStatusCode)
            {
                var albumAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var deserializedImage = JsonConvert.DeserializeObject<Album>(albumAsString);

                var albumViewModel = new AlbumViewModel
                {
                    Id = deserializedImage.Id,
                    Title = deserializedImage.Title,
                    Description = deserializedImage.Description,
                    DateCreated = deserializedImage.DateCreated,
                };

                return Ok(albumViewModel);
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
