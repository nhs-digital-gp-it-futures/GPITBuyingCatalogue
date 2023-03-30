using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.EditSolution;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.AddNewSolution.ClientApplicationTypes.MobileOrTablet
{
    [Collection(nameof(AdminCollection))]
    public sealed class MobileOrTabletDashboard : AuthorityTestBase, IDisposable
    {
        private static readonly CatalogueItemId SolutionId = new(99999, "002");

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(SolutionId), SolutionId.ToString() },
        };

        public MobileOrTabletDashboard(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(MobileTabletBasedController),
                  nameof(MobileTabletBasedController.MobileTablet),
                  Parameters)
        {
        }

        [Theory]
        [InlineData("Supported operating systems")]
        [InlineData("Connectivity")]
        [InlineData("Memory and storage")]
        [InlineData("Third-party components and device capabilities")]
        [InlineData("Hardware requirements")]
        [InlineData("Additional information")]
        public void MobileOrTabletDashboard_RowDisplayed(string rowTitle)
        {
            CommonActions.GetTableRowCells().Should().ContainEquivalentOf(rowTitle);
        }

        [Fact]
        public void MobileOrTabletDashboard_ClickGoBackLink()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(typeof(CatalogueSolutionsController), nameof(CatalogueSolutionsController.ClientApplicationType));
        }

        [Fact]
        public void MobileOrTabletDashboard_ClickDeleteLink_NavigatesToDeleteConfirmation()
        {
            CommonActions.ClickLinkElement(ClientApplicationObjects.DeleteClientApplicationLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeleteApplicationTypeController),
                nameof(DeleteApplicationTypeController.DeleteApplicationTypeConfirmation))
                .Should()
                .BeTrue();
        }

        public void Dispose()
        {
            ClearClientApplication(SolutionId);
        }
    }
}
