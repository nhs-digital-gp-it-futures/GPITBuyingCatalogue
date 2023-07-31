using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public class SolutionScoreModelValidator : AbstractValidator<SolutionScoreModel>
{
    internal const string NullScoreError = "Enter a score";
    internal const string RangeScoreError = "Score must be between 1 and 5";

    public SolutionScoreModelValidator()
    {
        RuleFor(x => x.Score)
            .NotNull()
            .WithMessage(NullScoreError)
            .InclusiveBetween(1, 5)
            .WithMessage(RangeScoreError);
    }
}
