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
using ImageGallery.Client.Test.Helpers;
using ImageGallery.Client.ViewModels.Album;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.Client.Test.Controllers
{
    public class AlbumApiQueryControllerTest
    {
        private readonly ITestOutputHelper _output;

        public AlbumApiQueryControllerTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task GetAlbums_ReturnsData()
        {
            var albumController = GetAlbumApiQueryController(null, null, null);
            albumController.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            var query = new AlbumRequestModel { };

            // Act
            var result = await albumController.Get(query, 1, 1);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
            Assert.IsType<AlbumIndexViewModel>(objectResult.Value);

            var albumIndex = objectResult.Value as AlbumIndexViewModel;
            Assert.NotNull(albumIndex);
            Assert.Equal("http://localhost/api/images", albumIndex.ImagesUri);

            var albums = albumIndex.Albums;
            Assert.NotNull(albums);
            Assert.IsType<List<Model.Models.Albums.Album>>(albums);
            Assert.True(albums.Count() == 1);
        }

        [Fact(Skip = "TODO")]
        public async Task GetAlbum_ReturnsData()
        {
            Assert.True(false);
        }

        private AlbumApiQueryController GetAlbumApiQueryController(
            ImageGalleryHttpClient imageGalleryHttpClient = null,
            IOptions<ApplicationOptions> settings = null,
            ILogger<AlbumApiQueryController> logger = null)
        {
            var responseMessage = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[{'Id':'9c270399-bbb0-4503-895b-8128642ac6c6','Title':'Nature','Description':'null','DateCreated':'0001-01-01T00:00:00','FileName':'null','Width':320,'Height':240}]"),
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
                BaseAddress = new Uri("http://localhost/"),
            };

            imageGalleryHttpClient = imageGalleryHttpClient ??
                new ImageGalleryHttpClient(httpClient, new Mock<IOptions<ApplicationOptions>>().Object, new Mock<IHttpContextAccessor>().Object);
            var applicationOptionsMock = new Mock<IOptions<ApplicationOptions>>();
            applicationOptionsMock.Setup(x => x.Value).Returns(new ApplicationOptions { ImagesUri = "http://localhost/api/images" });
            settings = settings ?? applicationOptionsMock.Object;
            logger = logger ?? new Mock<ILogger<AlbumApiQueryController>>().Object;

            return new AlbumApiQueryController(imageGalleryHttpClient, settings, logger);
        }
    }
}
