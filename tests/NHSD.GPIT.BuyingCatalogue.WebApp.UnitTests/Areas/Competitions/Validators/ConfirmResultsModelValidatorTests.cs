using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators
{
    public static class ConfirmResultsModelValidatorTests
    {
        [Theory]
        [MockAutoData]
        public static void Validate_ValuesMissing_ThrowsValidationError(
            ConfirmResultsModel model,
            ConfirmResultsModelValidator systemUnderTest)
        {
            model.CompleteCompetition = false;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.CompleteCompetition)
                .WithErrorMessage(ConfirmResultsModelValidator.CompleteCompetitionError);
        }

        [Theory]
        [MockAutoData]
        public static void Validate_ConfirmSolutionsValid_NoModelErrors(
            ConfirmResultsModel model,
            ConfirmResultsModelValidator systemUnderTest)
        {
            model.CompleteCompetition = true;

            var result = systemUnderTest.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
