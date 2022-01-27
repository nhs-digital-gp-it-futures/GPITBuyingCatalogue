using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class CheckBoxesTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public CheckBoxesTest(LocalWebApplicationFactory factory)
                  : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.Checkboxes),
                        null)
        {
        }

        [Fact]
        public void CheckBoxButtons_CanClickSingleCheckBox()
        {
            CommonActions.ClickElement(CheckBoxesObjects.SingleCheckBoxProperty);

            CommonActions
                 .GetElementChecked(CheckBoxesObjects.SingleCheckBoxProperty)
                .Should().Be("True");
        }

        [Fact]
        public void CheckBoxButtons_CanClickAnotherCheckBoxProperty()
        {
            CommonActions.ClickElement(CheckBoxesObjects.AnotherCheckBoxProperty);

            CommonActions
                 .GetElementChecked(CheckBoxesObjects.AnotherCheckBoxProperty)
                .Should().Be("True");
        }

        [Fact]
        public void CheckBoxButtons_AllDisplayed()
        {
            var checkboxes = CommonActions.GetElements(By.CssSelector("input[type='checkbox']"));

            checkboxes.Should().NotBeEmpty().And.HaveCount(8);
        }

        [Fact]
        public void CheckBoxButtons_CanClickOnAnotherEmbeddedCheckbox()
        {
            CommonActions.ClickElement(CheckBoxesObjects.ConditionalSingleCheckBoxProperty);

            CommonActions.ClickElement(CheckBoxesObjects.EmbeddedCheckBoxProperty);

            CommonActions
                .GetElementChecked(CheckBoxesObjects.EmbeddedCheckBoxProperty)
               .Should().Be("True");
        }

        [Fact]
        public void CheckBoxButtons_EnterAvalueIntoInputBox2()
        {
            var text = Strings.RandomString(500);
            CommonActions.ClickElement(CheckBoxesObjects.ConditionalAnotherCheckBoxProperty);

            CommonActions.SendTextToElement(CheckBoxesObjects.ForInput, text);

            CommonActions.GetElementValue(CheckBoxesObjects.ForInput).Should().Be(text);
        }
    }
}
