using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Middleware.CookieConsent;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.SharedMocks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Middleware.CookieConsent
{
    public static class CookieConsentMiddlewareTests
    {
        [Theory]
        [CommonAutoData]
        public static Task Invoke_NullHttpContext_ThrowsException(
            CookieExpirationSettings cookieExpirationSettings,
            CookieConsentMiddleware middleware)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => middleware.Invoke(null, cookieExpirationSettings));
        }

        [Theory]
        [CommonAutoData]
        public static Task Invoke_NullCookieExpirationSettings_ThrowsException(
            CookieConsentMiddleware middleware)
        {
            var httpContextMock = new MockHttpContext();

            return Assert.ThrowsAsync<ArgumentNullException>(() => middleware.Invoke(httpContextMock.HttpContext, null));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Invoke_NullRequest_SetsExpectedItems(
            CookieExpirationSettings cookieExpirationSettings,
            CookieConsentMiddleware middleware)
        {
            var expectedItems = new Dictionary<object, object>(2)
            {
                { CatalogueCookies.ShowCookieBanner, false },
                { CatalogueCookies.UseAnalytics, false },
            };

            var httpContextMock = new MockHttpContext(false, false);

            await middleware.Invoke(httpContextMock.HttpContext, cookieExpirationSettings);

            httpContextMock.ActualItems.Should().BeEquivalentTo(expectedItems);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Invoke_NoCookie_SetsExpectedItems(
            CookieExpirationSettings cookieExpirationSettings,
            CookieConsentMiddleware middleware)
        {
            var expectedItems = new Dictionary<object, object>(1)
            {
                { CatalogueCookies.ShowCookieBanner, true },
            };

            var httpContextMock = new MockHttpContext(true, false);

            await middleware.Invoke(httpContextMock.HttpContext, cookieExpirationSettings);

            httpContextMock.ActualItems.Should().BeEquivalentTo(expectedItems);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Invoke_NoCookieContent_SetsExpectedItems(
            MockHttpContext httpContextMock,
            CookieExpirationSettings cookieExpirationSettings,
            CookieConsentMiddleware middleware)
        {
            var expectedItems = new Dictionary<object, object>(1)
            {
                { CatalogueCookies.ShowCookieBanner, true },
            };

            await middleware.Invoke(httpContextMock.HttpContext, cookieExpirationSettings);

            httpContextMock.ActualItems.Should().BeEquivalentTo(expectedItems);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Invoke_BadCookieContent_SetsExpectedItems(
            MockHttpContext httpContextMock,
            CookieExpirationSettings cookieExpirationSettings,
            CookieConsentMiddleware middleware)
        {
            var expectedItems = new Dictionary<object, object>(1)
            {
                { CatalogueCookies.ShowCookieBanner, true },
            };

            httpContextMock.CookieContent = string.Empty;

            await middleware.Invoke(httpContextMock.HttpContext, cookieExpirationSettings);

            httpContextMock.ActualItems.Should().BeEquivalentTo(expectedItems);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Invoke_ValidCookieContent_NoAnalytics_SetsExpectedItems(
            MockHttpContext httpContextMock,
            CookieData cookieData,
            CookieExpirationSettings cookieExpirationSettings,
            CookieConsentMiddleware middleware)
        {
            var expectedItems = new Dictionary<object, object>(1)
            {
                { CatalogueCookies.ShowCookieBanner, true },
            };

            cookieData.Analytics = null;
            httpContextMock.SetCookieData(cookieData);

            await middleware.Invoke(httpContextMock.HttpContext, cookieExpirationSettings);

            httpContextMock.ActualItems.Should().BeEquivalentTo(expectedItems);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Invoke_ValidCookieContent_NoPolicyDate_SetsExpectedItems(
            MockHttpContext httpContextMock,
            CookieData cookieData,
            CookieExpirationSettings cookieExpirationSettings,
            CookieConsentMiddleware middleware)
        {
            var expectedItems = new Dictionary<object, object>(2)
            {
                { CatalogueCookies.ShowCookieBanner, false },
                { CatalogueCookies.UseAnalytics, cookieData.Analytics },
            };

            httpContextMock.SetCookieData(cookieData);
            cookieExpirationSettings.BuyingCatalogueCookiePolicyDate = null;

            await middleware.Invoke(httpContextMock.HttpContext, cookieExpirationSettings);

            httpContextMock.ActualItems.Should().BeEquivalentTo(expectedItems);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Invoke_ValidCookieContent_PolicyDateInFuture_SetsExpectedItems(
            MockHttpContext httpContextMock,
            CookieData cookieData,
            CookieExpirationSettings cookieExpirationSettings,
            CookieConsentMiddleware middleware)
        {
            var expectedItems = new Dictionary<object, object>(2)
            {
                { CatalogueCookies.ShowCookieBanner, false },
                { CatalogueCookies.UseAnalytics, cookieData.Analytics },
            };

            httpContextMock.SetCookieData(cookieData);
            cookieExpirationSettings.BuyingCatalogueCookiePolicyDate = DateTime.UtcNow.AddSeconds(5);

            await middleware.Invoke(httpContextMock.HttpContext, cookieExpirationSettings);

            httpContextMock.ActualItems.Should().BeEquivalentTo(expectedItems);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Invoke_ValidCookieContent_CookieDateEarlierThanPolicy_SetsExpectedItems(
            MockHttpContext httpContextMock,
            CookieData cookieData,
            CookieExpirationSettings cookieExpirationSettings,
            CookieConsentMiddleware middleware)
        {
            var expectedItems = new Dictionary<object, object>(1)
            {
                { CatalogueCookies.ShowCookieBanner, true },
            };

            cookieData.CreationDate = DateTime.UtcNow.AddSeconds(-10).Ticks;
            cookieExpirationSettings.BuyingCatalogueCookiePolicyDate = DateTime.UtcNow.AddSeconds(-5);
            httpContextMock.SetCookieData(cookieData);

            await middleware.Invoke(httpContextMock.HttpContext, cookieExpirationSettings);

            httpContextMock.ActualItems.Should().BeEquivalentTo(expectedItems);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Invoke_InvokesNext(
            Mock<RequestDelegate> requestDelegateMock,
            MockHttpContext httpContextMock,
            CookieExpirationSettings cookieExpirationSettings)
        {
            var middleware = new CookieConsentMiddleware(requestDelegateMock.Object);

            await middleware.Invoke(httpContextMock.HttpContext, cookieExpirationSettings);

            requestDelegateMock.Verify(r => r.Invoke(httpContextMock.HttpContext));
        }
    }
}
