using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    [Collection(nameof(SharedContextCollection))]
    public class AccessibilityStatement : AnonymousTestBase
    {
        public AccessibilityStatement(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(factory, typeof(HomeController), nameof(HomeController.AccessibilityStatement), null, testOutputHelper)
        {
        }

        [Fact]
        public void AccessibilityStatement_AllSectionsDisplayed()
        {
            RunTest(() =>
            {
                CommonActions.ElementIsDisplayed(CommonSelectors.GoBackLink).Should().BeTrue();
                CommonActions.ElementIsDisplayed(AccessibilityStatementObjects.ContactUsLink).Should().BeTrue();
            });
        }

        [Fact]
        public void AccessibilityStatement_ClickGoBackLink_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(CommonSelectors.GoBackLink);

                CommonActions.PageLoadedCorrectGetIndex(
                        typeof(HomeController),
                        nameof(HomeController.Index)).Should().BeTrue();
            });
        }

        [Fact]
        public void AccessibilityStatement_ClickContactUsLink_ExpectedResult()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(AccessibilityStatementObjects.ContactUsLink);

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.ContactUs)).Should().BeTrue();
            });
        }
    }
}
