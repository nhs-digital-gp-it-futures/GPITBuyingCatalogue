using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class WarningCalloutTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public WarningCalloutTest(LocalWebApplicationFactory factory)
             : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.WarningCallout),
                        null)
        {
        }

        [Fact]
        public void Address_VerifyThatWarningCalloutPageIsLoaded()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                 typeof(HomeController),
                 nameof(HomeController.WarningCallout))
                 .Should()
                 .BeTrue();
        }

        [Fact]
        public void WarningCallout_TitleDisplaysCorrectly()
        {
            CommonActions.PageTitle()
               .Should()
               .BeEquivalentTo($"Warningcallout-TagHelper".FormatForComparison());
        }
    }
}
