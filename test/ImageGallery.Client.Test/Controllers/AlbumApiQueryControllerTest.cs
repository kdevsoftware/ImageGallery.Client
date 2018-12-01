using System.Threading.Tasks;
using ImageGallery.Client.Apis;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Services;
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

        [Fact(Skip = "TODO")]
        public async void GetAlbums_ReturnsData()
        {
            var albumController = GetAlbumApiQueryController(null, null, null);
            await Task.Delay(10);

            Assert.IsType<AlbumApiQueryController>(albumController);
            Assert.True(false);
        }

        private AlbumApiQueryController GetAlbumApiQueryController(
            IImageGalleryHttpClient imageGalleryClient = null,
            IOptions<ApplicationOptions> settings = null,
            ILogger<AlbumApiQueryController> logger = null)
        {
            imageGalleryClient = imageGalleryClient ?? new Mock<IImageGalleryHttpClient>().Object;
            settings = settings ?? new Mock<IOptions<ApplicationOptions>>().Object;
            logger = logger ?? new Mock<ILogger<AlbumApiQueryController>>().Object;

            return new AlbumApiQueryController(imageGalleryClient, settings, logger);
        }
    }
}
