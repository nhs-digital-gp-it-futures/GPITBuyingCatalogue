using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Services.Identity;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Builders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Identity
{
    public static class PasswordResetCallbackTests
    {
        [Fact]
        public static void Constructor_IHttpContextAccessor_LinkGenerator_NullAccessor_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new PasswordResetCallback(
                null,
                Substitute.For<LinkGenerator>(),
                new DomainNameSettings()));
        }

        [Fact]
        public static void Constructor_IHttpContextAccessor_LinkGenerator_NullGenerator_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new PasswordResetCallback(
                Substitute.For<IHttpContextAccessor>(),
                null,
                new DomainNameSettings()));
        }

        [Fact]
        public static void GetPasswordResetCallback_NullToken_ThrowsException()
        {
            var callback = new PasswordResetCallback(
                Substitute.For<IHttpContextAccessor>(),
                Substitute.For<LinkGenerator>(),
                new DomainNameSettings());

            Assert.Throws<ArgumentNullException>(() => callback.GetPasswordResetCallback(null));
        }

        [Fact]
        public static void GetPasswordResetCallback_GetsExpectedAction()
        {
            const string expectedToken = "IAmBecomeToken";
            var expectedValues = new RouteValueDictionary
            {
                { "Email", "a.b@c.com" },
                { "Token", expectedToken },
                { "action", nameof(AccountController.ResetPassword) },
                { "controller", "Account" },
                { "Area", "Identity" },
            };

            var context = new PasswordResetCallbackContext("https://www.google.co.uk/");
            var callback = context.Callback;

            callback.GetPasswordResetCallback(
                new PasswordResetToken(expectedToken, AspNetUserBuilder.Create().Build()));

            context.RouteValues.Should().BeEquivalentTo(expectedValues);
        }

        [Fact]
        public static void GetPasswordResetCallback_ReturnsExpectedValue()
        {
            const string url = "https://nhs.uk/reset";

            var expectedUri = new Uri(url);

            var context = new PasswordResetCallbackContext(url);
            var callback = context.Callback;

            var actualUri = callback.GetPasswordResetCallback(
                new PasswordResetToken("Token", AspNetUserBuilder.Create().Build()));

            actualUri.Should().Be(expectedUri);
        }

        private sealed class PasswordResetCallbackContext
        {
            private readonly IHttpContextAccessor mockAccessor = Substitute.For<IHttpContextAccessor>();
            private readonly LinkGenerator mockGenerator = Substitute.For<LinkGenerator>();
            private readonly DomainNameSettings domainNameSettings = new();

            internal PasswordResetCallbackContext(string url)
            {
                var mockRequest = Substitute.For<HttpRequest>();
                mockRequest.Scheme.Returns("https");
                domainNameSettings.DomainName = url;

                var mockContext = Substitute.For<HttpContext>();
                mockContext.Request.Returns(mockRequest);
                mockContext.Features.Returns(new FeatureCollection());

                mockAccessor.HttpContext.Returns(mockContext);
                mockGenerator.GetUriByAddress(
                        Arg.Any<HttpContext>(),
                        Arg.Any<RouteValuesAddress>(),
                        Arg.Any<RouteValueDictionary>(),
                        Arg.Any<RouteValueDictionary>(),
                        Arg.Any<string>(),
                        Arg.Any<HostString?>(),
                        Arg.Any<PathString?>(),
                        Arg.Any<FragmentString>(),
                        Arg.Any<LinkOptions>())
                    .Returns(Callback<
                        HttpContext,
                        RouteValuesAddress,
                        RouteValueDictionary,
                        RouteValueDictionary,
                        string,
                        HostString?,
                        PathString?,
                        FragmentString,
                        LinkOptions>(GetUriByAddressCallback)
                    .Returns(url);
                //.Do(x => GetUriByAddressCallback(x.Configure))

            }

            internal PasswordResetCallback Callback => new(
                mockAccessor,
                mockGenerator,
                domainNameSettings);

            internal RouteValueDictionary RouteValues { get; private set; }

            private void GetUriByAddressCallback(
                HttpContext httpContext,
                RouteValuesAddress address,
                RouteValueDictionary values,
                RouteValueDictionary ambientValues,
                string scheme,
                HostString? host,
                PathString? pathBase,
                FragmentString fragment,
                LinkOptions options)
            {
                RouteValues = values;
            }
        }
    }
}
