using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
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
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;

namespace ImageGallery.Client.Test.Controllers
{
    public class GalleryApiQueryControllerTest
    {
        [Fact]
        public async Task Get_Images_ReturnsData()
        {
            // Arrange
            int count = 5;
            var images = ImageDataSet.GetImageTableData(count);
            var content = JsonConvert.SerializeObject(images);

            var galleryImagesApiQueryController = GetGalleryImagesApiQueryController(null, null, null, content);
            galleryImagesApiQueryController.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await galleryImagesApiQueryController.GalleryIndexViewModel();

            // Assert
            Assert.IsType<List<Image>>(images);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);

            var galleryImageIndex = objectResult.Value as GalleryIndexViewModel;
            Assert.IsType<GalleryIndexViewModel>(galleryImageIndex);
            Assert.True(count == galleryImageIndex.Images.Count());
        }

        [Fact]
        public async Task Get_Images_Paging_ReturnsData()
        {
            // Arrange
            int count = 10;
            var images = ImageDataSet.GetImageTableData(count);
            var content = JsonConvert.SerializeObject(images);

            var galleryImagesApiQueryController = GetGalleryImagesApiQueryController(null, null, null, content);
            galleryImagesApiQueryController.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var requestModel = new GalleryRequestModel { };
            var result = await galleryImagesApiQueryController.Get(requestModel, count, 0);

            // Assert
            Assert.IsType<List<Image>>(images);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);

            var galleryImageIndex = objectResult.Value as GalleryIndexViewModel;
            Assert.IsType<GalleryIndexViewModel>(galleryImageIndex);
            Assert.True(count == galleryImageIndex.Images.Count());
        }

        [Fact]
        public async Task Get_Image_Properties_ReturnsData()
        {
            var image = ImageDataSet.GetImageData();
            var content = JsonConvert.SerializeObject(image);

            var galleryImagesApiQueryController = GetGalleryImagesApiQueryController(null, null, null, content);
            galleryImagesApiQueryController.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await galleryImagesApiQueryController.GetImageProperties(It.IsAny<Guid>());

            // Assert
            Assert.IsType<Image>(image);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);

            var imageViewModel = objectResult.Value as ImageViewModel;
            Assert.IsType<ImageViewModel>(imageViewModel);
        }

        [Fact(Skip = "TODO")]
        public async Task Get_Image_Base64_Text_ReturnsData()
        {
            Assert.True(false);
        }

        private GalleryApiQueryController GetGalleryImagesApiQueryController(
            ImageGalleryHttpClient imageGalleryHttpClient = null,
            IOptions<ApplicationOptions> settings = null,
            ILogger<GalleryApiQueryController> logger = null,
            string responseContent = null)
        {
            var responseMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent),
            };

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
            logger = logger ?? new Mock<ILogger<GalleryApiQueryController>>().Object;

            return new GalleryApiQueryController(imageGalleryHttpClient, settings, logger);
        }
    }
}
