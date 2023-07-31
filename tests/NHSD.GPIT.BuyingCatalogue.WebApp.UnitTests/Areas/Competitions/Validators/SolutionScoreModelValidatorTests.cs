using FluentValidation.TestHelper;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Validators;

public static class SolutionScoreModelValidatorTests
{
    [Theory]
    [CommonAutoData]
    public static void Validate_NullScore_SetsModelError(
        SolutionScoreModel model,
        SolutionScoreModelValidator validator)
    {
        model.Score = null;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Score)
            .WithErrorMessage(SolutionScoreModelValidator.NullScoreError);
    }

    [Theory]
    [CommonInlineAutoData(0)]
    [CommonInlineAutoData(6)]
    public static void Validate_ScoreOutOfRange_SetsModelError(
        int score,
        SolutionScoreModel model,
        SolutionScoreModelValidator validator)
    {
        model.Score = score;

        var result = validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Score)
            .WithErrorMessage(SolutionScoreModelValidator.RangeScoreError);
    }

    [Theory]
    [CommonAutoData]
    public static void Validate_NullScore_NoModelErrors(
        SolutionScoreModel model,
        SolutionScoreModelValidator validator)
    {
        model.Score = 3;

        var result = validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
