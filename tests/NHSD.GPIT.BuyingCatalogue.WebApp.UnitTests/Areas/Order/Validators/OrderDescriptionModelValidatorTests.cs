using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderDescription;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators
{
    public static class OrderDescriptionModelValidatorTests
    {
        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        public static void Validate_DescriptionNullOrEmpty_SetsModelError(
            string description,
            OrderDescriptionModel model,
            OrderDescriptionModelValidator validator)
        {
            model.Description = description;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Description)
                .WithErrorMessage(OrderDescriptionModelValidator.OrderDescriptionErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            OrderDescriptionModel model,
            OrderDescriptionModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
