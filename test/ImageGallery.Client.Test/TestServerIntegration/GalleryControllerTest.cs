using ImageGallery.Client.Configuration;
using ImageGallery.Client.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ImageGallery.Client.Test.TestServerIntegration
{
    public class GalleryControllerTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void Get_Gallery_Index()
        {
            // Arrange
            var controller = GetGalleryController();

            // Act
            var result = controller.Index();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);

            var objectResult = result as ViewResult;
            Assert.NotNull(objectResult);
        }

        private GalleryController GetGalleryController()
        {
            // Application Options.
            var applicationOptionsMock = new Mock<IOptions<ApplicationOptions>>();
            applicationOptionsMock.Setup(x => x.Value).Returns(new ApplicationOptions
            {
                ClientConfiguration = new ClientConfiguration { ApiAttractionsUri = "Test" },
                OpenIdConnectConfiguration = new OpenIdConnectConfiguration { ClientId = "Test" },
                LogglyClientConfiguration = new LogglyClientConfiguration { LogglyKey = "Test" },
            });

            var settings = applicationOptionsMock.Object;

            var logger = new Mock<ILogger<GalleryController>>().Object;

            return new GalleryController(settings, logger);
        }
    }
}
