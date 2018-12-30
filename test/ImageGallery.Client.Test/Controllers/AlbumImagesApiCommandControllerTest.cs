﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
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
    public class AlbumImagesApiCommandControllerTest
    {
        [Fact]
        public async Task Delete_Image_From_Album_Returns_Success()
        {
            // Arrange
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetAlbumImagesApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.Delete(It.IsAny<Guid>(), It.IsAny<Guid>());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);

            var objectResult = result as OkResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
        }

        [Fact]
        public async Task Delete_Image_From_Album_Returns_Api_Unauthorized()
        {
            // Arrange
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized);
            var controller = GetAlbumImagesApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.Delete(It.IsAny<Guid>(), It.IsAny<Guid>());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UnauthorizedResult>(result);

            var objectResult = result as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        private AlbumImagesApiCommandController GetAlbumImagesApiCommandController(
        HttpResponseMessage responseMessage,
        ImageGalleryHttpClient imageGalleryHttpClient = null,
        IOptions<ApplicationOptions> settings = null,
        ILogger<AlbumImagesApiCommandController> logger = null)
        {
            // TODO Add to Helper 
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
            logger = logger ?? new Mock<ILogger<AlbumImagesApiCommandController>>().Object;

            return new AlbumImagesApiCommandController(imageGalleryHttpClient, settings, logger);
        }
    }
}
