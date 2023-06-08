using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ImplementationPlans;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.ImplementationPlans;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.ImplementationPlans
{
    public static class MilestoneModelValidatorTests
    {
        [Theory]
        [CommonAutoData]
        public static void Validate_Milestone_NameNull_SetsModelError(
            MilestoneModel model,
            MilestoneModelValidator validator)
        {
            model.PaymentTrigger = "test";
            model.Name = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Name)
                .WithErrorMessage(MilestoneModelValidator.NameRequiredErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Order_SelectedOptionsNull_SetsModelError(
            MilestoneModel model,
            MilestoneModelValidator validator)
        {
            model.PaymentTrigger = null;
            model.Name = "test";

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.PaymentTrigger)
                .WithErrorMessage(MilestoneModelValidator.PaymentTriggerRequiredErrorMessage);
        }

        [Theory]
        [CommonAutoData]
        public static void Validate_Valid_NoModelError(
            MilestoneModel model,
            MilestoneModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
