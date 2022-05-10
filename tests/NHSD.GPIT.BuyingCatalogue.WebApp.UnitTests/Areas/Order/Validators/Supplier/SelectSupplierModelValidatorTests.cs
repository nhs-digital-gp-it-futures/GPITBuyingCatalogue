using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Supplier
{
    public static class SelectSupplierModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void Validate_ValuesMissing_ThrowsValidationError(
            string supplierId,
            SelectSupplierModel model,
            SelectSupplierModelValidator systemUnderTest)
        {
            model.SelectedSupplierId = supplierId;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.SelectedSupplierId)
                .WithErrorMessage(SelectSupplierModelValidator.SupplierMissingErrorMessage);
        }
    }
}
