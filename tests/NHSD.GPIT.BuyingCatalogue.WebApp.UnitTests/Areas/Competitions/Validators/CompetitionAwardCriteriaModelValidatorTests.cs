using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators;

public static class CompetitionAwardCriteriaModelValidatorTests
{
    [Theory]
    [MockAutoData]
    public static void Validate_NoCriteriaSelection_SetsModelError(
        CompetitionAwardCriteriaModel model,
        CompetitionAwardCriteriaModelValidator validator)
    {
        model.IncludesNonPrice = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.IncludesNonPrice)
            .WithErrorMessage(CompetitionAwardCriteriaModelValidator.AwardCriteriaMissingError);
    }

    [Theory]
    [MockAutoData]
    public static void Validate_Valid_NoModelErrors(
        CompetitionAwardCriteriaModel model,
        CompetitionAwardCriteriaModelValidator validator)
    {
        model.IncludesNonPrice = false;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
