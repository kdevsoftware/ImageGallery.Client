using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ImageGallery.Client.Apis;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.HttpClients;
using ImageGallery.Client.Test.Data;
using ImageGallery.Client.Test.Helpers;
using ImageGallery.Client.ViewModels.Album;
using ImageGallery.Model.Models.Albums;
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
    public class AlbumImagesApiQueryControllerTest
    {
        [Fact]
        public async Task Get_Album_Images_ReturnsData()
        {
            // Arrange
            int count = 5;
            var albumImages = ImageDataSet.GetAlbumImages(count);
            var content = JsonConvert.SerializeObject(albumImages);

            var albumImagesController = GetAlbumImagesApiQueryController(null, null, null, content);
            albumImagesController.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await albumImagesController.GetAlbumImages(It.IsAny<Guid>());

            // Assert
            Assert.IsType<List<AlbumImage>>(albumImages);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);

            var albumImageIndex = objectResult.Value as AlbumImageIndexViewModel;
            Assert.IsType<AlbumImageIndexViewModel>(albumImageIndex);
            Assert.True(count == albumImageIndex.Images.Count());
        }

        [Fact]
        public async Task Get_Album_Images_Paging_ReturnsData()
        {
            // Arrange
            int count = 10;
            var albumImages = ImageDataSet.GetAlbumImages(count);
            var content = JsonConvert.SerializeObject(albumImages);

            var albumImagesController = GetAlbumImagesApiQueryController(null, null, null, content);
            albumImagesController.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await albumImagesController.GetAlbumImagesPaging(It.IsAny<Guid>(), count, 0);

            // Assert
            Assert.IsType<List<AlbumImage>>(albumImages);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);

            var albumImageIndex = objectResult.Value as AlbumImageIndexViewModel;
            Assert.IsType<AlbumImageIndexViewModel>(albumImageIndex);
            Assert.True(count == albumImageIndex.Images.Count());
        }

        private AlbumImagesApiQueryController GetAlbumImagesApiQueryController(
            ImageGalleryHttpClient imageGalleryHttpClient = null,
            IOptions<ApplicationOptions> settings = null,
            ILogger<AlbumImagesApiQueryController> logger = null,
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
            logger = logger ?? new Mock<ILogger<AlbumImagesApiQueryController>>().Object;

            return new AlbumImagesApiQueryController(imageGalleryHttpClient, settings, logger);
        }
    }
}
