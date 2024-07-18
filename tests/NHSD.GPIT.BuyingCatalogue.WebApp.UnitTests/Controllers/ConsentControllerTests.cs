using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NSubstitute.Core;
using Xunit;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    public static class ConsentControllerTests
    {
        [Theory]
        [MockInlineAutoData(false)]
        [MockInlineAutoData(true)]
        public static void AcceptCookies_CreatesExpectedCookieData(
            bool agreeToAnalytics,
            [Frozen] IResponseCookies responseCookiesMock,
            ConsentController controller)
        {
            var appendedCookieData = new AppendedCookieData(responseCookiesMock);

            controller.AcceptCookies(agreeToAnalytics);

            appendedCookieData.CookieName.Should().Be(CatalogueCookies.BuyingCatalogueConsent);
        }

        [Theory]
        [MockInlineAutoData(false)]
        [MockInlineAutoData(true)]
        public static void AcceptCookies_SetsExpectedCookieData(
            bool agreeToAnalytics,
            [Frozen] IResponseCookies responseCookiesMock,
            ConsentController controller)
        {
            var appendedCookieData = new AppendedCookieData(responseCookiesMock);

            controller.AcceptCookies(agreeToAnalytics);

            appendedCookieData.UseAnalytics.Should().Be(agreeToAnalytics);
            appendedCookieData.CookieCreationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Theory]
        [MockInlineAutoData(false)]
        [MockInlineAutoData(true)]
        public static void AcceptCookies_SetsExpectedCookieOptions(
            bool agreeToAnalytics,
            [Frozen] CookieExpirationSettings cookieExpirationSettings,
            [Frozen] IResponseCookies responseCookiesMock,
            ConsentController controller)
        {
            var appendedCookieData = new AppendedCookieData(responseCookiesMock);
            var expectedCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
            };

            controller.AcceptCookies(agreeToAnalytics);

            appendedCookieData.CookieOptions.Should().BeEquivalentTo(expectedCookieOptions, c => c.Excluding(o => o.Expires));
            appendedCookieData.CookieExpires.Should().BeCloseTo(
                DateTime.UtcNow.Add(cookieExpirationSettings.ConsentExpiration),
                TimeSpan.FromSeconds(5));
        }

        [Theory]
        [MockInlineAutoData(false)]
        [MockInlineAutoData(true)]
        public static void AcceptCookies_NoReferrer_ReturnsExpectedResult(
            bool agreeToAnalytics,
            ConsentController controller)
        {
            var result = controller.AcceptCookies(agreeToAnalytics);

            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectResult>();
            result.As<RedirectResult>().Url.Should().Be("~/");
        }

        [Theory]
        [MockInlineAutoData(false)]
        [MockInlineAutoData(true)]
        public static void AcceptCookies_WithReferrer_ReturnsExpectedResult(
            bool agreeToAnalytics,
            [Frozen] HeaderDictionary headerDictionary,
            ConsentController controller)
        {
            const string url = "https://buyingcatalogue.digital.nhs.uk/";

            headerDictionary.IsReadOnly = false;
            headerDictionary[HeaderNames.Referer] = url;

            var result = controller.AcceptCookies(agreeToAnalytics);

            result.Should().NotBeNull();
            result.Should().BeOfType<RedirectResult>();
            result.As<RedirectResult>().Url.Should().Be(url);
        }

        [Theory]
        [MockAutoData]
        public static void Get_CookieSettings_ExpectedResult(
            ConsentController controller)
        {
            var result = controller.CookieSettings();

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.As<CookieSettingsModel>().Should().NotBeNull();
        }

        [Theory]
        [MockAutoData]
        public static void Get_CookieSettings_UseAnalyticsAsExpected(
            CookieData cookieData,
            [Frozen] IRequestCookieCollection requestCookiesMock,
            ConsentController controller)
        {
            var cookieAsString = JsonSerializer.Serialize(cookieData);

            requestCookiesMock[CatalogueCookies.BuyingCatalogueConsent].Returns(cookieAsString);

            var result = controller.CookieSettings();

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.As<CookieSettingsModel>().Should().NotBeNull();
            result.As<ViewResult>().Model.As<CookieSettingsModel>().UseAnalytics.Should().Be(cookieData.Analytics);
        }

        [Theory]
        [MockAutoData]
        public static void Post_CookieSettings_InvalidModel(
            CookieSettingsModel model,
            ConsentController controller)
        {
            controller.ModelState.AddModelError("some-key", "Some-error");

            var result = controller.CookieSettings(model);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockInlineAutoData(false)]
        [MockInlineAutoData(true)]
        public static void Post_CookieSettings_SetsExpectedCookieData(
            bool agreeToAnalytics,
            [Frozen] IResponseCookies responseCookiesMock,
            ConsentController controller)
        {
            var appendedCookieData = new AppendedCookieData(responseCookiesMock);

            var model = new CookieSettingsModel
            {
                UseAnalytics = agreeToAnalytics,
            };

            _ = controller.CookieSettings(model);

            appendedCookieData.UseAnalytics.Should().Be(agreeToAnalytics);
            appendedCookieData.CookieCreationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Theory]
        [MockInlineAutoData(false)]
        [MockInlineAutoData(true)]
        public static void Post_CookieSettings_ClearsCookies(
            bool agreeToAnalytics,
            [Frozen] IRequestCookieCollection requestCookiesMock,
            [Frozen] IResponseCookies responseCookiesMock,
            ConsentController controller)
        {
            var cookieList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("some-cookie", "cookie-value"),
            };

            requestCookiesMock.GetEnumerator().Returns(cookieList.GetEnumerator());

            var model = new CookieSettingsModel
            {
                UseAnalytics = agreeToAnalytics,
            };

            _ = controller.CookieSettings(model);

            responseCookiesMock.Received().Delete(Arg.Any<string>());
        }

        [Theory]
        [MockInlineAutoData(false)]
        [MockInlineAutoData(true)]
        public static void Post_CookieSettings_DoesNotClearMandatoryCookies(
            bool agreeToAnalytics,
            [Frozen] IRequestCookieCollection requestCookiesMock,
            [Frozen] IResponseCookies responseCookiesMock,
            ConsentController controller)
        {
            var cookieList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("some-cookie", "cookie-value"),
                new KeyValuePair<string, string>("user-session", "user-session-value"),
            };

            requestCookiesMock.GetEnumerator().Returns(cookieList.GetEnumerator());

            var model = new CookieSettingsModel
            {
                UseAnalytics = agreeToAnalytics,
            };

            _ = controller.CookieSettings(model);

            responseCookiesMock.Received().Delete("some-cookie");
            responseCookiesMock.DidNotReceive().Delete("user-session");
        }

        private sealed class AppendedCookieData
        {
            private CookieData actualCookieData;

            internal AppendedCookieData(IResponseCookies responseCookiesMock)
            {
                responseCookiesMock
                    .When(rc => rc.Append(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CookieOptions>()))
                    .Do(CookieCallback);
            }

            internal DateTime CookieCreationDate => new(actualCookieData?.CreationDate.GetValueOrDefault() ?? 0);

            internal DateTimeOffset? CookieExpires => CookieOptions?.Expires;

            internal string CookieName { get; private set; }

            internal CookieOptions CookieOptions { get; private set; }

            internal bool? UseAnalytics => actualCookieData?.Analytics;

            private void CookieCallback(CallInfo callInfo)
            {
                CookieName = callInfo.ArgAt<string>(0);
                actualCookieData = JsonSerializer.Deserialize<CookieData>(callInfo.ArgAt<string>(1));
                CookieOptions = callInfo.ArgAt<CookieOptions>(2);
            }
        }
    }
}
