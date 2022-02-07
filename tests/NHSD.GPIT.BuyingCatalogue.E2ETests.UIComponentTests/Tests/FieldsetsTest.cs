using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Objects;
using NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Utils;
using NHSD.GPIT.BuyingCatalogue.UI.Components.WebApp.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Tests
{
    public sealed class FieldsetsTest : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public FieldsetsTest(LocalWebApplicationFactory factory)
             : base(factory,
                        typeof(HomeController),
                        nameof(HomeController.FieldSets),
                        null)
        {
        }

        [Fact]
        public void FieldSets_FieldSetsHeader()
        {
            CommonActions.IsElementDisplayed(FieldSetsObjects.FieldSets).Should().BeTrue();
        }

        [Fact]
        public void FieldSets_FieldSetsRadioListsLinkIsDisplayed()
        {
            CommonActions.GetElementText(FieldSetsObjects.FieldSetsRadioListsLink).
                Contains("Click here to view how they look on Radio Lists")
                .Should().BeTrue();
        }

        [Fact]
        public void FieldSets_FieldSetsCheckboxesLinkIsDisplayed()
        {
            CommonActions.GetElementText(FieldSetsObjects.FieldSetsCheckboxesLink).
                Contains("Click here to view how they look on checkboxes")
                .Should().BeTrue();
        }

        [Fact]
        public void FieldSets_FieldSetsDateInputsLinkIsDisplayed()
        {
            CommonActions.GetElementText(FieldSetsObjects.FieldSetsDateInputsLink).
                Contains("Click here to view how they look on Date Inputs")
                .Should().BeTrue();
        }

        [Fact]
        public void FieldSets_FieldSetsYesNoRadiosLinkIsDisplayed()
        {
            CommonActions.GetElementText(FieldSetsObjects.FieldSetsYesNoRadiosLink).
                Contains("Click here to view how they look on Yes No Radios")
                .Should().BeTrue();
        }
    }
}
