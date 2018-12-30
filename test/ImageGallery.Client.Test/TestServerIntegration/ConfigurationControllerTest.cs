using System;
using System.Net.Http;
using System.Threading.Tasks;
using ImageGallery.Client.Configuration;
using ImageGallery.Client.Test.Fixtures;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.Client.Test.TestServerIntegration
{
    public class ConfigurationControllerTest : IClassFixture<TestServerFixture>
    {
        /*
        https://andrewlock.net/introduction-to-integration-testing-with-xunit-and-testserver-in-asp-net-core/
        https://visualstudiomagazine.com/articles/2017/07/01/testserver.aspx
        https://ardalis.com/configuring-aspnet-core-apps-with-webhostbuilder
        https://github.com/ardalis/ConfigureWithWebHostBuilder
        */

        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public ConfigurationControllerTest(TestServerFixture fixture, ITestOutputHelper output)
        {
            _client = fixture.Client;
            _output = output;
        }

        [Fact(Skip = "TODO")]
        [Trait("Category", "Intergration")]
        public async Task Get_Client_Configuration()
        {
            var response = await _client.GetAsync("/api/ClientAppSettings");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var configurationOptions = JsonConvert.DeserializeObject<ApplicationOptions>(responseString);

            Console.WriteLine(responseString);
            _output.WriteLine(responseString);

            Assert.NotNull(configurationOptions);
            Assert.IsType<ApplicationOptions>(configurationOptions);
        }
    }
}
