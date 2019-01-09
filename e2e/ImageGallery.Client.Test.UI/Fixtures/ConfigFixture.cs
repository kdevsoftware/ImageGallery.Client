using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ImageGallery.Client.Test.UI.Fixtures
{
    public class ConfigFixture : IDisposable
    {
        public ConfigFixture()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            Console.WriteLine("Config-ENV:" + env);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            string appPath = Directory.GetCurrentDirectory();
            string artifactsPath = @"../../../../../artifacts";

            var appSetting = Configuration.GetSection("ApplicationSettings");
            ApplicationUrl = appSetting["ApplicationUrl"];

            // Create Artifacts Directory
            ArtifactsDirectory = Path.GetFullPath(Path.Combine(appPath, artifactsPath));

            Navigator.Common.Helpers.FileHelper.CheckFilePath(ArtifactsDirectory);
            Console.WriteLine($"ArtifactsDirectory:{ArtifactsDirectory}");
        }

        public string ArtifactsDirectory { get; private set; }

        public string ApplicationUrl { get; private set; }

        private IConfiguration Configuration { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
