using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.Import
{
    public class ImportGpPracticeListConfirmation : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ImportGpPracticeListConfirmation(LocalWebApplicationFactory factory)
            : base(factory, typeof(ImportController), nameof(ImportController.ImportGpPracticeListConfirmation), null)
        {
        }

        [Fact]
        public void ImportGpPracticeListConfirmation_AllElementsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonObjects.GoBackLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
        }

        [Fact]
        public void ImportGpPracticeListConfirmation_ClickGoBackLink_DisplaysCorrectPage()
        {
            CommonActions.ClickLinkElement(CommonObjects.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(OrganisationsController),
                    nameof(OrganisationsController.Index)).Should().BeTrue();
        }
    }
}
