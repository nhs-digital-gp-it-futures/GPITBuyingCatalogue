using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Actions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public class AddressTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AddressTest(LocalWebApplicationFactory factory)

              : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.Address),
                        null)
        {
        }

        [Fact]
        public void Address_GetHeader()
        {
            CommonActions.GetElementText(CommonObject.Header1).
              Contains("Address")
              .Should().BeTrue();
        }

        [Fact]
        public void Address_VerifyThatAddressPageIsLoaded()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                 typeof(HomeController),
                 nameof(HomeController.Address))
                 .Should()
                 .BeTrue();
        }
    }
}
