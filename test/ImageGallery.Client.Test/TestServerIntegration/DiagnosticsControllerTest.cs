﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ImageGallery.Client.Controllers;
using ImageGallery.Client.Test.Fixtures;
using ImageGallery.Client.ViewModels.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.Client.Test.TestServerIntegration
{
    public class DiagnosticsControllerTest : IClassFixture<TestServerFixture>
    {
        private readonly HttpClient _client;

        private readonly ITestOutputHelper _output;

        public DiagnosticsControllerTest(TestServerFixture fixture, ITestOutputHelper output)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
            _client = fixture.Client;
            _output = output;
        }

        [Fact]
        [Trait("Category", "Intergration")]
        public async Task Get_Diagnostics_TestServer()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/Diagnostics");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var serverDiagnostics = JsonConvert.DeserializeObject<ServerDiagnostics>(responseString);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.IsType<ServerDiagnostics>(serverDiagnostics);
        }

        [Fact]
        [Trait("Category", "Intergration")]
        public async Task Get_Diagnostics_Status_TestServer()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/Diagnostics/status");
            response.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Get_ServerDiagnostics_ReturnsData()
        {
            // Arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
            var mockEnvironment = new Mock<IHostingEnvironment>();
            mockEnvironment.Setup(m => m.EnvironmentName).Returns("Testing");

            var diagnosticsController = GetDiagnosticsController(mockEnvironment.Object);

            // Act
            var result = diagnosticsController.Get();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
            Assert.NotNull(objectResult.Value);

            var serverDiagnostics = objectResult.Value as ServerDiagnostics;

            Assert.NotNull(serverDiagnostics);
            Assert.IsType<ServerDiagnostics>(serverDiagnostics);
            Assert.Equal("Testing", serverDiagnostics.EnvironmentName);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Get_ServerDiagnostics_Returns_Status()
        {
            // Arrange
            var diagnosticsController = GetDiagnosticsController();

            // Act
            var result = diagnosticsController.Status();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);

            var objectResult = result as OkResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);
        }

        private DiagnosticsController GetDiagnosticsController(
            IHostingEnvironment hostingEnvironment = null)
        {
            hostingEnvironment = hostingEnvironment ?? new Mock<IHostingEnvironment>().Object;
            var logger = new Mock<ILogger<DiagnosticsController>>().Object;

            return new DiagnosticsController(hostingEnvironment, Configuration, logger);
        }

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .Build();
    }
}
