using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Suppliers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Suppliers
{
    public static class EditSupplierModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void EditSupplier_InactiveSupplier_TryToSetActive_AddressNotCompleted_ThrowsError(
            Supplier supplier,
            EditSupplierModel model,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditSupplierModelValidator validator)
        {
            supplier.IsActive = false;
            supplier.Address = null;

            suppliersService.Setup(x => x.GetSupplier(supplier.Id))
                .ReturnsAsync(supplier);

            model.SupplierId = supplier.Id;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupplierStatus).WithErrorMessage(EditSupplierModelValidator.MandatorySectionsMissing);
        }

        [Theory]
        [CommonAutoData]
        public static void EditSupplier_InactiveSupplier_TryToSetActive_ContactsNotCompleted_ThrowsError(
            Supplier supplier,
            EditSupplierModel model,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditSupplierModelValidator validator)
        {
            supplier.IsActive = false;
            supplier.SupplierContacts = new List<SupplierContact>();

            suppliersService.Setup(x => x.GetSupplier(supplier.Id))
                .ReturnsAsync(supplier);

            model.SupplierId = supplier.Id;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupplierStatus).WithErrorMessage(EditSupplierModelValidator.MandatorySectionsMissing);
        }

        [Theory]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        [CommonInlineAutoData(null)]
        public static void EditSupplier_InactiveSupplier_TryToSetActive_NameIsNull_ThrowsError(
            string name,
            Supplier supplier,
            EditSupplierModel model,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditSupplierModelValidator validator)
        {
            supplier.IsActive = false;
            supplier.Name = name;

            suppliersService.Setup(x => x.GetSupplier(supplier.Id))
                .ReturnsAsync(supplier);

            model.SupplierId = supplier.Id;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupplierStatus).WithErrorMessage(EditSupplierModelValidator.MandatorySectionsMissing);
        }

        [Theory]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        [CommonInlineAutoData(null)]
        public static void EditSupplier_InactiveSupplier_TryToSetActive_LegalNameIsNull_ThrowsError(
            string legalName,
            Supplier supplier,
            EditSupplierModel model,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditSupplierModelValidator validator)
        {
            supplier.IsActive = false;
            supplier.LegalName = legalName;

            suppliersService.Setup(s => s.GetSupplier(supplier.Id))
                .ReturnsAsync(supplier);

            model.SupplierId = supplier.Id;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupplierStatus).WithErrorMessage(EditSupplierModelValidator.MandatorySectionsMissing);
        }

        [Theory]
        [CommonAutoData]
        public static void EditSupplier_SupplierHasPublishedSolutions_TryToSetInactive_ThrowsError(
            Supplier supplier,
            List<CatalogueItem> catalogueItems,
            EditSupplierModel model,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditSupplierModelValidator validator)
        {
            suppliersService.Setup(c => c.GetAllSolutionsForSupplier(supplier.Id))
                .ReturnsAsync(catalogueItems);

            model.SupplierId = supplier.Id;
            model.CurrentStatus = true;
            model.SupplierStatus = false;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SupplierStatus).WithErrorMessage(validator.PublishedSolutionErrorMessage(model, model.SupplierStatus));
        }

        [Theory]
        [CommonAutoData]
        public static void EditSupplier_SupplierStatusNotChange_NoModelError(
            EditSupplierModel model,
            EditSupplierModelValidator validator)
        {
            model.SupplierStatus = model.CurrentStatus;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [CommonAutoData]
        public static void EditSupplier_NoPublishedSolutions_SetInactive_NoModelError(
            EditSupplierModel model,
            Supplier supplier,
            List<CatalogueItem> catalogueItems,
            [Frozen] Mock<ISuppliersService> suppliersService,
            EditSupplierModelValidator validator)
        {
            supplier.IsActive = true;

            suppliersService.Setup(s => s.GetSupplier(supplier.Id))
                .ReturnsAsync(supplier);

            suppliersService.Setup(s => s.GetAllSolutionsForSupplier(supplier.Id))
                .ReturnsAsync(catalogueItems.Where(c => c.PublishedStatus != PublicationStatus.Published).ToList());

            model.SupplierStatus = false;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
