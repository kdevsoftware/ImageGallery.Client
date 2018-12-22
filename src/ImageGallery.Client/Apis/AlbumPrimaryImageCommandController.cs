using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.Base;
using ImageGallery.Client.Apis.Constants;
using ImageGallery.Client.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ImageGallery.Client.Apis
{
    /// <summary>
    ///
    /// </summary>
    [ApiController]
    [Route(AlbumRoutes.AlbumsRoute)]
    [Authorize(Roles = "PayingUser, FreeUser")]
    public class AlbumPrimaryImageCommandController : BaseController
    {
        private const string InternalAlbumsRoute = "api/albums";

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ILogger<AlbumPrimaryImageCommandController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumPrimaryImageCommandController"/> class.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
        public AlbumPrimaryImageCommandController(IHttpClientFactory httpClientFactory, IOptions<ApplicationOptions> settings, ILogger<AlbumPrimaryImageCommandController> logger)
        {
            ApplicationSettings = settings.Value;
            _logger = logger;
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        private ApplicationOptions ApplicationSettings { get; }

        /// <summary>
        /// Update Album Primary Image.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="imageId"></param>
        /// <returns></returns>
        [HttpPut("primaryimage/{id}")]
        public async Task<IActionResult> Put(Guid id, [FromQuery] Guid imageId)
        {
            await WriteOutIdentityInformation();

            var httpClient = _httpClientFactory.CreateClient("imagegallery-api");

            var route = $"{InternalAlbumsRoute}/primaryImage/{id}?imageId={imageId}";
            var response = await httpClient.PutAsync(route, null).ConfigureAwait(false);

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
    }
}
