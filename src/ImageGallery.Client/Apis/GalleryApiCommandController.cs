using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Apis.Constants;
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
    [Route(GalleryRoutes.GalleryRoute)]
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
        public GalleryApiCommandController(IImageGalleryHttpClient imageGalleryHttpClient, IOptions<ApplicationOptions> settings, ILogger<GalleryApiCommandController> logger)
        {
            _logger = logger;
            _imageGalleryHttpClient = imageGalleryHttpClient ?? throw new ArgumentNullException(nameof(imageGalleryHttpClient));
            ApplicationSettings = settings.Value;
        }

        private ApplicationOptions ApplicationSettings { get; }

        /// <summary>
        /// Edit Image Properties.
        /// </summary>
        /// <param name="editImageViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("edit")]
        [Consumes("application/json")]
        [Authorize(Roles = "PayingUser")] /* TEST FREE USER VALIDATION */
        public async Task<IActionResult> EditImagePropeties([FromBody] EditImageViewModel editImageViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // create an ImageForUpdate instance
            var imageForUpdate = new ImageProperitesUpdate
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
                    new StringContent(serializedImageForUpdate, Encoding.Unicode, "application/json"))
                .ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
                return Ok();

            return UnprocessableEntity(response.ReasonPhrase);
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

            return UnprocessableEntity(response.ReasonPhrase);
        }

        /// <summary>
        /// Patch Image Name/Value.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDtos"></param>
        /// <returns></returns>
        [HttpPatch("{id}", Name = "PatchImage")]
        public async Task<IActionResult> PatchImage(Guid id, [FromBody] List<PatchDto> patchDtos)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            // call the API
            var imagesRoute = $"{InternalImagesRoute}/{id}";
            var httpClient = await _imageGalleryHttpClient.GetClient();

            var response = await httpClient.GetAsync(imagesRoute).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var imageAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var deserializedImage = JsonConvert.DeserializeObject<ImageProperitesUpdate>(imageAsString);

                var result = ApplyPatch(deserializedImage, patchDtos);
                var serializedImageForUpdate = JsonConvert.SerializeObject(result);
                var responsePut = await httpClient.PutAsync(
                        $"{InternalImagesRoute}/{id}",
                        new StringContent(serializedImageForUpdate, Encoding.Unicode, "application/json"))
                    .ConfigureAwait(false);

                if (responsePut.IsSuccessStatusCode)
                    return Ok();
            }

            return UnprocessableEntity(response.ReasonPhrase);
        }

        /// <summary>
        /// Add Image.
        /// </summary>
        /// <param name="addImageViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
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

            return UnprocessableEntity(response.ReasonPhrase);
        }

        /// <summary>
        /// Update Image.
        /// </summary>
        /// <param name="updateImageViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        [Authorize(Roles = "PayingUser")]
        public async Task<IActionResult> UpdateImage([FromForm] UpdateImageViewModel updateImageViewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await Task.Delay(1);

            return Ok();
        }
    }
}
