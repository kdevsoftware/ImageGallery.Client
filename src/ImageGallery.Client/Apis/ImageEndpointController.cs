using System;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Constants;
using ImageGallery.Client.HttpClients;
using ImageGallery.Client.ViewModels;
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
    public class ImageEndpointController : BaseController
    {
        private const string InternalImagesRoute = "api/images";

        private readonly ImageGalleryHttpClient _imageGalleryClient;

        private readonly ImageEndpointHttpClient _imageEndpoint;

        private readonly ILogger<ImageEndpointController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageEndpointController"/> class.
        /// </summary>
        /// <param name="imageGalleryClient"></param>
        /// <param name="imageEndpoint"></param>
        /// <param name="settings"></param>
        /// <param name="logger"></param>
        public ImageEndpointController(ImageGalleryHttpClient imageGalleryClient, ImageEndpointHttpClient imageEndpoint, IOptions<ApplicationOptions> settings, ILogger<ImageEndpointController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger;
            _imageGalleryClient = imageGalleryClient ?? throw new ArgumentNullException(nameof(imageGalleryClient));
            _imageEndpoint = imageEndpoint ?? throw new ArgumentNullException(nameof(imageEndpoint));
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
            var response = await _imageGalleryClient.Instance.GetAsync(imagesRoute).ConfigureAwait(false);

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
                var externalImagesRoute = $"{externalUri}{imageViewModel.FileName}";

                var result = await _imageEndpoint.Instance.GetAsync(externalImagesRoute);
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsByteArrayAsync();
                    var base64 = Convert.ToBase64String(content);

                    return Content("data:image/jpeg;base64," + base64);
                }
            }

            return UnprocessableEntity();
        }
    }
}
