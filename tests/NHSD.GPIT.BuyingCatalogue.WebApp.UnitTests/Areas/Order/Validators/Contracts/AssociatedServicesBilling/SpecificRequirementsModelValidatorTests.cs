using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.AssociatedServicesBilling;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.AssociatedServicesBilling;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.AssociatedServicesBilling
{
    public static class SpecificRequirementsModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_HasSpecificRequirementsNull_SetsModelError(
            SpecificRequirementsModel model,
            SpecificRequirmentsModelValidator validator)
        {
            model.ProceedWithoutSpecificRequirements = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.ProceedWithoutSpecificRequirements)
                .WithErrorMessage(SpecificRequirmentsModelValidator.NoSelectionErrorMessage);
        }

        [Theory]
        [CommonInlineAutoData(true)]
        [CommonInlineAutoData(false)]
        public static void Validate_Valid_NoModelError(
            bool hasSpecificRequirements,
            SpecificRequirementsModel model,
            SpecificRequirmentsModelValidator validator)
        {
            model.ProceedWithoutSpecificRequirements = hasSpecificRequirements;

            var result = validator
                .TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
