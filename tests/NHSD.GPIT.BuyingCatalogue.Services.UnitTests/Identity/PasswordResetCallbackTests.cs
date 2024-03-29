﻿using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Services.Identity;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Builders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
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
                Mock.Of<LinkGenerator>(),
                new DomainNameSettings()));
        }

        [Fact]
        public static void Constructor_IHttpContextAccessor_LinkGenerator_NullGenerator_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new PasswordResetCallback(
                Mock.Of<IHttpContextAccessor>(),
                null,
                new DomainNameSettings()));
        }

        [Fact]
        public static void GetPasswordResetCallback_NullToken_ThrowsException()
        {
            var callback = new PasswordResetCallback(
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<LinkGenerator>(),
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
            private readonly Mock<IHttpContextAccessor> mockAccessor = new();
            private readonly Mock<LinkGenerator> mockGenerator = new();
            private readonly DomainNameSettings domainNameSettings = new();

            internal PasswordResetCallbackContext(string url)
            {
                var mockRequest = new Mock<HttpRequest>();
                mockRequest.Setup(r => r.Scheme).Returns("https");
                domainNameSettings.DomainName = url;

                var mockContext = new Mock<HttpContext>();
                mockContext.Setup(c => c.Request).Returns(mockRequest.Object);
                mockContext.Setup(c => c.Features).Returns(new FeatureCollection());

                mockAccessor.Setup(a => a.HttpContext).Returns(mockContext.Object);
                mockGenerator.Setup(g => g.GetUriByAddress(
                        It.IsAny<HttpContext>(),
                        It.IsAny<RouteValuesAddress>(),
                        It.IsAny<RouteValueDictionary>(),
                        It.IsAny<RouteValueDictionary>(),
                        It.IsAny<string>(),
                        It.IsAny<HostString?>(),
                        It.IsAny<PathString?>(),
                        It.IsAny<FragmentString>(),
                        It.IsAny<LinkOptions>()))
                    .Callback<
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
            }

            internal PasswordResetCallback Callback => new(
                mockAccessor.Object,
                mockGenerator.Object,
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
