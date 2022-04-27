using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.AdditionalServices
{
    public static class SelectAdditionalServicePriceModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_SelectedPriceNull_SetsModelError(
            SelectAdditionalServicePriceModel model,
            SelectAdditionalServicePriceModelValidator validator)
        {
            model.SelectedPrice = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedPrice)
                .WithErrorMessage("Select a price");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelErrors(
            SelectAdditionalServicePriceModel model,
            SelectAdditionalServicePriceModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
