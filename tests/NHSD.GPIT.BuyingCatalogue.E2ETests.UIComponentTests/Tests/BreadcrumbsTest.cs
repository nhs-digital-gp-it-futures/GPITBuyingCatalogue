using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class BreadcrumbsTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public BreadcrumbsTest(LocalWebApplicationFactory factory)
             : base(factory,
                    typeof(HomeController),
                    nameof(HomeController.Breadcrumbs),
                    null)
        {
        }

        [Fact]
        public void Breadcrumbs_VerifyBreadcrumbsPageIsLoaded()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                 typeof(HomeController),
                 nameof(HomeController.Breadcrumbs))
                 .Should()
                 .BeTrue();
        }

        [Fact]
        public void Breadcrumbs_TitleIsDisplayed()
        {
            CommonActions.GetElementText(CommonObject.Header1).
             Contains("Breadcrumbs")
             .Should().BeTrue();
        }

        [Fact]
        public void Breadcrumbs_ActionLinksIsDisplayed()
        {
            CommonActions.GetElementText(BreadcrumbsObjects.BreadcrumbsActionLink).
             Contains("Action Links")
             .Should().BeTrue();
        }

        [Fact]
        public void Breadcrumbs_AddressLinksIsDisplayed()
        {
            CommonActions.GetElementText(BreadcrumbsObjects.BreadcrumbsAddressLink).
             Contains("Address")
             .Should().BeTrue();
        }
    }
}
