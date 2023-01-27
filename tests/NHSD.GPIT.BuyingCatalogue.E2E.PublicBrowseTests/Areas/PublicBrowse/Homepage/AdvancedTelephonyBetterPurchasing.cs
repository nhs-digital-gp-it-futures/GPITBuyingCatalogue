using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    public class AdvancedTelephonyBetterPurchasing : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AdvancedTelephonyBetterPurchasing(LocalWebApplicationFactory factory)
               : base(factory, typeof(HomeController), nameof(HomeController.AdvacedTelephonyBetterPurchaseFramework))
        {
        }

        [Fact]
        public void AdvacedTelephony_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().Be("Advanced Telephony Better Purchasing framework".FormatForComparison());
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.ElementIsDisplayed(AdvancedTelephonyObjects.HomepageButton).Should().BeTrue();
        }

        [Fact]
        public void AdvacedTelephony_ClickBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void AdvacedTelephony_ClickContactUsCrumb_Redirects()
        {
            CommonActions.ClickLinkElement(AdvancedTelephonyObjects.HomepageButton);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Index)).Should().BeTrue();
        }
    }
}
