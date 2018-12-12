using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Apis.Constants;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Filters;
using ImageGallery.Client.Services;
using ImageGallery.Client.ViewModels;
using ImageGallery.Model.Models.Images;
using ImageGallery.Service.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace ImageGallery.Client.Apis
{
    /// <summary>
    ///
    /// </summary>
    [ApiController]
    [Route(GalleryRoutes.GalleryRoute)]
    public class GalleryApiQueryController : BaseController
    {
        private const string InternalImagesRoute = "api/images";

        private readonly IImageGalleryHttpClient _imageGalleryHttpClient;

        private readonly ILogger<GalleryApiQueryController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GalleryApiQueryController"/> class.
        /// </summary>
        /// <param name="imageGalleryHttpClient"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public GalleryApiQueryController(IImageGalleryHttpClient imageGalleryHttpClient,
            IOptions<ApplicationOptions> settings, ILogger<GalleryApiQueryController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger;
            _imageGalleryHttpClient =
                imageGalleryHttpClient ?? throw new ArgumentNullException(nameof(imageGalleryHttpClient));
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

            return UnprocessableEntity(response.ReasonPhrase);
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

            var queryFilter = query.ToQueryString();
            var route = $"{InternalImagesRoute}/{limit}/{page}{queryFilter}";

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

            return UnprocessableEntity(response.ReasonPhrase);
        }

        /// <summary>
        /// Get Image Properties.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        [Produces("application/json", Type = typeof(IEnumerable<ImageViewModel>))]
        [ProducesResponseType(typeof(IEnumerable<ImageViewModel>), 200)]
        public async Task<IActionResult> GetImageProperties(Guid id)
        {
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

            return UnprocessableEntity(response.ReasonPhrase);
        }

        /// <summary>
        ///  Get Image File.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [Produces("image/jpeg")]
        [HttpGet("file/{id}")]
        public async Task<IActionResult> GetImageFile(Guid id)
        {
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
                    FileName = deserializedImage.FileName,
                };

                var externalUri = ApplicationSettings.ImagesUri;
                var externalimagesRoute = $"{externalUri}{imageViewModel.FileName}";

                using (var client = new HttpClient())
                {
                    using (var result = await client.GetAsync(externalimagesRoute))
                    {
                        if (result.IsSuccessStatusCode)
                        {
                            var content = await result.Content.ReadAsByteArrayAsync();
                            FileContentResult fileResult = new FileContentResult(content, MediaTypeNames.Image.Jpeg)
                            {
                                FileDownloadName = imageViewModel.FileName,
                                EnableRangeProcessing = true,
                            };

                            return fileResult;
                        }
                    }
                }
            }

            return UnprocessableEntity();
        }

        /// <summary>
        ///  Get Image File(Base64).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [Produces("text/plain")]
        [HttpGet("text/{id}")]
        public async Task<IActionResult> GetImageBase64File(Guid id)
        {
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
                    FileName = deserializedImage.FileName,
                };

                var externalUri = ApplicationSettings.ImagesUri;
                var externalimagesRoute = $"{externalUri}{imageViewModel.FileName}";

                using (var client = new HttpClient())
                {
                    using (var result = await client.GetAsync(externalimagesRoute))
                    {
                        if (result.IsSuccessStatusCode)
                        {
                            var content = await result.Content.ReadAsByteArrayAsync();
                            var base64 = Convert.ToBase64String(content);

                            return Content("data:image/jpeg;base64," + base64);
                        }
                    }
                }
            }

            return UnprocessableEntity();
            //return "data:image/jpeg;base64," + string.Empty;
            //return UnprocessableEntity(response.ReasonPhrase);
        }
    }
}
