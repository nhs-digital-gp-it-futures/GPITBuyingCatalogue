﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.SupplierDefinedEpics
{
    [Collection(nameof(AdminCollection))]
    public sealed class EditEpicDetails : AuthorityTestBase
    {
        private const string EpicId = "S00001";

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(EpicId), EpicId },
        };

        public EditEpicDetails(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SupplierDefinedEpicsController),
                  nameof(SupplierDefinedEpicsController.EditSupplierDefinedEpicDetails),
                  Parameters)
        {
        }

        [Fact]
        public void EditEpic_AllSectionsDisplayed()
        {
            CommonActions.PageTitle().Should().Be("Supplier defined Epic details".FormatForComparison());
            CommonActions.LedeText().Should().Be("Provide the following details about the supplier defined Epic.".FormatForComparison());

            CommonActions.GoBackLinkDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(AddSupplierDefinedEpicObjects.NameInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddSupplierDefinedEpicObjects.DescriptionInput).Should().BeTrue();
            CommonActions.ElementIsDisplayed(AddSupplierDefinedEpicObjects.StatusInput).Should().BeTrue();

            CommonActions.SaveButtonDisplayed().Should().BeTrue();
        }

        [Fact]
        public void EditEpic_ClickGoBack_Redirects()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.EditSupplierDefinedEpic))
                .Should().BeTrue();
        }

        [Fact]
        public void EditEpic_NoInput_ThrowsError()
        {
            CommonActions.ClearInputElement(AddSupplierDefinedEpicObjects.NameInput);
            CommonActions.ClearInputElement(AddSupplierDefinedEpicObjects.DescriptionInput);

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                AddSupplierDefinedEpicObjects.NameInputError,
                "Enter a name")
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                AddSupplierDefinedEpicObjects.DescriptionInputError,
                "Enter a description")
                .Should()
                .BeTrue();
        }

        [Fact]
        public void EditEpic_DuplicatesExistingEpic_ThrowsError()
        {
            using var context = GetEndToEndDbContext();
            var epic = context.Epics.Include(x => x.Capabilities).First(e => e.Id == "S00002");

            CommonActions.ElementAddValue(AddSupplierDefinedEpicObjects.NameInput, epic.Name);
            CommonActions.ElementAddValue(AddSupplierDefinedEpicObjects.DescriptionInput, epic.Description);
            CommonActions.ClickRadioButtonWithValue(epic.IsActive.ToString());

            CommonActions.ClickSave();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
        }

        [Fact]
        public void EditEpic_Valid_UpdatesEpic()
        {
            var nameText = TextGenerators.TextInputAddText(AddSupplierDefinedEpicObjects.NameInput, 500);
            var descriptionText = TextGenerators.TextInputAddText(AddSupplierDefinedEpicObjects.DescriptionInput, 1000);
            CommonActions.ClickRadioButtonWithText("Active");

            var expectedEpic = new Epic
            {
                Name = nameText,
                Description = descriptionText,
                IsActive = true,
                SupplierDefined = true,
            };

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.EditSupplierDefinedEpic))
                .Should().BeTrue();

            using var context = GetEndToEndDbContext();
            var epic = context.Epics.Find(EpicId);
            epic
                .Should()
                .BeEquivalentTo(
                    expectedEpic,
                    opt => opt
                        .Excluding(m => m.Id)
                        .Excluding(m => m.LastUpdated)
                        .Excluding(m => m.LastUpdatedBy)
                        .Excluding(m => m.Capabilities));
        }

        [Fact]
        public void EditEpic_WithReferencedItems_DisplaysTable_HiddenInsetText()
        {
            var parameters = new Dictionary<string, string>
            {
                { nameof(EpicId), "S00002" },
            };

            NavigateToUrl(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.EditSupplierDefinedEpicDetails),
                parameters);

            CommonActions.ElementIsDisplayed(EditSupplierDefinedEpicDetailsObjects.RelatedItemsTable).Should().BeTrue();
            CommonActions.ElementExists(EditSupplierDefinedEpicDetailsObjects.RelatedItemsInset).Should().BeFalse();
        }

        [Fact]
        public void EditEpic_WithNoReferencedItems_DisplaysInsetText_HiddenTable()
        {
            CommonActions.ElementExists(EditSupplierDefinedEpicDetailsObjects.RelatedItemsTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(EditSupplierDefinedEpicDetailsObjects.RelatedItemsInset).Should().BeTrue();
        }

        [Fact]
        public void EditEpic_ActiveToInactive_NoReferences_BecomesInactive()
        {
            using var context = GetEndToEndDbContext();

            CommonActions.ClickRadioButtonWithText("Inactive");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.EditSupplierDefinedEpic))
                .Should().BeTrue();

            var epic = context.Epics.Find(EpicId);
            epic.IsActive.Should().BeFalse();
        }

        [Fact]
        public void EditEpic_ActiveToInactive_References_ThrowsError()
        {
            var parameters = new Dictionary<string, string>
            {
                { nameof(EpicId), "S00002" },
            };

            NavigateToUrl(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.EditSupplierDefinedEpicDetails),
                parameters);

            CommonActions.ClickRadioButtonWithText("Inactive");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.EditSupplierDefinedEpicDetails))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(
                EditSupplierDefinedEpicDetailsObjects.StatusInputError,
                "This supplier defined Epic cannot be set to inactive as it is referenced by another solution or service");
        }

        [Fact]
        public void EditEpic_InactiveToActive_BecomesActive()
        {
            var parameters = new Dictionary<string, string>
            {
                { nameof(EpicId), "S00003" },
            };

            NavigateToUrl(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.EditSupplierDefinedEpicDetails),
                parameters);

            CommonActions.ClickRadioButtonWithText("Active");

            CommonActions.ClickSave();

            using var updatedContext = GetEndToEndDbContext();
            updatedContext.Epics.Find("S00003").IsActive.Should().BeTrue();
        }

        [Fact]
        public void EditEpic_ReferencedBySolution_ClickEditLink()
        {
            const string epicId = "S00002";
            using var context = GetEndToEndDbContext();
            var epic = context.CatalogueItemEpics
                .Include(e => e.CatalogueItem)
                .First(e => e.EpicId == epicId && e.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution);

            var parameters = new Dictionary<string, string>
            {
                { nameof(EpicId), epicId },
            };

            NavigateToUrl(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.EditSupplierDefinedEpicDetails),
                parameters);

            CommonActions.ClickLinkElement(By.XPath($"//tr[td//text()[contains(., '{epic.CatalogueItem.Name}')]]/td[3]/a"));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CatalogueSolutionsController),
                nameof(CatalogueSolutionsController.EditCapabilities))
                .Should().BeTrue();
        }

        [Fact]
        public void EditEpic_ReferencedByAdditionalService_ClickEditLink()
        {
            const string epicId = "S00004";
            using var context = GetEndToEndDbContext();
            var epic = context.CatalogueItemEpics
                .Include(e => e.CatalogueItem)
                .First(e => e.EpicId == epicId && e.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService);

            var parameters = new Dictionary<string, string>
            {
                { nameof(EpicId), epicId },
            };

            NavigateToUrl(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.EditSupplierDefinedEpicDetails),
                parameters);

            CommonActions.ClickLinkElement(By.XPath($"//tr[td//text()[contains(., '{epic.CatalogueItem.Name}')]]/td[3]/a"));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditCapabilities))
                .Should().BeTrue();
        }
    }
}
