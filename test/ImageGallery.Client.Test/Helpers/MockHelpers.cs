﻿using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Protected;

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

        public static HttpResponseMessage SetHttpResponseMessage(HttpStatusCode statusCode, string responseContent = null, List<KeyValuePair<string, string>> headers = null)
        {
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = responseContent != null ? new StringContent(responseContent) : null,
            };

            if (headers != null)
            {
                foreach (var res in headers)
                {
                    httpResponseMessage.Headers.Add(res.Key, res.Value);
                }
            }

            return httpResponseMessage;
        }

        public static Mock<HttpMessageHandler> GetHttpMessageHandlerMock(HttpResponseMessage responseMessage)
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage)
                .Verifiable();

            return httpMessageHandlerMock;
        }
    }
}
