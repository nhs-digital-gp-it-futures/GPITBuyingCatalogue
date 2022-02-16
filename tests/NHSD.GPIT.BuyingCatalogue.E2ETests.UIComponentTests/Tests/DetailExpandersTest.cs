using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Actions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public class DetailExpandersTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public DetailExpandersTest(LocalWebApplicationFactory factory)

            : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.DetailsAndExpanders),
                        null)
        {
        }

        [Fact]
        public void DetailsAndExpanders_VerifyThatDetailsAndExpandersPageIsLoaded()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                 typeof(HomeController),
                 nameof(HomeController.DetailsAndExpanders))
                 .Should()
                 .BeTrue();
        }

        [Fact]
        public void DetailsAndExpanders_DropDownWithSomeParagraphIsDislayed()
        {
            CommonActions.IsElementDisplayed(DetailsAndExpandersObjects.DetailsDropDownWithParagraph).Should().BeTrue();
        }

        [Fact]
        public void DetailsAndExpanders_AllheadersDisplayed()
        {
            var headers = CommonActions.GetElements(CommonObject.Header1);

            headers.Should().NotBeEmpty().And.HaveCount(3);
        }

        [Fact]
        public void DetailsAndExpanders_GetExpanderDropdownlinkCount()
        {
            var headers = CommonActions.GetElements(DetailsAndExpandersObjects.ExpanderDropdowns);

            headers.Should().NotBeEmpty().And.HaveCount(24);
        }

        [Fact]
        public void DetailsAndExpanders_PageTitleIsDisplayed()
        {
            CommonActions.IsElementDisplayed(DetailsAndExpandersObjects.ExpanderDropdowns).Should().BeTrue();
        }
    }
}
