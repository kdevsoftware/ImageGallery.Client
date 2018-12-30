using System.IO;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Moq;

namespace ImageGallery.Client.Test.Helpers
{
    public static class MockHelpers
    {
        public static Mock<IFormFile> GetMockIFormFile()
        {
            var fileMock = new Mock<IFormFile>();

            var content = "Hello World from a Fake File";
            var fileName = "test.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            return fileMock;
        }

        public static HttpResponseMessage SetHttpResponseMessage(HttpStatusCode statusCode, string responseContent = null)
        {
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = statusCode, //HttpStatusCode.OK,
                Content = responseContent != null ? new StringContent(responseContent) : null,
            };

            return httpResponseMessage;
        }

    }
}
