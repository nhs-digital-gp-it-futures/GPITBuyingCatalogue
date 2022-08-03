using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Contracts.ImplementationPlans;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Contracts.ImplementationPlans;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.ImplementationPlans
{
    public static class DefaultImplementationPlanModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_UseDefaultMilestonesIsNull_ThrowsValidationError(
            DefaultImplementationPlanModel model,
            DefaultImplementationPlanModelValidator validator)
        {
            model.UseDefaultMilestones = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.UseDefaultMilestones)
                .WithErrorMessage(DefaultImplementationPlanModelValidator.NoSelectionErrorMessage);
        }
    }
}
