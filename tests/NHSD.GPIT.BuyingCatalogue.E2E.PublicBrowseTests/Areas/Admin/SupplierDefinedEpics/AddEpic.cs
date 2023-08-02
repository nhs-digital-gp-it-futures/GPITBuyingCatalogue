using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
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
                  nameof(SupplierDefinedEpicsController.AddSupplierDefinedEpicDetails),
                  null)
        {
        }

        [Fact]
        public void AddEpicDetails_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().Be("Supplier defined Epic details".FormatForComparison());
            CommonActions.LedeText().Should().Be("Provide the following details for this supplier defined Epic.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddSupplierDefinedEpicObjects.NameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddSupplierDefinedEpicObjects.DescriptionInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddSupplierDefinedEpicObjects.StatusInput).Should().BeTrue();

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void AddEpicDetails_ClickGoBack_Redirects()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.SelectCapabilities))
                .Should().BeTrue();
        }

        [Fact]
        public void AddEpicDetails_NoInput_ThrowsError()
        {
            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                AddSupplierDefinedEpicObjects.NameInputError,
                "Enter a name")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                AddSupplierDefinedEpicObjects.StatusInputError,
                "Error: Select a status")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                AddSupplierDefinedEpicObjects.DescriptionInputError,
                "Enter a description")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddEpicDetails_DuplicatesExistingEpic_ThrowsError()
        {
            NavigateToUrl(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.AddSupplierDefinedEpicDetails),
                null,
                new Dictionary<string, string>
                {
                    { "selectedCapabilityIds", "1.2" },
                });
            using var context = GetEndToEndDbContext();
            var epic = context.Epics.Include(x => x.Capabilities).First(e => e.Id == "S00001");

            CommonActions.ElementAddValue(AddSupplierDefinedEpicObjects.NameInput, epic.Name);
            CommonActions.ElementAddValue(AddSupplierDefinedEpicObjects.DescriptionInput, epic.Description);
            CommonActions.ClickRadioButtonWithValue(epic.IsActive.ToString());

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
        }

        [Fact]
        public void AddEpicDetails_Valid_SavesEpic()
        {
            NavigateToUrl(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.AddSupplierDefinedEpicDetails),
                null,
                new Dictionary<string, string>
                {
                    { "selectedCapabilityIds", "1.2" },
                });
            var nameText = TextGenerators.TextInputAddText(AddSupplierDefinedEpicObjects.NameInput, 500);
            var descriptionText = TextGenerators.TextInputAddText(AddSupplierDefinedEpicObjects.DescriptionInput, 1000);

            CommonActions.ClickRadioButtonWithText("Active");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.EditSupplierDefinedEpic))
                .Should().BeTrue();

            using var context = GetEndToEndDbContext();
            var epic = context.Epics.First(e => e.Name == nameText && e.Description == descriptionText);
            epic.Should().NotBeNull();

            context.Epics.Remove(epic);
            context.SaveChanges();
        }
    }
}
