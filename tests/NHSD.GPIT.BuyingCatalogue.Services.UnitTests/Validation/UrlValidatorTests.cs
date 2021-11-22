using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NHSD.GPIT.BuyingCatalogue.Services.Validation;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Validation
{
    public static class UrlValidatorTests
    {
        [Fact]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(UrlValidator).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static Task IsValidUrl_InvalidUrl_ThrowsArgumentException(
            string url,
            UrlValidator validator)
            => Assert.ThrowsAsync<ArgumentException>(() => validator.IsValidUrl(url));

        [Theory]
        [CommonAutoData]
        public static async Task IsValidUrl_InvalidDomain_ReturnsFalse(
            Uri uri,
            [Frozen] Mock<HttpMessageHandler> handler,
            UrlValidator validator)
        {
            handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound });

            var result = await validator.IsValidUrl(uri.ToString());

            result.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static async Task IsValidUrl_Timeout_ReturnsFalse(
            Uri uri,
            [Frozen] Mock<HttpMessageHandler> handler,
            UrlValidator validator)
        {
            handler
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                   .ThrowsAsync(new TaskCanceledException());

            var result = await validator.IsValidUrl(uri.ToString());

            result.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static async Task IsValidUrl_HttpRequestException_ReturnsFalse(
            Uri uri,
            [Frozen] Mock<HttpMessageHandler> handler,
            UrlValidator validator)
        {
            handler
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                   .ThrowsAsync(new HttpRequestException());

            var result = await validator.IsValidUrl(uri.ToString());

            result.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static async Task IsValidUrl_ValidDomain_ReturnsTrue(
            Uri uri,
            [Frozen] Mock<HttpMessageHandler> handler,
            UrlValidator validator)
        {
            handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });

            var result = await validator.IsValidUrl(uri.ToString());

            result.Should().BeTrue();
        }
    }
}
