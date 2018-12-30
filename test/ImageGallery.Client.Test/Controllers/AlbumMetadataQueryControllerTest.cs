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
using ImageGallery.Model.Models.Albums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.Client.Test.Controllers
{
    public class AlbumMetadataQueryControllerTest
    {
        private readonly ITestOutputHelper _output;

        public AlbumMetadataQueryControllerTest(ITestOutputHelper output)
        {
            this._output = output;
        }

        [Fact]
        public async Task GetAlbum_Metadata_ReturnsData()
        {
            int tagCount = 5;
            var albumMetadata = AlbumDataSet.GetAlbumMetaData(tagCount);
            var content = JsonConvert.SerializeObject(albumMetadata);
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK, content);

            var albumMetadataController = GetAlbumMetadataQueryController(httpRespose, null, null);
            albumMetadataController.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await albumMetadataController.GetAlbumMetadata(It.IsAny<Guid>());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);

            var albumMetaData = objectResult.Value as AlbumMetaData;
            Assert.IsType<AlbumMetaData>(albumMetaData);

            Assert.True(albumMetaData.AlbumTags.Count == tagCount);
        }

        [Fact]
        public async Task GetAlbum_Metadata_Api_Unauthorized()
        {
            int tagCount = 5;
            var albumMetadata = AlbumDataSet.GetAlbumMetaData(tagCount);
            var content = JsonConvert.SerializeObject(albumMetadata);
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized, content);

            var albumMetadataController = GetAlbumMetadataQueryController(httpRespose, null, null);
            albumMetadataController.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await albumMetadataController.GetAlbumMetadata(It.IsAny<Guid>());

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UnauthorizedResult>(result);

            var objectResult = result as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        private AlbumMetadataQueryController GetAlbumMetadataQueryController(
            HttpResponseMessage responseMessage,
            ImageGalleryHttpClient imageGalleryHttpClient = null,
            ILogger<AlbumMetadataQueryController> logger = null)
        {
            var applicationOptionsMock = new Mock<IOptions<ApplicationOptions>>();
            applicationOptionsMock.Setup(x => x.Value).Returns(new ApplicationOptions
            {
                ImagesUri = CommonConstants.ImagesUri,
            });

            logger = logger ?? new Mock<ILogger<AlbumMetadataQueryController>>().Object;

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
               new ImageGalleryHttpClient(httpClient, applicationOptionsMock.Object, new Mock<IHttpContextAccessor>().Object);

            return new AlbumMetadataQueryController(imageGalleryHttpClient, applicationOptionsMock.Object, logger);
        }
    }
}
