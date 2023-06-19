using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using Xunit;
using Objects = NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ApplicationTypes.BrowserBased
{
    [Collection(nameof(AdminCollection))]
    public sealed class SupportedBrowsers : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public SupportedBrowsers(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(BrowserBasedController),
                  nameof(BrowserBasedController.SupportedBrowsers),
                  Parameters)
        {
        }

        [Fact]
        public async Task SupportedBrowsers_SelectBrowser()
        {
            var numBrowsers = CommonActions.GetNumberOfCheckBoxesDisplayed();

            var checkboxIndex = new Random().Next(numBrowsers);

            var expectedBrowser = CommonActions.ClickCheckbox(Objects.Common.CommonSelectors.CheckboxItem, checkboxIndex);

            CommonActions.ClickRadioButtonWithText("Yes");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.BrowserBased)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId);

            var clientApplication = solution.ClientApplication;

            clientApplication.Should().NotBeNull();
            clientApplication.BrowsersSupported.Should().Contain(b => b.BrowserName == expectedBrowser);
            clientApplication.MobileResponsive.Should().BeTrue();
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void SupportedBrowsers_ErrorThrownMissingMandatory(bool browserSelected, bool responsiveSelected)
        {
            if (browserSelected)
            {
                CommonActions.ClickFirstCheckbox();
            }

            if (responsiveSelected)
            {
                CommonActions.ClickRadioButtonWithText("Yes");
            }

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
        }

        [Fact]
        public async Task SupportedBrowsers_AddMinimumVersion_ExpectedResult()
        {
            var targetBrowser = "Google Chrome";

            CommonActions.ClickCheckboxByLabel(targetBrowser);

            var browserVersion = TextGenerators.TextInputAddText(By.Id("Browsers_0__MinimumBrowserVersion"), 50);

            CommonActions.ClickRadioButtonWithText("Yes");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.BrowserBased)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.FirstAsync(s => s.CatalogueItemId == SolutionId);

            var clientApplication = solution.ClientApplication;

            clientApplication.Should().NotBeNull();
            clientApplication.BrowsersSupported.First(bs => bs.BrowserName == targetBrowser).MinimumBrowserVersion.Should().Be(browserVersion);
        }

        public void Dispose()
        {
            ClearApplicationType(SolutionId);
        }
    }
}
