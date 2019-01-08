using System;
using System.Net;
using System.Net.Http;
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
using Newtonsoft.Json;
using Xunit;

namespace ImageGallery.Client.Test.Controllers
{
    public class ImageEndpointControllerTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task Get_Image_Base64_Text_ReturnsData()
        {
            var image = ImageDataSet.GetImageData();
            var content = JsonConvert.SerializeObject(image);
            var httpResponse = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK, content);

            var imageFile = ImageDataSet.GetImageFile(1000, 1000, 100, 100);
            var contentImage = JsonConvert.SerializeObject(imageFile);
            var httpResponse2 = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK, contentImage);

            var controller = GetImageEndpointController(httpResponse, httpResponse2, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var sut = await controller.GetImageBase64File(It.IsAny<Guid>());

            // Assert
            Assert.NotNull(sut);
            Assert.IsType<ContentResult>(sut);

            var objectResult = sut as ContentResult;
            Assert.NotNull(objectResult);
            Assert.NotNull(objectResult.Content);
        }

        private ImageEndpointController GetImageEndpointController(
            HttpResponseMessage responseMessage,
            HttpResponseMessage imageEndpointResponseMessage,
            ImageGalleryHttpClient imageGalleryHttpClient = null,
            ImageEndpointHttpClient imageEndpointHttpClient = null,
            ILogger<ImageEndpointController> logger = null)
        {
            var applicationOptionsMock = new Mock<IOptions<ApplicationOptions>>();
            applicationOptionsMock.Setup(x => x.Value).Returns(new ApplicationOptions
            {
                ImagesUri = CommonConstants.ImagesUri,
            });

            logger = logger ?? new Mock<ILogger<ImageEndpointController>>().Object;

            // ImageGalleryHttpClient
            var handlerMock = MockHelpers.GetHttpMessageHandlerMock(responseMessage);
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(CommonConstants.BaseAddress),
            };
            imageGalleryHttpClient = imageGalleryHttpClient ?? new ImageGalleryHttpClient(httpClient, applicationOptionsMock.Object, new Mock<IHttpContextAccessor>().Object);

            // ImageEndpointHttpClient
            var handlerMock2 = MockHelpers.GetHttpMessageHandlerMock(imageEndpointResponseMessage);
            var httpClient2 = new HttpClient(handlerMock2.Object)
            {
                BaseAddress = new Uri(CommonConstants.BaseAddress),
            };
            imageEndpointHttpClient = imageEndpointHttpClient ?? new ImageEndpointHttpClient(httpClient2, applicationOptionsMock.Object, new Mock<IHttpContextAccessor>().Object);

            return new ImageEndpointController(imageGalleryHttpClient, imageEndpointHttpClient, applicationOptionsMock.Object, logger);
        }
    }
}
