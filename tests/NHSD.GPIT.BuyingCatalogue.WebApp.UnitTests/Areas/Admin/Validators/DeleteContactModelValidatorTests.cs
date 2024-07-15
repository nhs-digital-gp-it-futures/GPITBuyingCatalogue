using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentValidation.TestHelper;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators
{
    public static class DeleteContactModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_WhenInactiveSupplierHasASingleContact_NoValidationErrors(
            Supplier supplier,
            SupplierContact contact,
            DeleteContactModel model,
            [Frozen] ISuppliersService supplierService,
            DeleteContactModelValidator validator)
        {
            supplier.IsActive = false;
            supplier.SupplierContacts = new List<SupplierContact>(new[] { contact });

            supplierService.GetSupplier(model.SupplierId).Returns(supplier);

            var result = validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(m => m);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_WhenActiveSupplierHasASingleContact_ValidationErrors(
            Supplier supplier,
            SupplierContact contact,
            DeleteContactModel model,
            [Frozen] ISuppliersService supplierService,
            DeleteContactModelValidator validator)
        {
            supplier.IsActive = true;
            supplier.SupplierContacts = new List<SupplierContact>(new[] { contact });

            supplierService.GetSupplier(model.SupplierId).Returns(supplier);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor("delete-contact")
                .WithErrorMessage(DeleteContactModelValidator.LastContactOnSupplierErrorMessage);
        }
    }
}
