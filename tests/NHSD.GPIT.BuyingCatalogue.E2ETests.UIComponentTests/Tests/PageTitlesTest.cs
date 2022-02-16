using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects
{
    public class PageTitlesTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public PageTitlesTest(LocalWebApplicationFactory factory)
            : base(factory,
                    typeof(HomeController),
                    nameof(HomeController.PageTitle),
                    null)

        {
        }

        [Fact]
        public void PageTitle_VerifyThatPageTitleIsLoaded()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                 typeof(HomeController),
                 nameof(HomeController.PageTitle))
                 .Should()
                 .BeTrue();
        }

        [Fact]
        public void PageTitle_CodeExampleHeaderIsDisplayed()
        {
            CommonActions.IsElementDisplayed(PageTitlesObject.CodeExampleForPageTitle)
               .Should().BeTrue();
        }
    }
}
