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
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
        public static void IsValidUrl_InvalidUrl_ThrowsArgumentException(
            string url,
            UrlValidator validator)
            => Assert.Throws<ArgumentException>(() => validator.IsValidUrl(url));

        [Theory]
        [CommonAutoData]
        public static void IsValidUrl_InvalidDomain_ReturnsFalse(
            Uri uri,
            [Frozen] Mock<HttpMessageHandler> handler,
            UrlValidator validator)
        {
            handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound });

            var result = validator.IsValidUrl(uri.ToString());

            result.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void IsValidUrl_Timeout_ReturnsFalse(
            Uri uri,
            [Frozen] Mock<HttpMessageHandler> handler,
            UrlValidator validator)
        {
            handler
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                   .ThrowsAsync(new TaskCanceledException());

            var result = validator.IsValidUrl(uri.ToString());

            result.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void IsValidUrl_HttpRequestException_ReturnsFalse(
            Uri uri,
            [Frozen] Mock<HttpMessageHandler> handler,
            UrlValidator validator)
        {
            handler
                   .Protected()
                   .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                   .ThrowsAsync(new HttpRequestException());

            var result = validator.IsValidUrl(uri.ToString());

            result.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void IsValidUrl_ValidDomain_ReturnsTrue(
            Uri uri,
            [Frozen] Mock<HttpMessageHandler> handler,
            UrlValidator validator)
        {
            handler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });

            var result = validator.IsValidUrl(uri.ToString());

            result.Should().BeTrue();
        }
    }
}
