using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Services.Validation;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Validation
{
    public static class UrlValidatorTests
    {
        [Fact]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(UrlValidator).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
        [MockInlineAutoData(" ")]
        public static void IsValidUrl_InvalidUrl_ThrowsArgumentException(
            string url,
            UrlValidator validator)
            => Assert.Throws<ArgumentException>(() => validator.IsValidUrl(url));

        [Theory]
        [MockAutoData]
        public static void IsValidUrl_InvalidDomain_ReturnsFalse(
            Uri uri,
            [Frozen] HttpMessageHandler handler,
            UrlValidator validator)
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            handler.GetType().GetMethod("SendAsync", flags)!
                .Invoke(handler, new object[] { Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>() })
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)));

            var result = validator.IsValidUrl(uri.ToString());

            result.Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void IsValidUrl_Timeout_ReturnsFalse(
            Uri uri,
            [Frozen] HttpMessageHandler handler,
            UrlValidator validator)
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            handler.GetType().GetMethod("SendAsync", flags)!
                .Invoke(handler, new object[] { Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>() })
                .Throws(new TaskCanceledException());

            var result = validator.IsValidUrl(uri.ToString());

            result.Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void IsValidUrl_HttpRequestException_ReturnsFalse(
            Uri uri,
            [Frozen] HttpMessageHandler handler,
            UrlValidator validator)
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            handler.GetType().GetMethod("SendAsync", flags)!
                .Invoke(handler, new object[] { Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>() })
                .Throws(new HttpRequestException());

            var result = validator.IsValidUrl(uri.ToString());

            result.Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void IsValidUrl_ValidDomain_ReturnsTrue(
            Uri uri,
            [Frozen] HttpMessageHandler handler,
            UrlValidator validator)
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            handler.GetType().GetMethod("SendAsync", flags)!
                .Invoke(handler, new object[] { Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>() })
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)));

            var result = validator.IsValidUrl(uri.ToString());

            result.Should().BeTrue();
        }
    }
}
