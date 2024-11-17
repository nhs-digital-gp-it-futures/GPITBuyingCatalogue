using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators
{
    public static class CompetitionReviewCriteriaModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_ValuesMissing_ThrowsValidationError(
            CompetitionReviewCriteriaModel model,
            CompetitionReviewCriteriaModelValidator systemUnderTest)
        {
            model.ConfirmCriteria = false;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.ConfirmCriteria)
                .WithErrorMessage(CompetitionReviewCriteriaModelValidator.ConfirmCriteriaError);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_ConfirmSolutionsValid_NoModelErrors(
            CompetitionReviewCriteriaModel model,
            CompetitionReviewCriteriaModelValidator systemUnderTest)
        {
            model.ConfirmCriteria = true;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
