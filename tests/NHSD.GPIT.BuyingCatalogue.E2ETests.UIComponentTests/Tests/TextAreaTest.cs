using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class TextAreaTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public TextAreaTest(LocalWebApplicationFactory factory)

             : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.TextArea),
                        null)
        {
        }

        [Fact]
        public void TextArea_LoadedCorrectly()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.TextArea))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void TextArea_TitleDisplaysCorrectly()
        {
            CommonActions.PageTitle()
               .Should()
               .BeEquivalentTo($"Textarea-TagHelpers".FormatForComparison());
        }

        [Fact]
        public void TextArea_Label1500CharactersIsDisplayed()
        {
            CommonActions.IsElementDisplayed(TextAreaObject.TextAreaLabel1500Characters).Should().BeTrue();
        }

        [Fact]
        public void TextArea_LabelCharacterCountTurnedOffIsDisplayed()
        {
            CommonActions.IsElementDisplayed(TextAreaObject.TextAreaLabelCharacterCountTurnedOff).Should().BeTrue();
        }

        [Fact]
        public void TextArea_EnterAvalueIntoInputBox1()
        {
            var text = Strings.RandomString(100);
            CommonActions.SendTextToElement(TextInputBoxObjects.TextBox100Character, text);

            CommonActions.GetElementValue(TextInputBoxObjects.TextBox100Character).Should().Be(text);
        }
    }
}
