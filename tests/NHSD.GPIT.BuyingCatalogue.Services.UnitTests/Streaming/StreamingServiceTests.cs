using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Services.Streaming;
using RichardSzalay.MockHttp;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Streaming
{
    public static class StreamingServiceTests
    {
        private static readonly Uri Uri = new("http://www.test.com");

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(StreamingService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadGateway)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        public static async Task StreamContents_HttpClientReturnsInvalidStatusCode_ReturnsNull(HttpStatusCode statusCode)
        {
            var mockMessageHandler = new MockHttpMessageHandler();

            mockMessageHandler
                .When(Uri.AbsolutePath)
                .Respond(statusCode);

            var systemUnderTest = new StreamingService(new HttpClient(mockMessageHandler));

            var result = await systemUnderTest.StreamContents(Uri);

            result.Should().BeNull();
        }

        [Fact]
        public static async Task StreamContents_HttpClientReturnsOk_ReturnsStream()
        {
            var mockMessageHandler = new MockHttpMessageHandler();

            mockMessageHandler
                .When(Uri.AbsolutePath)
                .Respond(_ => new HttpResponseMessage(HttpStatusCode.OK));

            var systemUnderTest = new StreamingService(new HttpClient(mockMessageHandler));

            var result = await systemUnderTest.StreamContents(Uri);

            result.Should().NotBeNull();
        }
    }
}
