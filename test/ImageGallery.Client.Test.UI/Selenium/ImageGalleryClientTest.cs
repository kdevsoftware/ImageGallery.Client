using System;
using System.IO;
using System.Reflection;
using ImageGallery.Client.Test.UI.Fixtures;
using ImageGallery.Client.Test.UI.Fixtures.TestData;
using ImageGallery.Client.Test.UI.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.Client.Test.UI.Selenium
{
    public class ImageGalleryClientTest : IClassFixture<ConfigFixture>, IDisposable
    {
        private const string BasicUserName = "Frank";
        private const string BasicUserPassword = "password";
        private const int BasicUserTotalPhotos = 6;
        private const string PrivilegedUserName = "Claire";
        private const string PrivilegedUserPassword = "password";

        private const string IncorrectPassword = "WRONG_PASSWORD";
        private const string LoginRequiredMessage = "The Username field is required.";
        private const string PasswordRequiredessage = "The Password field is required.";
        private const string InvalidLoginMessage = "Invalid username or password";
        private const string LoginPageTitle = "Identity";

        private readonly IWebDriver _driver;

        private readonly ITestOutputHelper _output;

        private readonly string _artifactsDirectory;

        private readonly string _applicationUrl;

        public ImageGalleryClientTest(ConfigFixture config, ITestOutputHelper output)
        {
            SeleniumFixture fixture = new SeleniumFixture();
            _artifactsDirectory = config.ArtifactsDirectory;
            _applicationUrl = config.ApplicationUrl;
            _driver = fixture.Driver;
            _output = output;
        }

        [Fact]
        [Trait("Category", "Intergration")]
        public void ShouldLoadApplicationPage_SmokeTest()
        {
            _driver.Navigate().GoToUrl("https://www.google.com/webhp?ie=utf-8&oe=utf-8");

            Screenshot ss = ((ITakesScreenshot)_driver).GetScreenshot();

            string filePath = Path.Combine(_artifactsDirectory, $"Selenium_Smoke_Test_{DateTime.Now.Ticks}.png");
            ss.SaveAsFile(filePath, ScreenshotImageFormat.Png);

            Assert.Equal("Google", _driver.Title);
        }

        [Fact]
        [Trait("Category", "UI")]
        public void BasicUserLoginTest()
        {
            using (var galleryPage = new GalleryPage(_driver, _applicationUrl))
            {
                galleryPage.Login(BasicUserName, BasicUserPassword);
                TakeScreenshot(galleryPage);

                Assert.False(
                    galleryPage.IsAddImageButtonAvailable(),
                    "User is logged in as free user, so 'Add Image' button should not be available.");
            }
        }

        [Fact]
        [Trait("Category", "UI")]
        public void BasicUserLogoutTest()
        {
            using (var galleryPage = new GalleryPage(_driver, _applicationUrl))
            {
                galleryPage.Login(BasicUserName, BasicUserPassword);
                galleryPage.LogoutAndWait();
                TakeScreenshot(galleryPage);

                // Assert.Contains(LoginPageTitle, galleryPage.Title);
            }
        }

        [Fact]
        [Trait("Category", "UI")]
        public void PrivilegedUserLoginTest()
        {
            using (var galleryPage = new GalleryPage(_driver, _applicationUrl))
            {
                galleryPage.Login(PrivilegedUserName, PrivilegedUserPassword);
                TakeScreenshot(galleryPage);

                Assert.True(
                    galleryPage.IsAddImageButtonAvailable(),
                    "User is logged in with elevated permissions, so 'Add Image' button should be available.");
            }
        }

        [Fact]
        [Trait("Category", "UI")]
        public void PrivilegedUserLogoutTest()
        {
            using (var galleryPage = new GalleryPage(_driver, _applicationUrl))
            {
                galleryPage.Login(PrivilegedUserName, PrivilegedUserPassword);
                galleryPage.LogoutAndWait();
                TakeScreenshot(galleryPage);

                //Assert.Contains(LoginPageTitle, galleryPage.Title);
            }
        }

        [Fact]
        [Trait("Category", "UI")]
        public void EmptyUsernamePasswordTest()
        {
            using (var galleryPage = new GalleryPage(_driver, _applicationUrl))
            {
                galleryPage.Login(string.Empty, string.Empty);
                TakeScreenshot(galleryPage);

                var validationText = galleryPage.GetValidationErrorText();

                Assert.Contains(LoginRequiredMessage, validationText);
                Assert.Contains(PasswordRequiredessage, validationText);
            }
        }

        [Fact]
        [Trait("Category", "UI")]
        public void IncorrectLoginAttemptTest()
        {
            using (var galleryPage = new GalleryPage(_driver, _applicationUrl))
            {
                galleryPage.Login(BasicUserName, IncorrectPassword);
                TakeScreenshot(galleryPage);

                var validationText = galleryPage.GetValidationErrorText();

                Assert.Equal(InvalidLoginMessage, validationText);
            }
        }

        [Fact]
        [Trait("Category", "UI")]
        public void GetUsersTotalPhotosCountTest()
        {
            var totalPhotosCount = BasicUserTotalPhotos;
            using (var galleryPage = new GalleryPage(_driver, _applicationUrl))
            {
                galleryPage.Login(BasicUserName, BasicUserPassword);
                TakeScreenshot(galleryPage);
                var totalRecordMessage = galleryPage.GetTotalRecordsMessage();
                int photosCountNumericPart;
                totalRecordMessage = totalRecordMessage.Substring(1 + totalRecordMessage.LastIndexOf(':'));
                Assert.True(int.TryParse(totalRecordMessage, out photosCountNumericPart));
                var totalPhotosActualCount = Convert.ToInt32(photosCountNumericPart);
                Assert.Equal(totalPhotosCount, totalPhotosActualCount);
            }
        }

        [Theory]
        [Trait("Category", "UI")]
        [InlineData("William", "password", @"Data\images\bears.jpg", "Bears", "Landscapes")]
        public void PrivilegedUserAddPhoto(
            string userName,
            string password,
            string imageFilePath,
            string imageTitle,
            string imageType)
        {
            var imageFullPath = Path.Combine(GetBaseDirectory(), imageFilePath);
            using (var galleryPage = new GalleryPage(_driver, _applicationUrl))
            {
                galleryPage.Login(userName, password);
                galleryPage.AddImageToGallery(imageTitle, imageType, imageFullPath);
                TakeScreenshot(galleryPage);

                var successMessage = galleryPage.GetSuccessMessage();
                Assert.Equal("Image has been added successfully!", successMessage);

                //var totalRecordsActual = galleryPage.GetTotalRecordsMessage();
                //Assert.Equal("Total Records: 1", totalRecordsActual);

                //galleryPage.DeleteImageByTitle(imageTitle);
                //successMessage = galleryPage.GetSuccessMessage();
                //Assert.Equal("Image has been deleted successfully!", successMessage);
            }
        }

        [Theory]
        [UserDataCsvData(FileName = "Data/users.csv")]
        [Trait("Category", "UI")]
        public void UserRolesTest(string userName, string password, string role)
        {
            using (var galleryPage = new GalleryPage(_driver, _applicationUrl))
            {
                galleryPage.Login(userName, password);
                TakeScreenshot(galleryPage);
                string actualRole = GetRole(galleryPage);

                Assert.Equal(role, actualRole);
            }
        }

        [Theory]
        [ImageDataCsvData(FileName = "Data/images.csv")]
        [Trait("Category", "UI")]
        public void GalleryImageAddRemoveTest(
            string userName,
            string password,
            string imageTitle,
            string imageType,
            string imageFilePath)
        {
            var imageFullPath = Path.Combine(GetBaseDirectory(), imageFilePath);
            using (var galleryPage = new GalleryPage(_driver, _applicationUrl))
            {
                galleryPage.Login(userName, password);
                galleryPage.AddImageToGallery(imageTitle, imageType, imageFullPath);
                TakeScreenshot(galleryPage);
                var successMessage = galleryPage.GetSuccessMessage();
                Assert.Equal("Image has been added successfully!", successMessage);
            }
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver?.Dispose();
        }

        private string GetRole(GalleryPage galleryPage)
        {
            return galleryPage.IsAddImageButtonAvailable() ? "FreeUser" : "PayingUser";
        }

        private string GetBaseDirectory()
        {
            var assemblyName = Assembly.GetExecutingAssembly().Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyName);
            return assemblyDirectory;
        }

        private void TakeScreenshot(BasePage page)
        {
            var filePath = Path.Combine(_artifactsDirectory, $"Selenium_Smoke_Test_{DateTime.Now.Ticks}.png");
            page.SaveScreenshotAs(filePath, ScreenshotImageFormat.Png);
        }
    }
}
