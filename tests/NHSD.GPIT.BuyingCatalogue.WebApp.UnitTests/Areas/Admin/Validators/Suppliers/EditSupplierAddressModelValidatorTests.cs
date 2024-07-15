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
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
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
                .WithErrorMessage("Enter a building or street");
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
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
                .WithErrorMessage("Enter a town or city");
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
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
                .WithErrorMessage("Enter a postcode");
        }

        [Theory]
        [MockInlineAutoData(null)]
        [MockInlineAutoData("")]
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
                .WithErrorMessage("Enter a country");
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Valid_NoModelErrors(
            EditSupplierAddressModel model,
            EditSupplierAddressModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
