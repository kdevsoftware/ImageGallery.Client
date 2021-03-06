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
using ImageGallery.Client.ViewModels.Album;
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
    public class AlbumImagesApiCommandControllerTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task Delete_Image_From_Album_Returns_Success()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetAlbumImagesApiCommandController(httpResponse, null, null, null);
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
        [Trait("Category", "Unit")]
        public async Task Delete_Image_From_Album_Returns_Api_Unauthorized()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized);
            var controller = GetAlbumImagesApiCommandController(httpResponse, null, null, null);
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

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Delete_Image_From_Album_Returns_Api_Forbidden()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Forbidden);
            var controller = GetAlbumImagesApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.Delete(It.IsAny<Guid>(), It.IsAny<Guid>());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ForbidResult>(result);

            var objectResult = result as ForbidResult;
            Assert.NotNull(objectResult);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Update_Album_Image_Sort_Returns_Success()
        {
            var albumImages = ImageDataSet.GetAlbumImagesSortList(5);
            var content = JsonConvert.SerializeObject(albumImages);
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK, content);

            var controller = GetAlbumImagesApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            var result = await controller.UpdateAlbumSort(It.IsAny<Guid>(), albumImages);

            // Assert
            Assert.IsType<List<AlbumImageSortItem>>(albumImages);
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);

            var objectResult = result as OkResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Update_Album_Image_Sort_Returns_Api_Unauthorized()
        {
            var albumImages = ImageDataSet.GetAlbumImagesSortList(5);
            var content = JsonConvert.SerializeObject(albumImages);
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized, content);

            var controller = GetAlbumImagesApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            var result = await controller.UpdateAlbumSort(It.IsAny<Guid>(), albumImages);

            // Assert
            Assert.IsType<List<AlbumImageSortItem>>(albumImages);
            Assert.NotNull(result);
            Assert.IsType<UnauthorizedResult>(result);

            var objectResult = result as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Update_Album_Image_Sort_Returns_Api_Forbidden()
        {
            var albumImages = ImageDataSet.GetAlbumImagesSortList(5);
            var content = JsonConvert.SerializeObject(albumImages);
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Forbidden, content);

            var controller = GetAlbumImagesApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            var result = await controller.UpdateAlbumSort(It.IsAny<Guid>(), albumImages);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ForbidResult>(result);

            var objectResult = result as ForbidResult;
            Assert.NotNull(objectResult);
        }

        private AlbumImagesApiCommandController GetAlbumImagesApiCommandController(
        HttpResponseMessage responseMessage,
        ImageGalleryHttpClient imageGalleryHttpClient = null,
        IOptions<ApplicationOptions> settings = null,
        ILogger<AlbumImagesApiCommandController> logger = null)
        {
            // TODO Add to Helper 
            responseMessage.Headers.Add("x-inlinecount", "10");

            var handlerMock = MockHelpers.GetHttpMessageHandlerMock(responseMessage);
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
