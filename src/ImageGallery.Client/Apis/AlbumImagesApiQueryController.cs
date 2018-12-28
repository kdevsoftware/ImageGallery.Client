using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Apis.Constants;
using ImageGallery.Client.Configuration;
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
    /// Album Images Query Api.
    /// </summary>
    [ApiController]
    [Route(AlbumRoutes.AlbumsRoute)]
    [Authorize(Roles = "PayingUser, FreeUser")]
    public class AlbumImagesApiQueryController : BaseController
    {
        private const string InternalAlbumsRoute = "api/albums";

        private readonly ImageGalleryHttpClient _imageGalleryClient;

        private readonly ILogger<AlbumImagesApiQueryController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumImagesApiQueryController"/> class.
        /// </summary>
        /// <param name="imageGalleryClient"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public AlbumImagesApiQueryController(ImageGalleryHttpClient imageGalleryClient, IOptions<ApplicationOptions> settings, ILogger<AlbumImagesApiQueryController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger;
            _imageGalleryClient = imageGalleryClient ?? throw new ArgumentNullException(nameof(imageGalleryClient));
        }

        private ApplicationOptions ApplicationSettings { get; }

        /// <summary>
        /// Album Images List.
        /// </summary>
        /// <param name="id">Album Id.</param>
        /// <returns></returns>
        [HttpGet("images/list")]
        [Produces("application/json", Type = typeof(List<GalleryIndexViewModel>))]
        public async Task<IActionResult> GetAlbumImages([FromQuery] Guid id)
        {
            await WriteOutIdentityInformation();

            // call the API
            var route = $"{InternalAlbumsRoute}/images/{id}";

            var response = await _imageGalleryClient.Instance.GetAsync(route).ConfigureAwait(false);

            _logger.LogInformation($"Call {InternalAlbumsRoute} return {response.StatusCode}.");
            if (response.IsSuccessStatusCode)
            {
                var imagesAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var albumIndexViewModel = new AlbumImageIndexViewModel(
                    JsonConvert.DeserializeObject<IList<AlbumImage>>(imagesAsString).ToList(),
                    ApplicationSettings.ImagesUri);

                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-InlineCount");
                HttpContext.Response.Headers.Add("X-InlineCount", albumIndexViewModel.Images.Count().ToString());

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
        /// Album Images Paging and Filtering List.
        /// </summary>
        /// <param name="id">Album Id.</param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("images/list/{limit:int}/{page:int}")]
        [Produces("application/json", Type = typeof(List<GalleryIndexViewModel>))]
        public async Task<IActionResult> GetAlbumImagesPaging([FromQuery] Guid id, int limit, int page)
        {
            await WriteOutIdentityInformation();

            // call the API
            var route = $"{InternalAlbumsRoute}/images/{limit}/{page}?id={id}";

            var response = await _imageGalleryClient.Instance.GetAsync(route).ConfigureAwait(false);
            string inlinecount = response.Headers.GetValues("x-inlinecount").FirstOrDefault();

            _logger.LogInformation($"Call {InternalAlbumsRoute} return {response.StatusCode}.");
            if (response.IsSuccessStatusCode)
            {
                var imagesAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var albumIndexViewModel = new AlbumImageIndexViewModel(
                    JsonConvert.DeserializeObject<IList<AlbumImage>>(imagesAsString).ToList(),
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
    }
}
