﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.SupplierDefinedEpics
{
    public sealed class EditEpic : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string EpicId = "S00001";
        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(EpicId), EpicId },
        };

        public EditEpic(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SupplierDefinedEpicsController),
                  nameof(SupplierDefinedEpicsController.EditEpic),
                  Parameters)
        {
        }

        [Fact]
        public void EditEpic_AllSectionsDisplayed()
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
        public void EditEpic_ClickGoBack_Redirects()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.Dashboard))
                .Should().BeTrue();
        }

        [Fact]
        public void EditEpic_NoInput_ThrowsError()
        {
            CommonActions.SelectDropDownItem(AddSupplierDefinedEpicObjects.CapabilityInput, 0);
            CommonActions.ClearInputElement(AddSupplierDefinedEpicObjects.NameInput);
            CommonActions.ClearInputElement(AddSupplierDefinedEpicObjects.DescriptionInput);

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
        }

        [Fact]
        public void EditEpic_DuplicatesExistingEpic_ThrowsError()
        {
            using var context = GetEndToEndDbContext();
            var epic = context.Epics.Single(e => e.Id == "S00002");

            CommonActions.ElementAddValue(AddSupplierDefinedEpicObjects.NameInput, epic.Name);
            CommonActions.ElementAddValue(AddSupplierDefinedEpicObjects.DescriptionInput, epic.Description);
            CommonActions.SelectDropDownItemByValue(AddSupplierDefinedEpicObjects.CapabilityInput, epic.CapabilityId.ToString());
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
            var selectedCapabilityId = CommonActions.SelectRandomDropDownItem(AddSupplierDefinedEpicObjects.CapabilityInput);
            CommonActions.ClickRadioButtonWithText("Active");

            var expectedEpic = new Epic
            {
                Name = nameText,
                Description = descriptionText,
                IsActive = true,
                CompliancyLevel = CompliancyLevel.May,
                CapabilityId = int.Parse(selectedCapabilityId),
                SupplierDefined = true,
            };

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.Dashboard))
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
                        .Excluding(m => m.LastUpdatedBy));
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
                nameof(SupplierDefinedEpicsController.EditEpic),
                parameters);

            CommonActions.ElementIsDisplayed(EditSupplierDefinedEpicObjects.RelatedItemsTable).Should().BeTrue();
            CommonActions.ElementExists(EditSupplierDefinedEpicObjects.RelatedItemsInset).Should().BeFalse();
        }

        [Fact]
        public void EditEpic_WithNoReferencedItems_DisplaysInsetText_HiddenTable()
        {
            CommonActions.ElementExists(EditSupplierDefinedEpicObjects.RelatedItemsTable).Should().BeFalse();
            CommonActions.ElementIsDisplayed(EditSupplierDefinedEpicObjects.RelatedItemsInset).Should().BeTrue();
        }

        [Fact]
        public void EditEpic_ActiveToInactive_NoReferences_BecomesInactive()
        {
            using var context = GetEndToEndDbContext();

            CommonActions.ClickRadioButtonWithText("Inactive");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.Dashboard))
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
                nameof(SupplierDefinedEpicsController.EditEpic),
                parameters);

            CommonActions.ClickRadioButtonWithText("Inactive");

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.EditEpic))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();
            CommonActions.ElementShowingCorrectErrorMessage(
                EditSupplierDefinedEpicObjects.StatusInputError,
                "This supplier defined Epic cannot be set to inactive as it is referenced by another solution or service");
        }

        [Fact]
        public void EditEpic_InactiveToActive_BecomesActive()
        {
            using var context = GetEndToEndDbContext();
            var parameters = new Dictionary<string, string>
            {
                { nameof(EpicId), "S00003" },
            };

            NavigateToUrl(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.EditEpic),
                parameters);

            CommonActions.ClickRadioButtonWithText("Active");

            CommonActions.ClickSave();

            var epic = context.Epics.Find("S00003");
            epic.IsActive.Should().BeTrue();
        }

        [Fact]
        public void EditEpic_ReferencedBySolution_ClickEditLink()
        {
            const string epicId = "S00002";
            using var context = GetEndToEndDbContext();
            var epic = context.CatalogueItemEpics
                .Include(e => e.CatalogueItem)
                .Single(e => e.EpicId == epicId && e.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution);

            var parameters = new Dictionary<string, string>
            {
                { nameof(EpicId), epicId },
            };

            NavigateToUrl(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.EditEpic),
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
                .Single(e => e.EpicId == epicId && e.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService);

            var parameters = new Dictionary<string, string>
            {
                { nameof(EpicId), epicId },
            };

            NavigateToUrl(
                typeof(SupplierDefinedEpicsController),
                nameof(SupplierDefinedEpicsController.EditEpic),
                parameters);

            CommonActions.ClickLinkElement(By.XPath($"//tr[td//text()[contains(., '{epic.CatalogueItem.Name}')]]/td[3]/a"));

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(AdditionalServicesController),
                nameof(AdditionalServicesController.EditCapabilities))
                .Should().BeTrue();
        }
    }
}
