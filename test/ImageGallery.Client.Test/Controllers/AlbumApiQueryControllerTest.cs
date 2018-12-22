using ImageGallery.Client.Apis;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Filters;
using ImageGallery.Client.HttpClients;
using ImageGallery.Client.Test.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.Client.Test.Controllers
{
    public class AlbumApiQueryControllerTest
    {
        private readonly ITestOutputHelper _output;

        public AlbumApiQueryControllerTest(ITestOutputHelper output)
        {
            this._output = output;
        }

        //[Fact]
        [Fact(Skip = "TODO")]
        public async void GetAlbums_ReturnsData()
        {
            var albumController = GetAlbumApiQueryController(null, null, null);
            albumController.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            var query = new AlbumRequestModel { };

            var result = await albumController.Get(query, 1, 1);

            Assert.IsType<AlbumApiQueryController>(albumController);
            Assert.True(false);
        }

        private AlbumApiQueryController GetAlbumApiQueryController(
            ImageGalleryHttpClient httpClientFactory = null,
            IOptions<ApplicationOptions> settings = null,
            ILogger<AlbumApiQueryController> logger = null)
        {
            httpClientFactory = httpClientFactory ?? new Mock<ImageGalleryHttpClient>().Object;
            settings = settings ?? new Mock<IOptions<ApplicationOptions>>().Object;
            logger = logger ?? new Mock<ILogger<AlbumApiQueryController>>().Object;

            return new AlbumApiQueryController(httpClientFactory, settings, logger);
        }
    }
}
