using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class RadioListsTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public RadioListsTest(LocalWebApplicationFactory factory)
           : base(factory,
                 typeof(HomeController),
                 nameof(HomeController.RadioLists),
                 null)
        {
        }

        [Fact]
        public void RadioListButtons_CanClickFirstOption()
        {
            CommonActions.ClickElement(RadioListsObjects.FirstOption);

            RadioListsActions
                 .GetElementChecked(RadioListsObjects.FirstOption)
                .Should().Be("True");
        }

        [Fact]
        public void RadioListButtons_CanClickSecondOption()
        {
            CommonActions.ClickElement(RadioListsObjects.SecondOption);

            RadioListsActions
                 .GetElementChecked(RadioListsObjects.SecondOption)
                .Should().Be("True");
        }

        [Fact]
        public void RadioListButtons_CanClickThirdOption()
        {
            CommonActions.ClickElement(RadioListsObjects.ThirdOption);

            CommonActions
                 .GetElementChecked(RadioListsObjects.ThirdOption)
                .Should().Be("True");
        }
    }
}
