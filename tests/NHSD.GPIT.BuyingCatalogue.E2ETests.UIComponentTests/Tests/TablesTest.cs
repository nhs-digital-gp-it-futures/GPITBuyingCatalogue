using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class TablesTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public TablesTest(LocalWebApplicationFactory factory)

              : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.Table),
                        null)
        {
        }

        [Fact]
        public void Table_TitleDisplaysCorrectly()
        {
            CommonActions.PageTitle()
               .Should()
               .BeEquivalentTo($"Tables-TagHelper".FormatForComparison());
        }

        [Fact]
        public void Table_LoadedCorrectGetIndex()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(HomeController),
                nameof(HomeController.Table))
                .Should()
                .BeTrue();
        }
    }
}
