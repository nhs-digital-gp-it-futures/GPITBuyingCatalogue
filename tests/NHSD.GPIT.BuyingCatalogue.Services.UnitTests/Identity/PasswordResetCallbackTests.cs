using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.Services.Identity;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.Builders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Controllers;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Identity
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class PasswordResetCallbackTests
    {
        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public static void Constructor_IHttpContextAccessor_LinkGenerator_NullAccessor_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new PasswordResetCallback(
                null,
                Mock.Of<LinkGenerator>(),
                new Framework.Settings.IssuerSettings()));
        }

        [Test]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "Exception testing")]
        public static void Constructor_IHttpContextAccessor_LinkGenerator_NullGenerator_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new PasswordResetCallback(
                Mock.Of<IHttpContextAccessor>(),
                null,
                new Framework.Settings.IssuerSettings()));
        }

        [Test]
        public static void GetPasswordResetCallback_NullToken_ThrowsException()
        {
            var callback = new PasswordResetCallback(
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<LinkGenerator>(),
                new Framework.Settings.IssuerSettings());

            Assert.Throws<ArgumentNullException>(() => callback.GetPasswordResetCallback(null));
        }

        [Test]
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

        [Test]
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

            internal PasswordResetCallbackContext(string url)
            {
                var mockContext = new Mock<HttpContext>();
                mockContext.Setup(c => c.Request).Returns(Mock.Of<HttpRequest>());
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
                new Framework.Settings.IssuerSettings { IssuerUrl = new Uri("http://www.google.com") });

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
