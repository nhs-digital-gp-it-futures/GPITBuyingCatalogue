using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators.Scoring;

public class SolutionScoreModelValidator : AbstractValidator<SolutionScoreModel>
{
    internal const string NullScoreError = "Enter a score";
    internal const string RangeScoreError = "Score must be between 1 and 5";
    internal const string NullJustificationError = "Provide a justification for your score";

    public SolutionScoreModelValidator()
    {
        RuleFor(x => x.Score)
            .NotNull()
            .WithMessage(NullScoreError)
            .InclusiveBetween(1, 5)
            .WithMessage(RangeScoreError);

        RuleFor(x => x.Justification)
            .NotEmpty()
            .WithMessage(NullJustificationError);
    }
}
