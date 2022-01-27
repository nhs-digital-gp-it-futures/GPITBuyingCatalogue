using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class SelectListTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public SelectListTest(LocalWebApplicationFactory factory)
            : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.SelectList),
                        null)
        {
        }

        [Theory]
        [InlineData("First Option", "1")]
        [InlineData("Second Option", "2")]
        [InlineData("Third Option", "3")]
        public void SelectList_SelectAnItemFromTheDropDown(string text, string expectedValue)
        {
            CommonActions.SelectDropDownItemByText(SelectListObject.SelectList, text);
            CommonActions.GetSelectDropDownValue(SelectListObject.SelectList).Should().Be(expectedValue);
        }
    }
}
