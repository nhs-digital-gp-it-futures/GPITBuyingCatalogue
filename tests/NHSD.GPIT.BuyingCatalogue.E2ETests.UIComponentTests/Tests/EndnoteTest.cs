using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Actions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class EndnoteTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public EndnoteTest(LocalWebApplicationFactory factory)
            : base(factory,
                 typeof(HomeController),
                 nameof(HomeController.EndNote),
                 null)
        {
        }

        [Fact]
        public void Endnote_ReturnsEndnoteCode()
        {
            CommonActions.GetElementText(EndNoteObjects.CodeExampleForEndnote).
                Contains("The Paragraphs Above were written to describe when to use endnotes.")
                .Should().BeTrue();
        }

        [Fact]
        public void EndNote_VerifyThatEndNotePageIsLoaded()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                 typeof(HomeController),
                 nameof(HomeController.EndNote))
                 .Should()
                 .BeTrue();
        }
    }
}
