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
            int count = 100;
            var albums = AlbumDataSet.GetAlbumTableData(count);
            var content = JsonConvert.SerializeObject(albums);
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK, content);

            var controller = GetAlbumApiQueryController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.AlbumIndexViewModel();

            // Assert
            Assert.IsType<List<Album>>(albums);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
            Assert.IsType<AlbumIndexViewModel>(objectResult.Value);

            var albumIndex = objectResult.Value as AlbumIndexViewModel;
            Assert.NotNull(albumIndex);
            Assert.Equal(CommonConstants.ImagesUri, albumIndex.ImagesUri);

            var results = albumIndex.Albums;
            Assert.NotNull(results);
            Assert.IsType<List<Album>>(results);
            Assert.True(results.Count() == count);
        }

        [Fact]
        public async Task GetAlbums_Returns_Api_Unauthorized()
        {
            int count = 100;
            var albums = AlbumDataSet.GetAlbumTableData(count);
            var content = JsonConvert.SerializeObject(albums);
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized, content);

            var controller = GetAlbumApiQueryController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.AlbumIndexViewModel();

            // Assert
            Assert.IsType<List<Album>>(albums);
            Assert.NotNull(result);
            Assert.IsType<UnauthorizedResult>(result);

            var objectResult = result as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        [Fact]
        public async Task GetAlbums_Paging_ReturnsData()
        {
            int count = 10;
            var albums = AlbumDataSet.GetAlbumTableData(count);
            var content = JsonConvert.SerializeObject(albums);
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK, content);

            var controller = GetAlbumApiQueryController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            var query = new AlbumRequestModel { };

            // Act
            var result = await controller.Get(query, count, 1);

            // Assert
            Assert.IsType<List<Album>>(albums);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
            Assert.IsType<AlbumIndexViewModel>(objectResult.Value);

            var albumIndex = objectResult.Value as AlbumIndexViewModel;
            Assert.NotNull(albumIndex);
            Assert.Equal(CommonConstants.ImagesUri, albumIndex.ImagesUri);

            var results = albumIndex.Albums;
            Assert.NotNull(results);
            Assert.IsType<List<Album>>(results);
            Assert.True(results.Count() == count);
        }

        [Fact]
        public async Task GetAlbums_Paging_Returns_Api_Unauthorized()
        {
            int count = 10;
            var albums = AlbumDataSet.GetAlbumTableData(count);
            var content = JsonConvert.SerializeObject(albums);
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized, content);

            var controller = GetAlbumApiQueryController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            var query = new AlbumRequestModel { };

            // Act
            var result = await controller.Get(query, count, 1);

            // Assert
            Assert.IsType<List<Album>>(albums);
            Assert.NotNull(result);
            Assert.IsType<UnauthorizedResult>(result);

            var objectResult = result as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        [Fact]
        public async Task GetAlbum_ReturnsData()
        {
            var album = AlbumDataSet.GetAlbum();
            var content = JsonConvert.SerializeObject(album);
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK, content);

            var controller = GetAlbumApiQueryController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.GetAlbum(It.IsAny<Guid>());

            // Assert
            Assert.IsType<Album>(album);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetAlbum_Returns_Api_Unauthorized()
        {
            var album = AlbumDataSet.GetAlbum();
            var content = JsonConvert.SerializeObject(album);
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized, content);

            var controller = GetAlbumApiQueryController(httpRespose, null, null, null);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.GetAlbum(It.IsAny<Guid>());

            // Assert
            Assert.IsType<Album>(album);
            Assert.NotNull(result);
            Assert.IsType<UnauthorizedResult>(result);

            var objectResult = result as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        private AlbumApiQueryController GetAlbumApiQueryController(
            HttpResponseMessage responseMessage,
            ImageGalleryHttpClient imageGalleryHttpClient = null,
            IOptions<ApplicationOptions> settings = null,
            ILogger<AlbumApiQueryController> logger = null)
        {

            // TODO Add to Helper
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
            var applicationOptionsMock = new Mock<IOptions<ApplicationOptions>>();
            applicationOptionsMock.Setup(x => x.Value).Returns(new ApplicationOptions { ImagesUri = "http://localhost/api/images" });
            settings = settings ?? applicationOptionsMock.Object;
            logger = logger ?? new Mock<ILogger<AlbumApiQueryController>>().Object;

            return new AlbumApiQueryController(imageGalleryHttpClient, settings, logger);
        }
    }
}
