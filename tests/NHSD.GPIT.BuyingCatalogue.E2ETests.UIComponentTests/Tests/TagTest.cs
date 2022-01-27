using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class TagTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public TagTest(LocalWebApplicationFactory factory)
             : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.Tags),
                        null)

        {
        }

        [Fact]
        public void Tag_GetTagsHeader()
        {
            CommonActions.GetElementText(CommonObject.Header1).
               Contains("Tag")
               .Should().BeTrue();
        }
    }
}
