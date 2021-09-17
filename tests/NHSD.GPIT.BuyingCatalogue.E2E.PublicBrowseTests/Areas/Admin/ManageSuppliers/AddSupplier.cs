﻿using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Admin.ManageSuppliers
{
    public sealed class AddSupplier : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const int TargetSupplierId = 99998;

        public AddSupplier(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(SuppliersController),
                  nameof(SuppliersController.AddSupplierDetails),
                  null)
        {
        }

        [Fact]
        public void AddSupplier_AllSectionsDisplayed()
        {
            CommonActions.ElementIsDisplayed(CommonSelectors.Header1).Should().BeTrue();
            CommonActions.GoBackLinkDisplayed().Should().BeTrue();
            CommonActions.SaveButtonDisplayed().Should().BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierLegalName).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsAboutSupplier).Should().BeTrue();
            CommonActions.ElementIsDisplayed(Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierWebsite).Should().BeTrue();
        }

        [Fact]
        public void AddSupplier_ClickGoBackLink_ExpectedResult()
        {
            CommonActions.ClickGoBackLink();

            CommonActions.PageLoadedCorrectGetIndex(typeof(SuppliersController), nameof(SuppliersController.Index))
            .Should()
            .BeTrue();
        }

        [Fact]
        public void AddSupplier_NoInput_ThrowsError()
        {
            var expectedNameErrorMessage = "Enter a supplier name";
            var expectedLegalNameErrorMessage = "Enter a supplier legal name";

            CommonActions.ClearInputElement(Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierName);
            CommonActions.ClearInputElement(Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierLegalName);

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.AddSupplierDetails))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierNameErrorMessage,
                expectedNameErrorMessage)
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierLegalNameErrorMessage,
                expectedLegalNameErrorMessage)
                .Should()
                .BeTrue();
        }

        [Fact]
        public async void AddSupplier_SupplierName_AddDuplicateName_ThrowsError()
        {
            var expectedNameErrorMessage = "Supplier name already exists. Enter a different name";
            var expectedLegalNameErrorMessage = "Supplier legal name already exists. Enter a different name";

            using var context = GetEndToEndDbContext();

            var existingSupplier = await context.Suppliers.FirstAsync(s => s.Id == TargetSupplierId);

            CommonActions.ClearInputElement(Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierName);
            CommonActions.ClearInputElement(Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierLegalName);

            CommonActions.ElementAddValue(
                Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierName,
                existingSupplier.Name);

            CommonActions.ElementAddValue(
                Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierLegalName,
                existingSupplier.LegalName);

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.AddSupplierDetails))
                .Should()
                .BeTrue();

            CommonActions.ErrorSummaryDisplayed().Should().BeTrue();
            CommonActions.ErrorSummaryLinksExist().Should().BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierNameErrorMessage,
                expectedNameErrorMessage)
                .Should()
                .BeTrue();

            CommonActions.ElementShowingCorrectErrorMessage(
                Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierLegalNameErrorMessage,
                expectedLegalNameErrorMessage)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void AddSupplier_CorrectValues_ExpectedResult()
        {
            CommonActions.ClearInputElement(Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierName);
            CommonActions.ClearInputElement(Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierLegalName);

            var suppliername = TextGenerators.TextInputAddText(Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierName, 255);
            TextGenerators.TextInputAddText(Objects.Admin.ManageSuppliers.ManageSuppliers.SupplierDetailsSupplierLegalName, 255);

            CommonActions.ClickSave();

            CommonActions
                .PageLoadedCorrectGetIndex(
                    typeof(SuppliersController),
                    nameof(SuppliersController.EditSupplier))
                .Should()
                .BeTrue();
        }
    }
}
