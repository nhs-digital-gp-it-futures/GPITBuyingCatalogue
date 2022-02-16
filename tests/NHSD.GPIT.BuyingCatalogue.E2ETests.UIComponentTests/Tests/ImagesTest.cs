using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class ImagesTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ImagesTest(LocalWebApplicationFactory factory)
            : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.Images),
                        null)
        {
        }

        [Fact]
        public void Images_VerifyThatImagesPageIsLoaded()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                 typeof(HomeController),
                 nameof(HomeController.Images))
                 .Should()
                 .BeTrue();
        }

        [Fact]
        public void Images_IsTrueImageIsDisplayed()
        {
            CommonActions.IsElementDisplayed(ImagesObjects.Images).Should().BeTrue();
        }
    }
}
