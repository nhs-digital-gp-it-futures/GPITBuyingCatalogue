using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class ActionLinkTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ActionLinkTest(LocalWebApplicationFactory factory)

                 : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.ActionLink),
                        null)
        {
        }

        [Fact]
        public void ActionLink_VerifyThatActionLinkIsDisplayed()
        {
            CommonActions.IsElementDisplayed(ActionLinksObjects.ActionLink).Should().BeTrue();
        }
    }
}
