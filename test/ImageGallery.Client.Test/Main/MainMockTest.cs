using System;
using System.IO;
using ImageGallery.Client.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace ImageGallery.Client.Test.Main
{
    public class MainMockTest
    {
        [Theory]
        [Trait("Category", "Unit")]
        [InlineData("Testing")]
        [InlineData("Development")]
        public void ConfigureServices_RegistersDependenciesCorrectly(string environment)
        {
            // Arrange
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);
            var mockEnvironment = new Mock<IHostingEnvironment>();
            mockEnvironment.Setup(m => m.EnvironmentName).Returns(environment);

            IServiceCollection services = new ServiceCollection();
            var target = new Startup(Configuration, mockEnvironment.Object);

            // Act
            target.ConfigureServices(services);

            services.AddTransient<ClientAppSettingsController>();

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            var controller = serviceProvider.GetService<ClientAppSettingsController>();
            Assert.NotNull(controller);
        }

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            // .SetBasePath(Directory.GetCurrentDirectory())
            .SetBasePath(Path.GetFullPath(@"../../../../../src/ImageGallery.Client"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .Build();
    }
}
