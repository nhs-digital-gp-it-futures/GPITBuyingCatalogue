using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public class ButtonTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ButtonTest(LocalWebApplicationFactory factory)

            : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.Buttons),
                        null)
        {
        }

        [Fact]
        public void Buttons_SaveAndContinueButtonIsDisplayed()
        {
            CommonActions.GetElementText(ButtonsObjects.SaveAndContinueButton).Contains("Save and continue").Should().BeTrue();
        }

        [Fact]
        public void Buttons_SecondaryButtonIsDisplayed()
        {
            CommonActions.IsElementDisplayed(ButtonsObjects.SecondaryButton).Should().BeTrue();
        }

        [Fact]
        public void Buttons_GetButtonHeader()
        {
            CommonActions.GetElementText(CommonObject.Header1).
               Contains("Button")
               .Should().BeTrue();
        }
    }
}
