using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class BookendInputTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public BookendInputTest(LocalWebApplicationFactory factory)

             : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.BookendedTextInput),
                        null)
        {
        }

        [Fact]
        public void Address_VerifyThatBookendedTextInputPageIsLoaded()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                 typeof(HomeController),
                 nameof(HomeController.BookendedTextInput))
                 .Should()
                 .BeTrue();
        }

        [Fact]
        public void BookendedTextInput_TitleDisplaysCorrectly()
        {
            CommonActions.PageTitle()
               .Should()
               .BeEquivalentTo($"Bookendedinputs-TagHelpers".FormatForComparison());
        }

        [Fact]
        public void BookendedInput_Enter100CharacterIntoBookendTextInputBox()
        {
            var text = Strings.RandomString(100);
            CommonActions.SendTextToElement(BookendedInputsObjects.BackendInput100Character, text);

            CommonActions.GetElementValue(BookendedInputsObjects.BackendInput100Character).Should().Be(text);
        }

        [Fact]
        public void BookendedInput_Enter10CharacterIntoBookendTextInputBox()
        {
            var text = Strings.RandomString(10);
            CommonActions.SendTextToElement(BookendedInputsObjects.BackendInput10Character, text);

            CommonActions.GetElementValue(BookendedInputsObjects.BackendInput10Character).Should().Be(text);
        }
    }
}
