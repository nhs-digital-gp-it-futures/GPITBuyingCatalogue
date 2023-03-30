using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Filtering
{
    [Collection(nameof(SharedContextCollection))]
    public class IncludeEpics : BuyerTestBase
    {
        private const string ValidCapabilityId = "1";
        private const string InvalidCapabilityId = "41";

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { "selectedCapabilityIds", ValidCapabilityId },
        };

        public IncludeEpics(LocalWebApplicationFactory factory)
            : base(factory, typeof(FilterController), nameof(FilterController.IncludeEpics), null, null, Parameters)
        {
        }

        [Fact]
        public void IncludeEpics_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(FilterObjects.HomeBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(FilterObjects.CatalogueSolutionsBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(FilterObjects.CapabilitiesBreadcrumbLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.SubmitButton).Should().BeTrue();
        }

        [Fact]
        public void IncludeEpics_NoEpicsAvailable_RedirectsCorrectly()
        {
            NavigateToUrl(
                typeof(FilterController),
                nameof(FilterController.IncludeEpics),
                null,
                new Dictionary<string, string>
                {
                    { "selectedCapabilityIds", InvalidCapabilityId },
                });

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SolutionsController),
                nameof(SolutionsController.Index)).Should().BeTrue();
        }

        [Fact]
        public void IncludeEpics_ClickHomeBreadcrumbLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(FilterObjects.HomeBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void IncludeEpics_ClickCatalogueSolutionsBreadcrumbLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(FilterObjects.CatalogueSolutionsBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SolutionsController),
                nameof(SolutionsController.Index)).Should().BeTrue();
        }

        [Fact]
        public void IncludeEpics_ClickCapabilitiesBreadcrumbLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(FilterObjects.CapabilitiesBreadcrumbLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FilterController),
                nameof(FilterController.FilterCapabilities)).Should().BeTrue();
        }

        [Fact]
        public void IncludeEpics_ClickSave_NoSelectionMade_ExpectedResult()
        {
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FilterController),
                nameof(FilterController.IncludeEpics)).Should().BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
        }

        [Fact]
        public void IncludeEpics_SelectFilterByEpics_ClickSave_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("Filter by Epics");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FilterController),
                nameof(FilterController.FilterEpics)).Should().BeTrue();
        }

        [Fact]
        public void IncludeEpics_SelectGoToResults_ClickSave_ExpectedResult()
        {
            CommonActions.ClickRadioButtonWithText("Go to results");
            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SolutionsController),
                nameof(SolutionsController.Index)).Should().BeTrue();
        }
    }
}
