using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ImageGallery.Client.Test.Helpers
{
    public class TestPrincipal : ClaimsPrincipal
    {
        public TestPrincipal(params Claim[] claims)
            : base(new TestIdentity(claims)) { }
    }

    public class TestIdentity : ClaimsIdentity
    {
        public TestIdentity(params Claim[] claims) : base(claims) { }
    }

    public static class WebTestHelpers
    {
        public static ClaimsPrincipal GetClaimsPrincipal()
        {
            return new TestPrincipal
            (
            new Claim("role", "PayingUser"),
            new Claim("subscriptionlevel", "PayingUser"),
            new Claim(ClaimTypes.Country, "be"),
            new Claim(ClaimTypes.GivenName, "Claire"),
            new Claim("address", "Main Street"),
            new Claim("sub", "b7539694-97e7-4dfe-84da-b4256e1ff5c7")
          );
        }

        public static ControllerContext GetHttpContextWithUser()
        {
            var principal = GetClaimsPrincipal();

            var contextMock = new Mock<HttpContext>();
            contextMock.SetupGet(ctx => ctx.User)
                .Returns(principal);
            var controllerContextMock = new Mock<ControllerContext>();
            var controllerContext = controllerContextMock.Object;
            controllerContext.HttpContext = contextMock.Object;
            return controllerContext;
        }

        public static string GetWebApplicationPath()
        {
            string appPath = Directory.GetCurrentDirectory();
            string webPath = @"../../../../../src/ImageGallery.Client";
            string path = Path.GetFullPath(Path.Combine(appPath, webPath));

            return path;
        }
    }
}
