﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
using Newtonsoft.Json;
using Xunit;

namespace ImageGallery.Client.Test.Controllers
{
    public class GalleryApiCommandControllerTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task Add_Image_Returns_Success()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            AddImageViewModel model = new AddImageViewModel
            {
                Category = It.IsAny<string>(),
                Title = It.IsAny<string>(),
                File = MockHelpers.GetMockIFormFile().Object,
            };

            var sut = await controller.AddImage(model);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<OkResult>(sut);

            var objectResult = sut as OkResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Add_Image_Returns_Api_Unauthorized()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized);
            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            AddImageViewModel model = new AddImageViewModel
            {
                Category = It.IsAny<string>(),
                Title = It.IsAny<string>(),
                File = MockHelpers.GetMockIFormFile().Object,
            };

            var sut = await controller.AddImage(model);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<UnauthorizedResult>(sut);

            var objectResult = sut as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Add_Image_Returns_Validation_Error()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();
            controller.ModelState.AddModelError("key", "error message");

            // Act
            AddImageViewModel model = new AddImageViewModel
            {
                Category = It.IsAny<string>(),
                Title = null,
                File = MockHelpers.GetMockIFormFile().Object,
            };

            var sut = await controller.AddImage(model);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<BadRequestObjectResult>(sut);

            var objectResult = sut as BadRequestObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 400);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Update_Image_Returns_Success()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            UpdateImageViewModel model = new UpdateImageViewModel
            {
                Id = Guid.NewGuid(),
                File = MockHelpers.GetMockIFormFile().Object,
            };

            var sut = await controller.UpdateImage(model);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<OkResult>(sut);

            var objectResult = sut as OkResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Update_Image_Returns_Api_Unauthorized()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized);
            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            UpdateImageViewModel model = new UpdateImageViewModel
            {
                Id = Guid.NewGuid(),
                File = MockHelpers.GetMockIFormFile().Object,
            };

            var sut = await controller.UpdateImage(model);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<UnauthorizedResult>(sut);

            var objectResult = sut as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Update_Image_Returns_Validation_Error()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.BadRequest);
            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();
            controller.ModelState.AddModelError("key", "error message");

            // Act
            UpdateImageViewModel model = new UpdateImageViewModel
            {
                Id = Guid.NewGuid(),
                File = MockHelpers.GetMockIFormFile().Object,
            };

            var sut = await controller.UpdateImage(model);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<BadRequestObjectResult>(sut);

            var objectResult = sut as BadRequestObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 400);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Update_Image_Returns_Api_Exception()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.ExpectationFailed);
            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            UpdateImageViewModel model = new UpdateImageViewModel
            {
                Id = Guid.NewGuid(),
                File = MockHelpers.GetMockIFormFile().Object,
            };

            var sut = await controller.UpdateImage(model);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<UnprocessableEntityObjectResult>(sut);

            var objectResult = sut as UnprocessableEntityObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 422);
            Assert.Equal("Expectation Failed", objectResult.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Update_Image_Properties_Returns_Success()
        {
            var album = ImageDataSet.GetImageData();
            var content = JsonConvert.SerializeObject(album);
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK, content);

            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var patchDtos = new List<PatchDto>
            {
                new PatchDto { PropertyName = "Title", PropertyValue = "Test" },
            };

            var sut = await controller.PatchImageProperties(Guid.NewGuid(), patchDtos);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<OkResult>(sut);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Update_Image_Properties_Returns_Api_Unauthorized()
        {
            var album = ImageDataSet.GetImageData();
            var content = JsonConvert.SerializeObject(album);
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized, content);

            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var patchDtos = new List<PatchDto>
            {
                new PatchDto { PropertyName = "Title", PropertyValue = "Test" },
            };

            var sut = await controller.PatchImageProperties(Guid.NewGuid(), patchDtos);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<UnauthorizedResult>(sut);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Update_Image_Properties_Returns_Api_Exception()
        {
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.ExpectationFailed, null);

            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var patchDtos = new List<PatchDto>
            {
                new PatchDto { PropertyName = "Title", PropertyValue = "Test" },
            };

            var sut = await controller.PatchImageProperties(Guid.NewGuid(), patchDtos);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<UnprocessableEntityObjectResult>(sut);

            var objectResult = sut as UnprocessableEntityObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 422);
            Assert.Equal("Expectation Failed", objectResult.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Edit_Image_Properties_Returns_Success()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            EditImageViewModel model = new EditImageViewModel
            {
                Id = It.IsAny<Guid>(),
                Category = It.IsAny<string>(),
                Title = It.IsAny<string>(),
            };

            var sut = await controller.EditImageProperties(model);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<OkResult>(sut);

            var objectResult = sut as OkResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Edit_Image_Properties_Returns_Validation_Error()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.BadRequest);
            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();
            controller.ModelState.AddModelError("key", "error message");

            // Act
            EditImageViewModel model = new EditImageViewModel
            {
                Id = It.IsAny<Guid>(),
                Category = It.IsAny<string>(),
                Title = null,
            };

            var sut = await controller.EditImageProperties(model);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<BadRequestObjectResult>(sut);

            var objectResult = sut as BadRequestObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 400);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Edit_Image_Properties_Returns_Api_Unauthorized()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized);
            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            EditImageViewModel model = new EditImageViewModel
            {
                Id = It.IsAny<Guid>(),
                Category = It.IsAny<string>(),
                Title = It.IsAny<string>(),
            };

            var sut = await controller.EditImageProperties(model);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<UnauthorizedResult>(sut);

            var objectResult = sut as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Edit_Image_Properties_Returns_Api_Exception()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.ExpectationFailed);
            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            EditImageViewModel model = new EditImageViewModel
            {
                Id = It.IsAny<Guid>(),
                Category = It.IsAny<string>(),
                Title = It.IsAny<string>(),
            };

            var sut = await controller.EditImageProperties(model);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<UnprocessableEntityObjectResult>(sut);

            var objectResult = sut as UnprocessableEntityObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 422);
            Assert.Equal("Expectation Failed", objectResult.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Delete_Image_Returns_Success()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK);
            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var sut = await controller.DeleteImage(It.IsAny<Guid>());

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<OkResult>(sut);

            var objectResult = sut as OkResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Delete_Image_Returns_Api_Unauthorized()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized);
            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var sut = await controller.DeleteImage(It.IsAny<Guid>());

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<UnauthorizedResult>(sut);

            var objectResult = sut as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Delete_Image_Returns_Api_Exception()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.ExpectationFailed);
            var controller = GetGalleryApiCommandController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var sut = await controller.DeleteImage(It.IsAny<Guid>());

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<UnprocessableEntityObjectResult>(sut);

            var objectResult = sut as UnprocessableEntityObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 422);
            Assert.Equal("Expectation Failed", objectResult.Value);
        }

        private GalleryApiCommandController GetGalleryApiCommandController(
            HttpResponseMessage responseMessage,
            ImageGalleryHttpClient imageGalleryHttpClient = null,
            IOptions<ApplicationOptions> settings = null,
            ILogger<GalleryApiCommandController> logger = null)
        {

            //TODO Add to Helper
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
            logger = logger ?? new Mock<ILogger<GalleryApiCommandController>>().Object;

            return new GalleryApiCommandController(imageGalleryHttpClient, settings, logger);
        }
    }
}
