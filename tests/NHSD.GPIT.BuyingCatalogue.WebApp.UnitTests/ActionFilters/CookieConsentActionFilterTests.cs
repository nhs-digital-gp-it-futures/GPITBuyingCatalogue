using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.ActionFilters
{
    public sealed class CookieConsentActionFilterTests
    {
        private readonly Dictionary<object, object> contextItems;
        private readonly HttpRequest httpRequestMock;
        private readonly HttpContext httpContextMock;
        private readonly ActionExecutingContext actionExecutingContext;
        private readonly ActionExecutedContext actionExecutedContext;

        public CookieConsentActionFilterTests()
        {
            contextItems = new Dictionary<object, object>();
            httpRequestMock = Substitute.For<HttpRequest>();
            httpContextMock = Substitute.For<HttpContext>();
            httpContextMock.Request.Returns(httpRequestMock);
            httpContextMock.Items = contextItems;

            var actionContext = new ActionContext(
                httpContextMock,
                Substitute.For<Microsoft.AspNetCore.Routing.RouteData>(),
                Substitute.For<ActionDescriptor>(),
                new ModelStateDictionary());

            actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Substitute.For<Controller>())
            {
                Result = new OkResult(),
            };

            actionExecutedContext = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Substitute.For<Controller>());
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CookieConsentActionFilter).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static Task Invoke_NullContext_ThrowsException(
            CookieConsentActionFilter actionFilter)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => actionFilter.OnActionExecutionAsync(null, null));
        }

        [Theory]
        [MockAutoData]
        public async Task Invoke_NullRequest_SetsExpectedItems(
            CookieConsentActionFilter actionFilter)
        {
            var expectedItems = new Dictionary<object, object>(2)
            {
                { CatalogueCookies.ShowCookieBanner, false },
                { CatalogueCookies.UseAnalytics, false },
            };

            httpContextMock.Request.Returns((HttpRequest)null);

            await actionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            contextItems.Should().BeEquivalentTo(expectedItems);
            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Theory]
        [MockAutoData]
        public async Task Invoke_NoCookie_SetsExpectedItems(
            IRequestCookieCollection cookieCollection,
            CookieConsentActionFilter actionFilter)
        {
            var expectedItems = new Dictionary<object, object>(1)
            {
                { CatalogueCookies.ShowCookieBanner, true },
            };

            httpRequestMock.Cookies.Returns(cookieCollection);

            await actionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            contextItems.Should().BeEquivalentTo(expectedItems);
            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Theory]
        [MockAutoData]
        public async Task Invoke_NoCookieContent_SetsExpectedItems(
            IRequestCookieCollection cookieCollection,
            CookieConsentActionFilter actionFilter)
        {
            var expectedItems = new Dictionary<object, object>(1)
            {
                { CatalogueCookies.ShowCookieBanner, true },
            };

            var outString = string.Empty;
            cookieCollection.TryGetValue(Arg.Any<string>(), out Arg.Any<string>()).Returns(x =>
            {
                x[1] = outString;
                return true;
            });

            httpRequestMock.Cookies.Returns(cookieCollection);

            await actionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            contextItems.Should().BeEquivalentTo(expectedItems);
            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Theory]
        [MockAutoData]
        public async Task Invoke_ValidCookieContent_NoAnalytics_SetsExpectedItems(
            CookieData cookieData,
            IRequestCookieCollection cookieCollection,
            CookieConsentActionFilter actionFilter)
        {
            var expectedItems = new Dictionary<object, object>(1)
            {
                { CatalogueCookies.ShowCookieBanner, true },
            };

            cookieData.Analytics = null;

            var outString = JsonSerializer.Serialize(cookieData, (JsonSerializerOptions)null);
            cookieCollection.TryGetValue(Arg.Any<string>(), out Arg.Any<string>()).Returns(x =>
            {
                x[1] = outString;
                return true;
            });

            httpRequestMock.Cookies.Returns(cookieCollection);

            await actionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            contextItems.Should().BeEquivalentTo(expectedItems);
            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Theory]
        [MockAutoData]
        public async Task Invoke_ValidCookieContent_NoPolicyDate_SetsExpectedItems(
            CookieData cookieData,
            IRequestCookieCollection cookieCollection,
            [Frozen] CookieExpirationSettings cookieExpirationSettings,
            CookieConsentActionFilter actionFilter)
        {
            cookieExpirationSettings.BuyingCatalogueCookiePolicyDate = null;

            var expectedItems = new Dictionary<object, object>(2)
            {
                { CatalogueCookies.ShowCookieBanner, false },
                { CatalogueCookies.UseAnalytics, cookieData.Analytics },
            };

            var outString = JsonSerializer.Serialize(cookieData, (JsonSerializerOptions)null);
            cookieCollection.TryGetValue(Arg.Any<string>(), out Arg.Any<string>())
                .Returns(
                    x =>
                    {
                        x[1] = outString;
                        return true;
                    });

            httpRequestMock.Cookies.Returns(cookieCollection);

            await actionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            contextItems.Should().BeEquivalentTo(expectedItems);
            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Theory]
        [MockAutoData]
        public async Task Invoke_ValidCookieContent_PolicyDateInFuture_SetsExpectedItems(
            CookieData cookieData,
            IRequestCookieCollection cookieCollection,
            [Frozen] CookieExpirationSettings cookieExpirationSettings,
            CookieConsentActionFilter actionFilter)
        {
            var expectedItems = new Dictionary<object, object>(2)
            {
                { CatalogueCookies.ShowCookieBanner, false },
                { CatalogueCookies.UseAnalytics, cookieData.Analytics },
            };

            cookieExpirationSettings.BuyingCatalogueCookiePolicyDate = DateTime.UtcNow.AddSeconds(5);

            var outString = JsonSerializer.Serialize(cookieData, (JsonSerializerOptions)null);
            cookieCollection.TryGetValue(Arg.Any<string>(), out Arg.Any<string>())
                .Returns(
                    x =>
                    {
                        x[1] = outString;
                        return true;
                    });

            httpRequestMock.Cookies.Returns(cookieCollection);

            await actionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            contextItems.Should().BeEquivalentTo(expectedItems);
            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }

        [Theory]
        [MockAutoData]
        public async Task Invoke_ValidCookieContent_CookieDateEarlierThanPolicy_SetsExpectedItems(
            CookieData cookieData,
            IRequestCookieCollection cookieCollection,
            [Frozen] CookieExpirationSettings cookieExpirationSettings,
            CookieConsentActionFilter actionFilter)
        {
            var expectedItems = new Dictionary<object, object>(1)
            {
                { CatalogueCookies.ShowCookieBanner, true },
            };

            cookieData.CreationDate = DateTime.UtcNow.AddSeconds(-10).Ticks;
            cookieExpirationSettings.BuyingCatalogueCookiePolicyDate = DateTime.UtcNow.AddSeconds(-5);

            var outString = JsonSerializer.Serialize(cookieData, (JsonSerializerOptions)null);
            cookieCollection.TryGetValue(Arg.Any<string>(), out Arg.Any<string>())
                .Returns(
                    x =>
                    {
                        x[1] = outString;
                        return true;
                    });

            httpRequestMock.Cookies.Returns(cookieCollection);

            await actionFilter.OnActionExecutionAsync(actionExecutingContext, () => Task.FromResult(actionExecutedContext));

            contextItems.Should().BeEquivalentTo(expectedItems);
            actionExecutingContext.Result.Should().BeOfType<OkResult>();
        }
    }
}
