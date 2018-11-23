using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Services;
using ImageGallery.Client.ViewModels;
using ImageGallery.Client.ViewModels.Album;
using ImageGallery.Model;
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
    [Route("api/albums")]
    [Authorize(Roles = "PayingUser, FreeUser")]
    public class AlbumImagesApiQueryController : BaseController
    {
        private const string InternalAlbumsRoute = "api/albums";

        private readonly IImageGalleryHttpClient _imageGalleryHttpClient;

        private readonly ILogger<AlbumImagesApiQueryController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumImagesApiQueryController"/> class.
        /// </summary>
        /// <param name="imageGalleryHttpClient"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public AlbumImagesApiQueryController(IImageGalleryHttpClient imageGalleryHttpClient, IOptions<ApplicationOptions> settings, ILogger<AlbumImagesApiQueryController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger;
            _imageGalleryHttpClient = imageGalleryHttpClient ?? throw new ArgumentNullException(nameof(imageGalleryHttpClient));
        }

        private ApplicationOptions ApplicationSettings { get; }

        /// <summary>
        /// Album Images Paging and Filtering List.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet("images/list/{limit:int}/{page:int}")]
        [Produces("application/json", Type = typeof(List<GalleryIndexViewModel>))]
        public async Task<IActionResult> GetAlbumImagesPaging([FromQuery] Guid id, int limit, int page)
        {
            await WriteOutIdentityInformation();

            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();
            var route = $"{InternalAlbumsRoute}/images/{limit}/{page}?id={id}";

            var response = await httpClient.GetAsync(route).ConfigureAwait(false);
            string inlinecount = response.Headers.GetValues("x-inlinecount").FirstOrDefault();

            _logger.LogInformation($"Call {InternalAlbumsRoute} return {response.StatusCode}.");
            if (response.IsSuccessStatusCode)
            {
                var imagesAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var galleryIndexViewModel = new AlbumImageIndexViewModel(
                    JsonConvert.DeserializeObject<IList<AlbumImage>>(imagesAsString).ToList(),
                    ApplicationSettings.ImagesUri);

                HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "X-InlineCount");
                HttpContext.Response.Headers.Add("X-InlineCount", inlinecount);

                return Ok(galleryIndexViewModel);
            }

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.Unauthorized:
                    return Unauthorized();

                case System.Net.HttpStatusCode.Forbidden:
                    return new ForbidResult();
            }

            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }
    }
}