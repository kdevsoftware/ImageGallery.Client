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
    public class GalleryApiCommandControllerTest
    {
        [Fact]
        public async Task Add_Image_Returns_Success()
        {
            // Arrange
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetGalleryApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            AddImageViewModel model = new AddImageViewModel
            {
                Category = It.IsAny<string>(),
                Title = It.IsAny<string>(),
                File = MockHelpers.GetMockIFormFile().Object,
            };

            var result = await controller.AddImage(model);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);

            var objectResult = result as OkResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
        }

        [Fact]
        public async Task Add_Image_Returns_Api_Unauthorized()
        {
            // Arrange
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized);
            var controller = GetGalleryApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            AddImageViewModel model = new AddImageViewModel
            {
                Category = It.IsAny<string>(),
                Title = It.IsAny<string>(),
                File = MockHelpers.GetMockIFormFile().Object,
            };

            var result = await controller.AddImage(model);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UnauthorizedResult>(result);

            var objectResult = result as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        [Fact]
        public async Task Update_Image_Returns_Success()
        {
            // Arrange
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetGalleryApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            UpdateImageViewModel model = new UpdateImageViewModel
            {
                Id = Guid.NewGuid(),
                File = MockHelpers.GetMockIFormFile().Object,
            };

            var result = await controller.UpdateImage(model);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);

            var objectResult = result as OkResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
        }

        [Fact]
        public async Task Update_Image_Returns_Api_Unauthorized()
        {
            // Arrange
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized);
            var controller = GetGalleryApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            UpdateImageViewModel model = new UpdateImageViewModel
            {
                Id = Guid.NewGuid(),
                File = MockHelpers.GetMockIFormFile().Object,
            };

            var result = await controller.UpdateImage(model);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UnauthorizedResult>(result);

            var objectResult = result as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        [Fact]
        public async Task Update_Image_Properties_Returns_Success()
        {
            var album = ImageDataSet.GetImageData();
            var content = JsonConvert.SerializeObject(album);
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK, content);

            var controller = GetGalleryApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var patchDtos = new List<PatchDto>
            {
                new PatchDto { PropertyName = "Title", PropertyValue = "Test" },
            };

            var result = await controller.PatchImageProperties(Guid.NewGuid(), patchDtos);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Update_Image_Properties_Returns_Api_Unauthorized()
        {
            var album = ImageDataSet.GetImageData();
            var content = JsonConvert.SerializeObject(album);
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized, content);

            var controller = GetGalleryApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var patchDtos = new List<PatchDto>
            {
                new PatchDto { PropertyName = "Title", PropertyValue = "Test" },
            };

            var result = await controller.PatchImageProperties(Guid.NewGuid(), patchDtos);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Edit_Image_Properties_Returns_Success()
        {
            // Arrange
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetGalleryApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            EditImageViewModel model = new EditImageViewModel
            {
                Id = It.IsAny<Guid>(),
                Category = It.IsAny<string>(),
                Title = It.IsAny<string>(),
            };

            var result = await controller.EditImagePropeties(model);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);

            var objectResult = result as OkResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
        }

        [Fact]
        public async Task Edit_Image_Properties_Returns_Api_Unauthorized()
        {
            // Arrange
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized);
            var controller = GetGalleryApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            EditImageViewModel model = new EditImageViewModel
            {
                Id = It.IsAny<Guid>(),
                Category = It.IsAny<string>(),
                Title = It.IsAny<string>(),
            };

            var result = await controller.EditImagePropeties(model);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UnauthorizedResult>(result);

            var objectResult = result as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        [Fact]
        public async Task Delete_Image_Returns_Success()
        {
            // Arrange
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetGalleryApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.DeleteImage(It.IsAny<Guid>());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);

            var objectResult = result as OkResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
        }

        [Fact]
        public async Task Delete_Image_Returns_Api_Unauthorized()
        {
            // Arrange
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized);
            var controller = GetGalleryApiCommandController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.DeleteImage(It.IsAny<Guid>());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UnauthorizedResult>(result);

            var objectResult = result as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        private GalleryApiCommandController GetGalleryApiCommandController(
            HttpResponseMessage responseMessage,
            ImageGalleryHttpClient imageGalleryHttpClient = null,
            IOptions<ApplicationOptions> settings = null,
            ILogger<GalleryApiCommandController> logger = null)
        {

            //TODO Add to Helper
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
            logger = logger ?? new Mock<ILogger<GalleryApiCommandController>>().Object;

            return new GalleryApiCommandController(imageGalleryHttpClient, settings, logger);
        }
    }
}
