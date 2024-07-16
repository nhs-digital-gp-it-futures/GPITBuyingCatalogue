using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.Requirement;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.Contracts.Requirements;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Validators.Contracts.Requirement
{
    public static class RequirementDetailsModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_Requirement_DetailsNull_SetsModelError(
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
        public static void Validate_Requirement_SelectedOrderItemIdNull_SetsModelError(
            RequirementDetailsModel model,
            RequirementDetailsModelValidator validator)
        {
            model.SelectedOrderItemId = default(CatalogueItemId);

            var result = validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(m => m.SelectedOrderItemId)
                .WithErrorMessage(RequirementDetailsModelValidator.AssociatedServiceRequiredErrorMessage);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_Valid_NoModelError(
            RequirementDetailsModel model,
            RequirementDetailsModelValidator validator)
        {
            var result = validator.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
