using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Controllers;
using ImageGallery.Client.Test.Fixtures;
using Loggly;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.Client.Test.TestServerIntegration
{
    public class ConfigurationControllerTest : IClassFixture<TestServerFixture>
    {
        private readonly HttpClient _client;

        private readonly ITestOutputHelper _output;

        public ConfigurationControllerTest(TestServerFixture fixture, ITestOutputHelper output)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
            _client = fixture.Client;
            _output = output;
        }

        [Fact]
        [Trait("Category", "Intergration")]
        public async Task Get_Client_Configuration()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/ClientAppSettings");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var configurationOptions = JsonConvert.DeserializeObject<ApplicationOptions>(responseString);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.IsType<ApplicationOptions>(configurationOptions);
            Assert.IsType<ClientConfiguration>(configurationOptions.ClientConfiguration);
            Assert.IsType<OpenIdConnectConfiguration>(configurationOptions.OpenIdConnectConfiguration);
            Assert.IsType<LogglyClientConfiguration>(configurationOptions.LogglyClientConfiguration);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Get_Client_Configuration_Mock()
        {
            var controller = GetClientAppSettingsController();

            // Act
            var result = controller.Get();

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
            Assert.NotNull(objectResult.Value);
        }

        private ClientAppSettingsController GetClientAppSettingsController()
        {
            // Application Options.
            var applicationOptionsMock = new Mock<IOptions<ApplicationOptions>>();
            applicationOptionsMock.Setup(x => x.Value).Returns(new ApplicationOptions
            {
                ClientConfiguration = new ClientConfiguration { ApiAttractionsUri = "Test" },
                OpenIdConnectConfiguration = new OpenIdConnectConfiguration { ClientId = "Test" },
                LogglyClientConfiguration = new LogglyClientConfiguration { LogglyKey = "Test" },
            });

            var settings = applicationOptionsMock.Object;

            var logglyClient = new Mock<ILogglyClient>().Object;

            return new ClientAppSettingsController(settings, logglyClient);
        }
    }
}
