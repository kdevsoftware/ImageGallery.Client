using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ImageGallery.Client.Apis;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.HttpClients;
using ImageGallery.Client.Test.Data;
using ImageGallery.Client.Test.Helpers;
using ImageGallery.Client.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace ImageGallery.Client.Test.Controllers
{
    public class AlbumApiCommandControllerTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task Update_Album_Properties_Returns_Success()
        {
            var album = AlbumDataSet.GetAlbum();
            var content = JsonConvert.SerializeObject(album);
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK, content);

            var controller = GetAlbumApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var patchDtos = new List<PatchDto>
            {
                new PatchDto { PropertyName = "Description", PropertyValue = "Test" },
            };

            var result = await controller.PatchAlbum(Guid.NewGuid(), patchDtos);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Update_Album_Invalid_Guid_Returns_Failure()
        {
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetAlbumApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.PatchAlbum(It.IsAny<Guid>(), It.IsAny<List<PatchDto>>());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Update_Album_Properties_Returns_Api_Unauthorized()
        {
            var album = AlbumDataSet.GetAlbum();
            var content = JsonConvert.SerializeObject(album);
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized, content);

            var controller = GetAlbumApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var patchDtos = new List<PatchDto>
            {
                new PatchDto { PropertyName = "Description", PropertyValue = "Test" },
            };

            var result = await controller.PatchAlbum(Guid.NewGuid(), patchDtos);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Delete_Album_Returns_Success()
        {
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetAlbumApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.DeleteAlbum(It.IsAny<Guid>());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Delete_Album_Returns_Api_Unauthorized()
        {
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized);
            var controller = GetAlbumApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.DeleteAlbum(It.IsAny<Guid>());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UnauthorizedResult>(result);
        }

        private AlbumApiCommandController GetAlbumApiCommandController(
            HttpResponseMessage responseMessage,
            ImageGalleryHttpClient imageGalleryHttpClient = null,
            IOptions<ApplicationOptions> settings = null,
            ILogger<AlbumApiCommandController> logger = null)
        {

            // TODO - Add to Helper
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
            logger = logger ?? new Mock<ILogger<AlbumApiCommandController>>().Object;

            return new AlbumApiCommandController(imageGalleryHttpClient, settings, logger);
        }
    }
}
