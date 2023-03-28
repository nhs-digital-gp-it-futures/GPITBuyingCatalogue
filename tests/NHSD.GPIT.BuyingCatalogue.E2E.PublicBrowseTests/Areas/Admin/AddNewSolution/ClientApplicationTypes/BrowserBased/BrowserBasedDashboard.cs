using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.EditSolution;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ClientApplicationTypes.BrowserBased
{
    [Collection(nameof(AdminCollection))]
    public sealed class BrowserBasedDashboard : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public BrowserBasedDashboard(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(BrowserBasedController),
                  nameof(BrowserBasedController.BrowserBased),
                  Parameters)
        {
        }

        [Theory]
        [InlineData("Supported browsers")]
        [InlineData("Plug-ins or extensions required")]
        [InlineData("Connectivity and resolution")]
        [InlineData("Hardware requirements")]
        [InlineData("Additional information")]
        public void BrowserBasedDashboard_RowDisplayed(string rowTitle)
        {
            CommonActions.GetTableRowCells(0).Should().ContainEquivalentOf(rowTitle);
        }

        [Fact]
        public void BrowserBasedDashboard_ClickGoBackLink()
        {
            CommonActions.ClickGoBackLink();

            CommonActions
                .PageLoadedCorrectGetIndex(typeof(CatalogueSolutionsController), nameof(CatalogueSolutionsController.AddApplicationType))
                .Should().BeTrue();
        }

        [Fact]
        public void BrowserBasedDashboard_ClickDeleteLink_NavigatesToDeleteConfirmation()
        {
            CommonActions.ClickLinkElement(ClientApplicationObjects.DeleteClientApplicationLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeleteApplicationTypeController),
                nameof(DeleteApplicationTypeController.DeleteApplicationTypeConfirmation))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void BrowserBasedDashboard_HasApplication_ClickGoBackLink()
        {
            var solutionWithBrowserBasedApp = new CatalogueItemId(99999, "001");
            var parameters = new Dictionary<string, string>
            {
                { nameof(SolutionId), solutionWithBrowserBasedApp.ToString() },
            };

            NavigateToUrl(
                typeof(BrowserBasedController),
                nameof(BrowserBasedController.BrowserBased),
                parameters);

            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.ClientApplicationType))
                .Should().BeTrue();
        }

        public void Dispose()
        {
            ClearClientApplication(SolutionId);
        }
    }
}
