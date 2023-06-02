using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.SupplierDefinedEpics
{
    [Collection(nameof(AdminCollection))]
    public sealed class AddEpic : AuthorityTestBase
    {
        public AddEpic(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SupplierDefinedEpicsController),
                  nameof(SupplierDefinedEpicsController.AddEpic),
                  null)
        {
        }

        [Fact]
        public void AddEpic_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().Be("Supplier defined Epic details".FormatForComparison());
            CommonActions.LedeText().Should().Be("Provide the following details about the supplier defined Epic.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddSupplierDefinedEpicObjects.CapabilityInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddSupplierDefinedEpicObjects.NameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddSupplierDefinedEpicObjects.DescriptionInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddSupplierDefinedEpicObjects.StatusInput).Should().BeTrue();

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AddEpic_ClickGoBack_Redirects()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.Dashboard))
                .Should().BeTrue();
        }

        [Fact]
        public void AddEpic_NoInput_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                AddSupplierDefinedEpicObjects.CapabilityInputError,
                "Select a Capability")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                AddSupplierDefinedEpicObjects.NameInputError,
                "Enter an Epic name")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                AddSupplierDefinedEpicObjects.DescriptionInputError,
                "Enter a description")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                AddSupplierDefinedEpicObjects.StatusInputError,
                "Error: Select a status")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddEpic_DuplicatesExistingEpic_ThrowsError()
        {
            using var context = GetEndToEndDbContext();
            var epic = context.Epics.Include(x => x.Capabilities).First(e => e.Id == "S00001");

            CommonActions.ElementAddValue(AddSupplierDefinedEpicObjects.NameInput, epic.Name);
            CommonActions.ElementAddValue(AddSupplierDefinedEpicObjects.DescriptionInput, epic.Description);
            CommonActions.SelectDropDownItemByValue(AddSupplierDefinedEpicObjects.CapabilityInput, epic.Capabilities.First().Id.ToString());
            CommonActions.ClickRadioButtonWithValue(epic.IsActive.ToString());

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
        }

        [Fact]
        public void AddEpic_Valid_SavesEpic()
        {
            var nameText = TextGenerators.TextInputAddText(AddSupplierDefinedEpicObjects.NameInput, 500);
            var descriptionText = TextGenerators.TextInputAddText(AddSupplierDefinedEpicObjects.DescriptionInput, 1000);
            CommonActions.SelectRandomDropDownItem(AddSupplierDefinedEpicObjects.CapabilityInput);
            CommonActions.ClickRadioButtonWithText("Active");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.Dashboard))
                .Should().BeTrue();

            using var context = GetEndToEndDbContext();
            var epic = context.Epics.First(e => e.Name == nameText && e.Description == descriptionText);
            epic.Should().NotBeNull();

            context.Epics.Remove(epic);
            context.SaveChanges();
        }
    }
}
