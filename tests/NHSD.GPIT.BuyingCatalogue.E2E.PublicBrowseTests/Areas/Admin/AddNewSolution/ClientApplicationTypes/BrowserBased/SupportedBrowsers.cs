using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ClientApplicationTypes.BrowserBased
{
    public sealed class SupportedBrowsers : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public SupportedBrowsers(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(CatalogueSolutionsController),
                  nameof(CatalogueSolutionsController.SupportedBrowsers),
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
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.BrowserBased)).Should().BeTrue();

            await using var context = GetEndToEndDbContext();
            var solution = await context.Solutions.SingleAsync(s => s.CatalogueItemId == SolutionId);

            var clientApplication = JsonSerializer.Deserialize<ServiceContracts.Solutions.ClientApplication>(solution.ClientApplication);

            clientApplication.Should().NotBeNull();
            clientApplication.BrowsersSupported.Should().Contain(expectedBrowser);
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
        }

        public void Dispose()
        {
            ClearClientApplication(SolutionId);
        }
    }
}
