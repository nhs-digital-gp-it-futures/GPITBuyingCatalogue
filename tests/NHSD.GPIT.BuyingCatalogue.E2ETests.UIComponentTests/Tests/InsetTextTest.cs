using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class InsetTextTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public InsetTextTest(LocalWebApplicationFactory factory)
                         : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.InsetText),
                        null)

        {
        }

        [Fact]
        public void InsetText_LoadedCorrectGetIndex()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.InsetText))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void InsetText_TitleDisplaysCorrectly()
        {
            CommonActions.PageTitle()
               .Should()
               .BeEquivalentTo($"Insettext-TagHelper".FormatForComparison());
        }
    }
}
