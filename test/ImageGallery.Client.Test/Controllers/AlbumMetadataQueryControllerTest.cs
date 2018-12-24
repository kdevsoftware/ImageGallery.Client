using ImageGallery.Client.Apis;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.HttpClients;
using ImageGallery.Client.Test.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.Client.Test.Controllers
{
    public class AlbumMetadataQueryControllerTest
    {
        private readonly ITestOutputHelper _output;

        public AlbumMetadataQueryControllerTest(ITestOutputHelper output)
        {
            this._output = output;
        }

        //[Fact]
        [Fact(Skip = "TODO")]
        public async void GetAlbum_Metadata_ReturnsData()
        {
            var mockHttpClient = new Mock<ImageGalleryHttpClient>();

            var albumController = GetAlbumMetadataQueryController(null, null, null);
            albumController.ControllerContext = WebTestHelpers.GetHttpContextWithUser();
        }

        private AlbumMetadataQueryController GetAlbumMetadataQueryController(
            ImageGalleryHttpClient httpClientFactory = null,
            IOptions<ApplicationOptions> settings = null,
            ILogger<AlbumMetadataQueryController> logger = null)
        {
            httpClientFactory = httpClientFactory ?? new Mock<ImageGalleryHttpClient>().Object;
            settings = settings ?? new Mock<IOptions<ApplicationOptions>>().Object;
            logger = logger ?? new Mock<ILogger<AlbumMetadataQueryController>>().Object;

            return new AlbumMetadataQueryController(httpClientFactory, settings, logger);
        }
    }
}
