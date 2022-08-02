using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Contracts.Validators.AssociatedServicesBilling;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.AssociatedServicesBilling;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.AssociatedServicesBilling
{
    public static class ReviewBillingModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_UseDefaultBillingNull_SetsModelError(
            ReviewBillingModel model,
            ReviewBillingModelValidator validator)
        {
            model.UseDefaultBilling = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.UseDefaultBilling)
                .WithErrorMessage(ReviewBillingModelValidator.NoSelectionErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(true)]
        [CommonInlineAutoData(false)]
        public static void Validate_Valid_NoModelError(
            bool useDefaultBilling,
            ReviewBillingModel model,
            ReviewBillingModelValidator validator)
        {
            model.UseDefaultBilling = useDefaultBilling;

            var result = validator
                .TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
