using System;
using System.IO;
using ImageGallery.Client.Controllers;
using ImageGallery.Client.ViewModels.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ImageGallery.Client.Test.TestServerIntegration
{
    public class DiagnosticsControllerTest
    {
        [Fact]
        public void Get_ServerDiagnostics_ReturnsData()
        {
            // Arange
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "test");
            var mockEnvironment = new Mock<IHostingEnvironment>();
            mockEnvironment.Setup(m => m.EnvironmentName).Returns("Test");

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
            Assert.Equal("Test", serverDiagnostics.EnvironmentName);
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
