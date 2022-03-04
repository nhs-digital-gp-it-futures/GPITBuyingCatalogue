using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    public class AccessibilityStatement : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AccessibilityStatement(LocalWebApplicationFactory factory)
            : base(factory, typeof(HomeController), nameof(HomeController.AccessibilityStatement), null)
        {
        }

        [Fact]
        public void AccessibilityStatement_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.GoBackLink).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AccessibilityStatementObjects.ContactUsLink).Should().BeTrue();
        }

        [Fact]
        public void AccessibilityStatement_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(CommonSelectors.GoBackLink);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.Index)).Should().BeTrue();
        }

        [Fact]
        public void AccessibilityStatement_ClickContactUsLink_ExpectedResult()
        {
            CommonActions.ClickLinkElement(AccessibilityStatementObjects.ContactUsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.ContactUs)).Should().BeTrue();
        }
    }
}
