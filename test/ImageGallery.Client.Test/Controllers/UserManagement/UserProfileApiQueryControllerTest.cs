using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ImageGallery.Client.Apis.UserManagement;
using ImageGallery.Client.HttpClients;
using ImageGallery.Client.Test.Data;
using ImageGallery.Client.Test.Helpers;
using ImageGallery.Client.ViewModels.UserManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace ImageGallery.Client.Test.Controllers.UserManagement
{
    public class UserProfileApiQueryControllerTest
    {
        private readonly ITestOutputHelper _output;

        public UserProfileApiQueryControllerTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Get_UserProfile_ReturnsData()
        {
            var userProfile = UserProfileDataSet.GetUserProfile();
            var content = JsonConvert.SerializeObject(userProfile);
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.OK, content);

            var controller = GetUserProfileApiQueryController(httpRespose);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.Get();

            // Assert
            Assert.IsType<UserProfileViewModel>(userProfile);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 200);

            Assert.IsType<UserProfileViewModel>(objectResult.Value);
            var userProfileModel = objectResult.Value as UserProfileViewModel;

            Assert.NotNull(userProfileModel);

            _output.WriteLine($"FirstName:{userProfileModel.FirstName}");
            Assert.Equal(userProfile.FirstName, userProfileModel.FirstName);

            _output.WriteLine($"LastName:{userProfileModel.LastName}");
            Assert.Equal(userProfile.LastName, userProfileModel.LastName);

            _output.WriteLine($"Address:{userProfileModel.Address}");
            Assert.Equal(userProfile.Address, userProfileModel.Address);

            _output.WriteLine($"Address2:{userProfileModel.Address2}");
            Assert.Equal(userProfile.Address2, userProfileModel.Address2);

            _output.WriteLine($"City:{userProfileModel.City}");
            Assert.Equal(userProfile.City, userProfileModel.City);

            _output.WriteLine($"State:{userProfileModel.State}");
            Assert.Equal(userProfile.State, userProfileModel.State);

            _output.WriteLine($"PostalCode:{userProfileModel.PostalCode}");
            Assert.Equal(userProfile.PostalCode, userProfileModel.PostalCode);

            _output.WriteLine($"Country:{userProfileModel.Country}");
            Assert.Equal(userProfile.Country, userProfileModel.Country);

            _output.WriteLine($"Language:{userProfileModel.Language}");
            Assert.Equal(userProfile.Language, userProfileModel.Language);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Get_UserProfile_Returns_Api_Unauthorized()
        {
            var userProfile = UserProfileDataSet.GetUserProfile();
            var content = JsonConvert.SerializeObject(userProfile);
            var httpRespose = MockHelpers.SetHttpResponseMessage(HttpStatusCode.Unauthorized, content);

            var controller = GetUserProfileApiQueryController(httpRespose);
            controller.ControllerContext = WebTestHelpers.GetHttpContextWithUser();

            // Act
            var result = await controller.Get();

            // Assert
            Assert.IsType<UserProfileViewModel>(userProfile);
            Assert.NotNull(result);
            Assert.IsType<UnauthorizedResult>(result);

            var objectResult = result as UnauthorizedResult;
            Assert.NotNull(objectResult);
            Assert.True(objectResult.StatusCode == 401);
        }

        private UserProfileApiQueryController GetUserProfileApiQueryController(
            HttpResponseMessage responseMessage,
            UserManagementHttpClient client = null,
            ILogger<UserProfileApiQueryController> logger = null)
        {
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

            client = client ?? new UserManagementHttpClient(httpClient);
            logger = logger ?? new Mock<ILogger<UserProfileApiQueryController>>().Object;

            return new UserProfileApiQueryController(client, logger);
        }
    }
}
