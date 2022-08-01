using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Suppliers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.Suppliers
{
    public static class EditSupplierAddressModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_AddressLine1NullOrEmpty_SetsModelError(
            string addressLine1,
            EditSupplierAddressModelValidator validator)
        {
            var model = new EditSupplierAddressModel
            {
                AddressLine1 = addressLine1,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.AddressLine1)
                .WithErrorMessage(EditSupplierAddressModelValidator.AddressLine1Error);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_TownNullOrEmpty_SetsModelError(
            string town,
            EditSupplierAddressModelValidator validator)
        {
            var model = new EditSupplierAddressModel
            {
                Town = town,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Town)
                .WithErrorMessage(EditSupplierAddressModelValidator.TownOrCityError);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_PostCodeNullOrEmpty_SetsModelError(
            string postCode,
            EditSupplierAddressModelValidator validator)
        {
            var model = new EditSupplierAddressModel
            {
                PostCode = postCode,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.PostCode)
                .WithErrorMessage(EditSupplierAddressModelValidator.PostcodeError);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_CountryNullOrEmpty_SetsModelError(
            string country,
            EditSupplierAddressModelValidator validator)
        {
            var model = new EditSupplierAddressModel
            {
                Country = country,
            };

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Country)
                .WithErrorMessage(EditSupplierAddressModelValidator.CountryError);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelErrors(
            EditSupplierAddressModel model,
            EditSupplierAddressModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
