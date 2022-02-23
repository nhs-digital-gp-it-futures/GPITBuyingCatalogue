using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ActionFilters
{
    public sealed class CookieConsentActionFilterTests
    {
        private readonly Dictionary<object, object> contextItems;
        private readonly Mock<HttpRequest> httpRequestMock;
        private readonly Mock<HttpContext> httpContextMock;
        private readonly ActionExecutingContext actionExecutingContext;
        private readonly ActionExecutedContext actionExecutedContext;

        public CookieConsentActionFilterTests()
        {
            contextItems = new Dictionary<object, object>();
            httpRequestMock = new Mock<HttpRequest>();
            httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.Request).Returns(httpRequestMock.Object);
            httpContextMock.Setup(c => c.Items).Returns(contextItems);

            var actionContext = new ActionContext(
                httpContextMock.Object,
                Mock.Of<Microsoft.AspNetCore.Routing.RouteData>(),
                Mock.Of<ActionDescriptor>(),
                new ModelStateDictionary());

            actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>())
            {
                Result = new OkResult(),
            };

            actionExecutedContext = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CookieConsentActionFilter).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static Task Invoke_NullContext_ThrowsException(
            CookieConsentActionFilter actionFilter)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => actionFilter.OnActionExecutionAsync(null, null));
        }

        [Theory]
        [CommonAutoData]
        public async Task Invoke_NullRequest_SetsExpectedItems(
            CookieConsentActionFilter actionFilter)
        {
            var expectedItems = new Dictionary<object, object>(2)
            {
                { CatalogueCookies.ShowCookieBanner, false },
                { CatalogueCookies.UseAnalytics, false },
            };

            httpContextMock.Setup(c => c.Request).Returns((HttpRequest)null);

            await actionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            contextItems.Should().BeEquivalentTo(expectedItems);
            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Theory]
        [CommonAutoData]
        public async Task Invoke_NoCookie_SetsExpectedItems(
            CookieConsentActionFilter actionFilter)
        {
            var expectedItems = new Dictionary<object, object>(1)
            {
                { CatalogueCookies.ShowCookieBanner, true },
            };

            var cookieCollectionMock = new Mock<IRequestCookieCollection>();
            httpRequestMock.Setup(r => r.Cookies).Returns(cookieCollectionMock.Object);

            await actionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            contextItems.Should().BeEquivalentTo(expectedItems);
            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Theory]
        [CommonAutoData]
        public async Task Invoke_NoCookieContent_SetsExpectedItems(
            CookieConsentActionFilter actionFilter)
        {
            var expectedItems = new Dictionary<object, object>(1)
            {
                { CatalogueCookies.ShowCookieBanner, true },
            };

            var cookieCollectionMock = new Mock<IRequestCookieCollection>();
            var outString = string.Empty;
            cookieCollectionMock.Setup(c => c.TryGetValue(It.IsAny<string>(), out outString)).Returns(true);

            httpRequestMock.Setup(r => r.Cookies).Returns(cookieCollectionMock.Object);

            await actionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            contextItems.Should().BeEquivalentTo(expectedItems);
            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Theory]
        [CommonAutoData]
        public async Task Invoke_ValidCookieContent_NoAnalytics_SetsExpectedItems(
            CookieData cookieData,
            CookieConsentActionFilter actionFilter)
        {
            var expectedItems = new Dictionary<object, object>(1)
            {
                { CatalogueCookies.ShowCookieBanner, true },
            };

            cookieData.Analytics = null;

            var cookieCollectionMock = new Mock<IRequestCookieCollection>();
            var outString = JsonSerializer.Serialize(cookieData, (JsonSerializerOptions)null);
            cookieCollectionMock.Setup(c => c.TryGetValue(It.IsAny<string>(), out outString)).Returns(true);
            httpRequestMock.Setup(r => r.Cookies).Returns(cookieCollectionMock.Object);

            await actionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            contextItems.Should().BeEquivalentTo(expectedItems);
            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Theory]
        [CommonAutoData]
        public async Task Invoke_ValidCookieContent_NoPolicyDate_SetsExpectedItems(
            CookieData cookieData,
            CookieConsentActionFilter actionFilter)
        {
            var expectedItems = new Dictionary<object, object>(2)
            {
                { CatalogueCookies.ShowCookieBanner, false },
                { CatalogueCookies.UseAnalytics, cookieData.Analytics },
            };

            var cookieCollectionMock = new Mock<IRequestCookieCollection>();
            var outString = JsonSerializer.Serialize(cookieData, (JsonSerializerOptions)null);
            cookieCollectionMock.Setup(c => c.TryGetValue(It.IsAny<string>(), out outString)).Returns(true);
            httpRequestMock.Setup(r => r.Cookies).Returns(cookieCollectionMock.Object);

            await actionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            contextItems.Should().BeEquivalentTo(expectedItems);
            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Theory]
        [CommonAutoData]
        public async Task Invoke_ValidCookieContent_PolicyDateInFuture_SetsExpectedItems(
            CookieData cookieData,
            [Frozen] CookieExpirationSettings cookieExpirationSettings,
            CookieConsentActionFilter actionFilter)
        {
            var expectedItems = new Dictionary<object, object>(2)
            {
                { CatalogueCookies.ShowCookieBanner, false },
                { CatalogueCookies.UseAnalytics, cookieData.Analytics },
            };

            cookieExpirationSettings.BuyingCatalogueCookiePolicyDate = DateTime.UtcNow.AddSeconds(5);

            var cookieCollectionMock = new Mock<IRequestCookieCollection>();
            var outString = JsonSerializer.Serialize(cookieData, (JsonSerializerOptions)null);
            cookieCollectionMock.Setup(c => c.TryGetValue(It.IsAny<string>(), out outString)).Returns(true);
            httpRequestMock.Setup(r => r.Cookies).Returns(cookieCollectionMock.Object);

            await actionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            contextItems.Should().BeEquivalentTo(expectedItems);
            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Theory]
        [CommonAutoData]
        public async Task Invoke_ValidCookieContent_CookieDateEarlierThanPolicy_SetsExpectedItems(
            CookieData cookieData,
            [Frozen] CookieExpirationSettings cookieExpirationSettings,
            CookieConsentActionFilter actionFilter)
        {
            var expectedItems = new Dictionary<object, object>(1)
            {
                { CatalogueCookies.ShowCookieBanner, true },
            };

            cookieData.CreationDate = DateTime.UtcNow.AddSeconds(-10).Ticks;
            cookieExpirationSettings.BuyingCatalogueCookiePolicyDate = DateTime.UtcNow.AddSeconds(-5);

            var cookieCollectionMock = new Mock<IRequestCookieCollection>();
            var outString = JsonSerializer.Serialize(cookieData, (JsonSerializerOptions)null);
            cookieCollectionMock.Setup(c => c.TryGetValue(It.IsAny<string>(), out outString)).Returns(true);

            httpRequestMock.Setup(r => r.Cookies).Returns(cookieCollectionMock.Object);

            await actionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            contextItems.Should().BeEquivalentTo(expectedItems);
            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }
    }
}
