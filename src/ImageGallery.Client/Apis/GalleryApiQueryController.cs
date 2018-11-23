using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Services;
using ImageGallery.Client.ViewModels;
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
    [Route("api/images")]
    public class GalleryApiQueryController : BaseController
    {
        private const string InternalImagesRoute = "api/images";

        private readonly IImageGalleryHttpClient _imageGalleryHttpClient;

        private readonly ILogger<GalleryApiQueryController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GalleryApiQueryController"/> class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="imageGalleryHttpClient"></param>
        /// <param name="logger"></param>
        public GalleryApiQueryController(IOptions<ApplicationOptions> settings, IImageGalleryHttpClient imageGalleryHttpClient, ILogger<GalleryApiQueryController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger;
            _imageGalleryHttpClient = imageGalleryHttpClient ?? throw new ArgumentNullException(nameof(imageGalleryHttpClient));
        }

        private ApplicationOptions ApplicationSettings { get; }

        /// <summary>
        /// Get Images.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "PayingUser, FreeUser")]
        [HttpGet]
        [Produces("application/json", Type = typeof(IEnumerable<GalleryIndexViewModel>))]
        [ProducesResponseType(typeof(IEnumerable<GalleryIndexViewModel>), 200)]
        public async Task<IActionResult> GalleryIndexViewModel()
        {
            await WriteOutIdentityInformation();

            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();
            var response = await httpClient.GetAsync(InternalImagesRoute).ConfigureAwait(false);

            _logger.LogInformation($"Call {InternalImagesRoute} return {response.StatusCode}.");

            if (response.IsSuccessStatusCode)
            {
                var imagesAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var galleryIndexViewModel = new GalleryIndexViewModel(
                        JsonConvert.DeserializeObject<IList<Image>>(imagesAsString).ToList(),
                        ApplicationSettings.ImagesUri);

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

        /// <summary>
        /// Get Images Paging.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [Authorize(Roles = "PayingUser, FreeUser")]
        [HttpGet]
        [Route("list")]
        [Produces("application/json", Type = typeof(IEnumerable<GalleryIndexViewModel>))]
        [ProducesResponseType(typeof(IEnumerable<GalleryIndexViewModel>), 200)]
        public async Task<IActionResult> Get([FromQuery] GalleryRequestModel query, int limit, int page)
        {
            await WriteOutIdentityInformation();

            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var route = $"{InternalImagesRoute}/{limit}/{page}";
            var response = await httpClient.GetAsync(route).ConfigureAwait(false);
            string inlinecount = response.Headers.GetValues("x-inlinecount").FirstOrDefault();

            _logger.LogInformation($"Call {InternalImagesRoute} return {response.StatusCode}.");

            if (response.IsSuccessStatusCode)
            {
                var imagesAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var galleryIndexViewModel = new GalleryIndexViewModel(
                        JsonConvert.DeserializeObject<IList<Image>>(imagesAsString).ToList(),
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

        /// <summary>
        /// Get Image Properties 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        [Produces("application/json", Type = typeof(IEnumerable<ImageViewModel>))]
        [ProducesResponseType(typeof(IEnumerable<ImageViewModel>), 200)]
        public async Task<IActionResult> GetImageProperties(Guid id)
        {
            // call the API
            var imagesRoute = $"{InternalImagesRoute}/{id}";
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.GetAsync(imagesRoute).ConfigureAwait(false);

            _logger.LogInformation($"Call {imagesRoute} return {response.StatusCode}.");

            if (response.IsSuccessStatusCode)
            {
                var imageAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var deserializedImage = JsonConvert.DeserializeObject<Image>(imageAsString);

                var imageViewModel = new ImageViewModel(ApplicationSettings.ImagesUri)
                {
                    Id = deserializedImage.Id,
                    Title = deserializedImage.Title,
                    Category = deserializedImage.Category,
                    FileName = deserializedImage.FileName,
                    Height = deserializedImage.Height,
                    Width = deserializedImage.Width,
                };

                return Ok(imageViewModel);
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
