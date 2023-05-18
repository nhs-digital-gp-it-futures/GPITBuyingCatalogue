using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.ManageFilters
{
    [Collection(nameof(SharedContextCollection))]
    public class CannotSaveFilter : BuyerTestBase
    {
        private static readonly string BackLink = "/catalogue-solutions";

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(BackLink), BackLink },
        };

        public CannotSaveFilter(LocalWebApplicationFactory factory)
            : base(factory, typeof(ManageFiltersController), nameof(ManageFiltersController.CannotSaveFilter), null, null, Parameters)
        {
        }

        [Fact]
        public async Task CannotSaveFilter_AllSectionsDisplayed()
        {
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementTextEqualTo(CommonSelectors.Header1, "Filter cannot be saved");
            CommonActions.ElementIsDisplayed(ManageFilterObjects.ManageFilterLink).Should().BeTrue();
            CommonActions.CancelLinkDisplayed().Should().BeTrue();
        }

        [Fact]
        public void CannotSaveFilter_ClickBackLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonObjects.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SolutionsController),
                    nameof(SolutionsController.Index))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CannotSaveFilter_CancelLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonSelectors.CancelLink);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(SolutionsController),
                    nameof(SolutionsController.Index))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void CannotSaveFilter_ClickManageFilterLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(ManageFilterObjects.ManageFilterLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ManageFiltersController),
                nameof(ManageFiltersController.Index)).Should().BeTrue();
        }
    }
}
