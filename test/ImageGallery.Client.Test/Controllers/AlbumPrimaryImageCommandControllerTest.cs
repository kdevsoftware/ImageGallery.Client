using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ImageGallery.Client.Apis;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.HttpClients;
using ImageGallery.Client.Test.Data;
using ImageGallery.Client.Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace ImageGallery.Client.Test.Controllers
{
    public class AlbumPrimaryImageCommandControllerTest
    {

        [Fact]
        public async Task Update_Image_Properties_Returns_Success()
        {
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetAlbumPrimaryImageCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.Put(It.IsAny<Guid>(), It.IsAny<Guid>());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Update_Image_Properties_Returns_Api_Unauthorized()
        {
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized);
            var controller = GetAlbumPrimaryImageCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.Put(It.IsAny<Guid>(), It.IsAny<Guid>());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UnauthorizedResult>(result);

        }

        private AlbumPrimaryImageCommandController GetAlbumPrimaryImageCommandController(
            HttpResponseMessage responseMessage,
            ImageGalleryHttpClient imageGalleryHttpClient = null,
            IOptions<ApplicationOptions> settings = null,
            ILogger<AlbumPrimaryImageCommandController> logger = null)
        {

            responseMessage.Headers.Add("x-inlinecount", "10");

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage)
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(CommonConstants.BaseAddress),
            };

            imageGalleryHttpClient = imageGalleryHttpClient ??
                                     new ImageGalleryHttpClient(httpClient, new Mock<IOptions<ApplicationOptions>>().Object, new Mock<IHttpContextAccessor>().Object);

            // Application Options.
            var applicationOptionsMock = new Mock<IOptions<ApplicationOptions>>();
            applicationOptionsMock.Setup(x => x.Value).Returns(new ApplicationOptions
            {
                ImagesUri = CommonConstants.ImagesUri,
            });
            settings = settings ?? applicationOptionsMock.Object;

            // Logger
            logger = logger ?? new Mock<ILogger<AlbumPrimaryImageCommandController>>().Object;

            return new AlbumPrimaryImageCommandController(imageGalleryHttpClient, settings, logger);
        }
    }
}
