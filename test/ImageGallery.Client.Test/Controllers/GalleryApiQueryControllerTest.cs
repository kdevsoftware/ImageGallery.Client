using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ImageGallery.Client.Apis;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Filters;
using ImageGallery.Client.HttpClients;
using ImageGallery.Client.Test.Data;
using ImageGallery.Client.Test.Helpers;
using ImageGallery.Client.ViewModels;
using ImageGallery.Client.ViewModels.Gallery;
using ImageGallery.Model.Models.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace ImageGallery.Client.Test.Controllers
{
    public class GalleryApiQueryControllerTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task Get_Images_ReturnsData()
        {
            // Arrange
            int count = 5;
            var images = ImageDataSet.GetImageTableData(count);
            var content = JsonConvert.SerializeObject(images);
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK, content);

            var controller = GetGalleryImagesApiQueryController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();
            controller.Response.Headers.Add("x-inlinecount", "10");

            // Act
            var sut = await controller.GalleryIndexViewModel();

            // Assert
            Assert.IsType<List<Image>>(images);
            Assert.NotNull(sut);
            Assert.IsType<OkObjectResult>(sut);

            var objectResult = sut as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
            // Assert.Equal("10", controller.Response.Headers["x-inlinecount"]);

            var galleryImageIndex = objectResult.Value as GalleryIndexViewModel;
            Assert.IsType<GalleryIndexViewModel>(galleryImageIndex);
            Assert.True(count == galleryImageIndex.Images.Count());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Get_Images_Returns_Api_Unauthorized()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized, null);

            var controller = GetGalleryImagesApiQueryController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var sut = await controller.GalleryIndexViewModel();

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<UnauthorizedResult>(sut);

            var objectResult = sut as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Get_Images_Returns_Api_Exception()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.ExpectationFailed);

            var controller = GetGalleryImagesApiQueryController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.GalleryIndexViewModel();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UnprocessableEntityObjectResult>(result);

            var objectResult = result as UnprocessableEntityObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 422);
            Assert.Equal("Expectation Failed", objectResult.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Get_Images_Paging_ReturnsData()
        {
            // Arrange
            int count = 10;
            var images = ImageDataSet.GetImageTableData(count);
            var content = JsonConvert.SerializeObject(images);
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK, content);

            var controller = GetGalleryImagesApiQueryController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var requestModel = new GalleryRequestModel { };
            var sut = await controller.Get(requestModel, count, 0);

            // Assert
            Assert.IsType<List<Image>>(images);
            Assert.NotNull(sut);
            Assert.IsType<OkObjectResult>(sut);

            var objectResult = sut as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);

            var galleryImageIndex = objectResult.Value as GalleryIndexViewModel;
            Assert.IsType<GalleryIndexViewModel>(galleryImageIndex);
            Assert.True(count == galleryImageIndex.Images.Count());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Get_Images_Paging_Returns_Api_Unauthorized()
        {
            // Arrange
            int count = 10;
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized, null);

            var controller = GetGalleryImagesApiQueryController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var requestModel = new GalleryRequestModel { };
            var sut = await controller.Get(requestModel, count, 0);

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<UnauthorizedResult>(sut);

            var objectResult = sut as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Get_Images_Paging_Returns_Api_Exception()
        {
            // Arrange
            int count = 10;
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.ExpectationFailed);

            var controller = GetGalleryImagesApiQueryController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var requestModel = new GalleryRequestModel { };
            var sut = await controller.Get(requestModel, count, 0);

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
        public async Task Get_Image_Properties_ReturnsData()
        {
            var image = ImageDataSet.GetImageData();
            var content = JsonConvert.SerializeObject(image);
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK, content);

            var controller = GetGalleryImagesApiQueryController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var sut = await controller.GetImageProperties(It.IsAny<Guid>());

            // Assert
            Assert.IsType<Image>(image);
            Assert.Equal(image.Width < image.Height ? 240 : 320, image.Width);
            Assert.Equal(image.Width > image.Height ? 320 : 240, image.Width);

            Assert.NotNull(sut);
            Assert.IsType<OkObjectResult>(sut);

            var objectResult = sut as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);

            var imageViewModel = objectResult.Value as ImageViewModel;
            Assert.IsType<ImageViewModel>(imageViewModel);
            Assert.Equal($"{CommonConstants.ImagesUri}{image.FileName}", imageViewModel.ImageUrl);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Get_Image_Properties_Api_Unauthorized()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized, null);

            var controller = GetGalleryImagesApiQueryController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var sut = await controller.GetImageProperties(It.IsAny<Guid>());

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<UnauthorizedResult>(sut);

            var objectResult = sut as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Get_Image_Properties_Api_Exception()
        {
            // Arrange
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.ExpectationFailed, null);

            var controller = GetGalleryImagesApiQueryController(httpResponse, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var sut = await controller.GetImageProperties(It.IsAny<Guid>());

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<UnprocessableEntityObjectResult>(sut);

            var objectResult = sut as UnprocessableEntityObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 422);
            Assert.Equal("Expectation Failed", objectResult.Value);
        }

        private GalleryApiQueryController GetGalleryImagesApiQueryController(
            HttpResponseMessage responseMessage,
            ImageGalleryHttpClient imageGalleryHttpClient = null,
            IOptions<ApplicationOptions> settings = null,
            ILogger<GalleryApiQueryController> logger = null)
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
            logger = logger ?? new Mock<ILogger<GalleryApiQueryController>>().Object;

            return new GalleryApiQueryController(imageGalleryHttpClient, settings, logger);
        }
    }
}
