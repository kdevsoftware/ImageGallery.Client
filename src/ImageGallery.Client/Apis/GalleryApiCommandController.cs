using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
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
    public class GalleryApiCommandController : BaseController
    {
        private const string InternalImagesRoute = "api/images";

        private readonly IImageGalleryHttpClient _imageGalleryHttpClient;

        private readonly ILogger<GalleryApiCommandController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GalleryApiCommandController"/> class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="imageGalleryHttpClient"></param>
        /// <param name="logger"></param>
        public GalleryApiCommandController(IOptions<ApplicationOptions> settings, IImageGalleryHttpClient imageGalleryHttpClient, ILogger<GalleryApiCommandController> logger)
        {
            _logger = logger;
            _imageGalleryHttpClient = imageGalleryHttpClient ?? throw new ArgumentNullException(nameof(imageGalleryHttpClient));
            ApplicationSettings = settings.Value;
        }

        private ApplicationOptions ApplicationSettings { get; }

        /// <summary>
        /// Edit Image.
        /// </summary>
        /// <param name="editImageViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("edit")]
        [Consumes("application/json")]
        [Authorize(Roles = "PayingUser")] /* TEST FREE USER VALIDATION */
        public async Task<IActionResult> EditImage([FromBody] EditImageViewModel editImageViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // create an ImageForUpdate instance
            var imageForUpdate = new ImageForUpdate
            {
                Title = editImageViewModel.Title,
                Category = editImageViewModel.Category,
            };

            // serialize it
            var serializedImageForUpdate = JsonConvert.SerializeObject(imageForUpdate);

            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.PutAsync(
                    $"{InternalImagesRoute}/{editImageViewModel.Id}",
                    new StringContent(serializedImageForUpdate, System.Text.Encoding.Unicode, "application/json"))
                .ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
                return Ok();

            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }

        /// <summary>
        /// Delete Image.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "PayingUser")] /* TEST FREE USER VALIDATION */
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            _logger.LogInformation($"Delete image by Id {id}");

            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.DeleteAsync($"{InternalImagesRoute}/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
                return Ok();

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return Unauthorized();

                case HttpStatusCode.Forbidden:
                    return new ForbidResult();
            }

            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }

        /// <summary>
        /// Add Image.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        [Authorize(Roles = "PayingUser")]
        public IActionResult AddImage()
        {
            return Ok();
        }

        /// <summary>
        /// Order Picture Frame.
        /// </summary>
        /// <param name="addImageViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("order")]
        [Authorize(Roles = "PayingUser")]
        public async Task<IActionResult> AddImage([FromForm] AddImageViewModel addImageViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // create an ImageForCreation instance
            var imageForCreation = new ImageForCreation
            {
                Title = addImageViewModel.Title,
                Category = addImageViewModel.Category,
            };

            // take the first (only) file in the Files list
            var imageFile = addImageViewModel.File;

            if (imageFile.Length > 0)
            {
                using (var fileStream = imageFile.OpenReadStream())
                using (var ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    imageForCreation.Bytes = ms.ToArray();
                }
            }

            // serialize it
            var serializedImageForCreation = JsonConvert.SerializeObject(imageForCreation);

            // call the API
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.PostAsync(
                    InternalImagesRoute,
                    new StringContent(serializedImageForCreation, Encoding.Unicode, "application/json"))
                .ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
                return Ok();

            throw new Exception($"A problem happened while calling the API: {response.ReasonPhrase}");
        }
    }
}
