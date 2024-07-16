using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Validators.ListPrices
{
    public static class ListPriceTypeModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_NullSelection_SetsModelError(
            ListPriceTypeModel model,
            ListPriceTypeModelValidator validator)
        {
            model.SelectedCataloguePriceType = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedCataloguePriceType)
                .WithErrorMessage(ListPriceTypeModelValidator.SelectedCataloguePriceTypeError);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_ValidSection_NoModelError(
            ListPriceTypeModel model,
            ListPriceTypeModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
