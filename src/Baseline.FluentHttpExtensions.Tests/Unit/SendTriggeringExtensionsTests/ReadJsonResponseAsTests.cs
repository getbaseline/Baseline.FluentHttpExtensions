using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

// ReSharper disable NotNullMemberIsNotInitialized
// ReSharper disable UnusedAutoPropertyAccessor.Local
#pragma warning disable 8618

namespace Baseline.FluentHttpExtensions.Tests.Unit.SendTriggeringExtensionsTests
{
    public class ReadJsonResponseAsTests : UnitTest
    {
        [Fact]
        public async Task Checks_Success_Status_First()
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            ConfigureMessageHandlerResultFailure(mockMessageHandler);
            var request = new HttpRequest(RequestUrl, new HttpClient(mockMessageHandler.Object));

            Func<Task> func = async () => await request.ReadJsonResponseAsAsync<object>();

            await func.Should().ThrowExactlyAsync<HttpRequestException>();
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class ResolvedObject
        {
            public string Name { get; set; }
        }

        [Theory]
        [InlineData(@"{ ""Name"": ""bob smith"" }")]
        [InlineData(@"{ ""name"": ""bob smith"" }")]
        public async Task Successfully_Resolves_Into_An_Object(string json)
        {
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            ConfigureMessageHandlerResultSuccess(mockMessageHandler, json);
            var request = new HttpRequest(RequestUrl, new HttpClient(mockMessageHandler.Object));

            var response = await request.ReadJsonResponseAsAsync<ResolvedObject>();

            response.Should().NotBeNull();
            response.Name.Should().Be("bob smith");
        }
    }
}
