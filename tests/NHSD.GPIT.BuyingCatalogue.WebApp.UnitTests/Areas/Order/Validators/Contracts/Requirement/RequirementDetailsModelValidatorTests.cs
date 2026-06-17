using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.Requirement;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.Requirements;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.Requirement
{
    public static class RequirementDetailsModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_DetailsNull_SetsModelError(
            RequirementDetailsModel model,
            RequirementDetailsModelValidator validator)
        {
            model.Details = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.Details)
                .WithErrorMessage(RequirementDetailsModelValidator.DetailsRequiredErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_SelectedOrderItemIdNull_SetsModelError(
            RequirementDetailsModel model,
            RequirementDetailsModelValidator validator)
        {
            model.SelectedOrderItemId = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedOrderItemId)
                .WithErrorMessage(RequirementDetailsModelValidator.AssociatedServiceRequiredErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_ExplanationRequiredNull_SetsModelError(
            RequirementDetailsModel model,
            RequirementDetailsModelValidator validator)
        {
            model.RequiresExplanation = null;

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.RequiresExplanation)
                .WithErrorMessage(RequirementDetailsModelValidator.ExplanationRequiredErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Valid_NoModelError(
            int orderItemId,
            string details,
            RequirementDetailsModel model,
            RequirementDetailsModelValidator validator)
        {
            model.SelectedOrderItemId = orderItemId;
            model.Details = details;
            model.RequiresExplanation = true;

            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
