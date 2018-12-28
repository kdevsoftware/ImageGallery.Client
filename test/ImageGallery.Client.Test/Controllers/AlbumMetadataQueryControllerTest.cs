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

            var albumMetadataController = GetAlbumMetadataQueryController(null, null, null, content);
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

        private AlbumMetadataQueryController GetAlbumMetadataQueryController(
            ImageGalleryHttpClient imageGalleryHttpClient = null,
            IOptions<ApplicationOptions> settings = null,
            ILogger<AlbumMetadataQueryController> logger = null,
            string responseContent = null)
        {

            var responseMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent),
            };

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
                BaseAddress = new Uri("http://localhost/"),
            };

            imageGalleryHttpClient = imageGalleryHttpClient ?? new ImageGalleryHttpClient(httpClient, new Mock<IOptions<ApplicationOptions>>().Object, new Mock<IHttpContextAccessor>().Object);

            settings = settings ?? new Mock<IOptions<ApplicationOptions>>().Object;
            logger = logger ?? new Mock<ILogger<AlbumMetadataQueryController>>().Object;

            return new AlbumMetadataQueryController(imageGalleryHttpClient, settings, logger);
        }
    }
}
