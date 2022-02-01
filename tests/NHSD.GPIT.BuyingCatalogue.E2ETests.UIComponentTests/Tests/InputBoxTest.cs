using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class InputBoxTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public InputBoxTest(LocalWebApplicationFactory factory)
            : base(factory,
                 typeof(HomeController),
                 nameof(HomeController.TextInput),
                 null)
        {
        }

        [Fact]
        public void TextInputBox_EnterAvalueIntoInputBox1()
        {
            var text = Strings.RandomString(100);
            CommonActions.SendTextToElement(TextInputBoxObjects.TextBox100Character, text);

            CommonActions.GetElementValue(TextInputBoxObjects.TextBox100Character).Should().Be(text);
        }

        [Fact]
        public void TextInputBox_EnterAvalueIntoInputBox2()
        {
            var text = Strings.RandomString(500);
            CommonActions.SendTextToElement(TextInputBoxObjects.TextBox500Character, text);

            CommonActions.GetElementValue(TextInputBoxObjects.TextBox500Character).Should().Be(text);
        }

        [Fact]
        public void TextInputBox_EnterPassword()
        {
            var text = Strings.RandomString(500);
            CommonActions.SendTextToElement(TextInputBoxObjects.TextInputPassword, text);

            CommonActions.GetElementValue(TextInputBoxObjects.TextInputPassword).Should().Be(text);
        }
    }
}
