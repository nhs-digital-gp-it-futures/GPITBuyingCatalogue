using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators.Results;

public class SelectWinningSolutionModelValidator : AbstractValidator<SelectWinningSolutionModel>
{
    internal const string SolutionNotSelectedError = "Select a winning solution";

    public SelectWinningSolutionModelValidator()
    {
        RuleFor(x => x.SolutionId)
            .NotNull()
            .WithMessage(SolutionNotSelectedError);
    }
}
