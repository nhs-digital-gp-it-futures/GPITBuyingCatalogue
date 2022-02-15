using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class SummaryListTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public SummaryListTest(LocalWebApplicationFactory factory)

             : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.SummaryList),
                        null)
        {
        }

        [Fact]
        public void SummaryList_VerifyThatSummaryListPageIsLoaded()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                 typeof(HomeController),
                 nameof(HomeController.SummaryList))
                 .Should()
                 .BeTrue();
        }

        [Fact]
        public void SummaryList_EnterValueIntoSummaryListInputBox()
        {
            var text = Strings.RandomString(500);
            CommonActions.SendTextToElement(SummaryListObject.SummaryList, text);

            CommonActions.GetElementValue(SummaryListObject.SummaryList).Should().Be(text);
        }
    }
}
