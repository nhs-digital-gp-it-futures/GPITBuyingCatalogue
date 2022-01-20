using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators
{
    public static class SupplierSearchSelectModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_SelectedSupplierNull_SetsModelError(
            SupplierSearchSelectModel model,
            SupplierSearchSelectModelValidator validator)
        {
            model.SelectedSupplierId = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedSupplierId)
                .WithErrorMessage("Please select a supplier");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            SupplierSearchSelectModel model,
            SupplierSearchSelectModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
