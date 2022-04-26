using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.AssociatedServices
{
    public static class SelectAssociatedServicePriceModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_SelectedPriceNull_SetsModelError(
            SelectAssociatedServicePriceModel model,
            SelectAssociatedServicePriceModelValidator validator)
        {
            model.SelectedPrice = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedPrice)
                .WithErrorMessage("Select a price");
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelErrors(
            SelectAssociatedServicePriceModel model,
            SelectAssociatedServicePriceModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
