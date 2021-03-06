﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Constants;
using ImageGallery.Client.Filters;
using ImageGallery.Client.Helpers;
using ImageGallery.Client.HttpClients;
using ImageGallery.Client.ViewModels;
using ImageGallery.Client.ViewModels.Gallery;
using ImageGallery.Model.Models.Images;
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
    [Route(ApiRoutes.GalleryRoute)]
    public class GalleryApiQueryController : BaseController
    {
        private const string InternalImagesRoute = "api/images";

        private readonly ImageGalleryHttpClient _imageGalleryClient;

        private readonly ILogger<GalleryApiQueryController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GalleryApiQueryController"/> class.
        /// </summary>
        /// <param name="imageGalleryClient"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public GalleryApiQueryController(ImageGalleryHttpClient imageGalleryClient, IOptions<ApplicationOptions> settings, ILogger<GalleryApiQueryController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger;
            _imageGalleryClient = imageGalleryClient ?? throw new ArgumentNullException(nameof(imageGalleryClient));
        }

        /// <summary>
        /// Get Images.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "PayingUser, FreeUser")]
        [HttpGet]
        [Produces("application/json", Type = typeof(GalleryIndexViewModel))]
        [ProducesResponseType(typeof(GalleryIndexViewModel), 200)]
        public async Task<IActionResult> GalleryIndexViewModel()
        {
            await WriteOutIdentityInformation();

            // call the API
            var response = await _imageGalleryClient.Instance.GetAsync(InternalImagesRoute).ConfigureAwait(false);

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
        [Produces("application/json", Type = typeof(GalleryIndexViewModel))]
        [ProducesResponseType(typeof(IEnumerable<GalleryIndexViewModel>), 200)]
        public async Task<IActionResult> Get([FromQuery] GalleryRequestModel query, int limit, int page)
        {
            await WriteOutIdentityInformation();

            // call the API
            var queryFilter = query.ToQueryString();
            var route = $"{InternalImagesRoute}/{limit}/{page}{queryFilter}";

            var response = await _imageGalleryClient.Instance.GetAsync(route).ConfigureAwait(false);
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
        /// <param name="id">Image Id.</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        [Produces("application/json", Type = typeof(ImageViewModel))]
        [ProducesResponseType(typeof(ImageViewModel), 200)]
        public async Task<IActionResult> GetImageProperties(Guid id)
        {
            var imagesRoute = $"{InternalImagesRoute}/{id}";
            var response = await _imageGalleryClient.Instance.GetAsync(imagesRoute).ConfigureAwait(false);

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
    }
}
