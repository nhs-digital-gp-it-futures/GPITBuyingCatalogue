using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Middleware.CookieConsent;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Controllers
{
    public static class ConsentControllerTests
    {
        [Theory]
        [CommonInlineAutoData(false)]
        [CommonInlineAutoData(true)]
        public static void AcceptCookies_CreatesExpectedCookieData(
            bool agreeToAnalytics,
            [Frozen] Mock<IResponseCookies> responseCookiesMock,
            ConsentController controller)
        {
            var appendedCookieData = new AppendedCookieData(responseCookiesMock);

            controller.AcceptCookies(agreeToAnalytics);

            appendedCookieData.CookieName.Should().Be(CatalogueCookies.BuyingCatalogueConsent);
        }

        [Theory]
        [CommonInlineAutoData(false)]
        [CommonInlineAutoData(true)]
        public static void AcceptCookies_SetsExpectedCookieData(
            bool agreeToAnalytics,
            [Frozen] Mock<IResponseCookies> responseCookiesMock,
            ConsentController controller)
        {
            var appendedCookieData = new AppendedCookieData(responseCookiesMock);

            controller.AcceptCookies(agreeToAnalytics);

            appendedCookieData.UseAnalytics.Should().Be(agreeToAnalytics);
            appendedCookieData.CookieCreationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Theory]
        [CommonInlineAutoData(false)]
        [CommonInlineAutoData(true)]
        public static void AcceptCookies_SetsExpectedCookieOptions(
            bool agreeToAnalytics,
            [Frozen] CookieExpirationSettings cookieExpirationSettings,
            [Frozen] Mock<IResponseCookies> responseCookiesMock,
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
        [CommonInlineAutoData(false)]
        [CommonInlineAutoData(true)]
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
        [CommonInlineAutoData(false)]
        [CommonInlineAutoData(true)]
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
        [CommonAutoData]
        public static void Get_CookieSettings_ExpectedResult(
            ConsentController controller)
        {
            var result = controller.CookieSettings();

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.As<CookieSettingsModel>().Should().NotBeNull();
        }

        [Theory]
        [CommonAutoData]
        public static void Get_CookieSettings_UseAnalyticsAsExpected(
            CookieData cookieData,
            [Frozen] Mock<IRequestCookieCollection> requestCookiesMock,
            ConsentController controller)
        {
            var cookieAsString = JsonSerializer.Serialize(cookieData);

            requestCookiesMock.Setup(c => c[CatalogueCookies.BuyingCatalogueConsent]).Returns(cookieAsString);

            var result = controller.CookieSettings();

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.As<CookieSettingsModel>().Should().NotBeNull();
            result.As<ViewResult>().Model.As<CookieSettingsModel>().UseAnalytics.Should().Be(cookieData.Analytics);
        }

        [Theory]
        [CommonAutoData]
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
        [CommonInlineAutoData(false)]
        [CommonInlineAutoData(true)]
        public static void Post_CookieSettings_SetsExpectedCookieData(
            bool agreeToAnalytics,
            [Frozen] Mock<IResponseCookies> responseCookiesMock,
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
        [CommonInlineAutoData(false)]
        [CommonInlineAutoData(true)]
        public static void Post_CookieSettings_ClearsCookies(
            bool agreeToAnalytics,
            [Frozen] Mock<IRequestCookieCollection> requestCookiesMock,
            [Frozen] Mock<IResponseCookies> responseCookiesMock,
            ConsentController controller)
        {
            var cookieList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("some-cookie", "cookie-value"),
            };

            requestCookiesMock.Setup(c => c.GetEnumerator()).Returns(cookieList.GetEnumerator());

            var model = new CookieSettingsModel
            {
                UseAnalytics = agreeToAnalytics,
            };

            _ = controller.CookieSettings(model);

            responseCookiesMock.Verify(c => c.Delete(It.IsAny<string>()));
        }

        [Theory]
        [CommonInlineAutoData(false)]
        [CommonInlineAutoData(true)]
        public static void Post_CookieSettings_DoesNotClearMandatoryCookies(
            bool agreeToAnalytics,
            [Frozen] Mock<IRequestCookieCollection> requestCookiesMock,
            [Frozen] Mock<IResponseCookies> responseCookiesMock,
            ConsentController controller)
        {
            var cookieList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("some-cookie", "cookie-value"),
                new KeyValuePair<string, string>("user-session", "user-session-value"),
            };

            requestCookiesMock.Setup(c => c.GetEnumerator()).Returns(cookieList.GetEnumerator());

            var model = new CookieSettingsModel
            {
                UseAnalytics = agreeToAnalytics,
            };

            _ = controller.CookieSettings(model);

            responseCookiesMock.Verify(c => c.Delete("some-cookie"));
            responseCookiesMock.Verify(c => c.Delete("user-session"), Times.Never());
        }

        private sealed class AppendedCookieData
        {
            private CookieData actualCookieData;

            internal AppendedCookieData(Mock<IResponseCookies> responseCookiesMock)
            {
                responseCookiesMock
                    .Setup(rc => rc.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()))
                    .Callback<string, string, CookieOptions>(CookieCallback);
            }

            internal DateTime CookieCreationDate => new(actualCookieData?.CreationDate.GetValueOrDefault() ?? 0);

            internal DateTimeOffset? CookieExpires => CookieOptions?.Expires;

            internal string CookieName { get; private set; }

            internal CookieOptions CookieOptions { get; private set; }

            internal bool? UseAnalytics => actualCookieData?.Analytics;

            private void CookieCallback(string key, string value, CookieOptions cookieOptions)
            {
                CookieName = key;
                actualCookieData = JsonSerializer.Deserialize<CookieData>(value);
                CookieOptions = cookieOptions;
            }
        }
    }
}
