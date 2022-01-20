using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class YesNoRadioButtons : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public YesNoRadioButtons(LocalWebApplicationFactory factory)
            : base(factory,
                  typeof(HomeController),
                  nameof(HomeController.YesNoRadios),
                  null)
        {
        }

        [Fact]
        public void YesNoRadioButtons_CanClickYes()
        {
            CommonActions.ClickElement(YesNoRadioButtonObjects.Yes);

            YesNoRadioButtonActions
                .GetElementChecked(YesNoRadioButtonObjects.Yes)
                .Should().Be("True");
        }

        [Fact]
        public void YesNoRadioButtons_CanClickNo()
        {
            CommonActions.ClickElement(YesNoRadioButtonObjects.No);

            YesNoRadioButtonActions
                .GetElementChecked(YesNoRadioButtonObjects.No)
                .Should().Be("True");
        }
    }
}
