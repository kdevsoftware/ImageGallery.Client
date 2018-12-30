using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ImageGallery.Client.Test.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Net.Http.Headers;

namespace ImageGallery.Client.Test.Fixtures
{
    public class TestServerFixture : IDisposable
    {
        public static readonly string AntiForgeryFieldName = "__AFTField";

        public static readonly string AntiForgeryCookieName = "AFTCookie";

        private readonly Microsoft.AspNetCore.TestHost.TestServer _testServer;

        public TestServerFixture()
        {
            var environment = "Testing";
            var applicationPath = WebTestHelpers.GetWebApplicationPath();

            _testServer = new Microsoft.AspNetCore.TestHost.TestServer(new WebHostBuilder()
                .UseEnvironment(environment)
                .UseContentRoot(applicationPath)
                .ConfigureServices(x =>
                {
                    x.AddAntiforgery(t =>
                    {
                        t.Cookie.Name = AntiForgeryCookieName;
                        t.FormFieldName = AntiForgeryFieldName;
                    });
                })
                .UseConfiguration(new ConfigurationBuilder()
                    .SetBasePath(applicationPath)
                    .AddJsonFile($"appsettings.{environment}.json")
                    .Build())
                .UseStartup<Startup>());

            Client = _testServer.CreateClient();
        }

        public HttpClient Client { get; }

        public async Task<(string fieldValue, string cookieValue)> ExtractAntiForgeryValues(HttpResponseMessage response)
        {
            return (ExtractAntiForgeryToken(await response.Content.ReadAsStringAsync()),
                                            ExtractAntiForgeryCookieValueFrom(response));
        }

        private string GetContentRootPath()
        {
            string testProjectPath = PlatformServices.Default.Application.ApplicationBasePath;

            var relativePathToWebProject = @"..\..\..\..\..\src\ImageGallery.Client";

            return Path.Combine(testProjectPath, relativePathToWebProject);
        }

        private string ExtractAntiForgeryCookieValueFrom(HttpResponseMessage response)
        {
            string antiForgeryCookie =
                        response.Headers
                                .GetValues("Set-Cookie")
                                .FirstOrDefault(x => x.Contains(AntiForgeryCookieName));

            if (antiForgeryCookie is null)
            {
                throw new ArgumentException(
                    $"Cookie '{AntiForgeryCookieName}' not found in HTTP response",
                    nameof(response));
            }

            string antiForgeryCookieValue = SetCookieHeaderValue.Parse(antiForgeryCookie).Value.ToString();

            return antiForgeryCookieValue;
        }

        private string ExtractAntiForgeryToken(string htmlBody)
        {
            var requestVerificationTokenMatch =
                Regex.Match(htmlBody, $@"\<input name=""{AntiForgeryFieldName}"" type=""hidden"" value=""([^""]+)"" \/\>");

            if (requestVerificationTokenMatch.Success)
            {
                return requestVerificationTokenMatch.Groups[1].Captures[0].Value;
            }

            throw new ArgumentException($"Anti forgery token '{AntiForgeryFieldName}' not found in HTML", nameof(htmlBody));
        }

        public void Dispose()
        {
            Client.Dispose();
            _testServer.Dispose();
        }
    }
}
