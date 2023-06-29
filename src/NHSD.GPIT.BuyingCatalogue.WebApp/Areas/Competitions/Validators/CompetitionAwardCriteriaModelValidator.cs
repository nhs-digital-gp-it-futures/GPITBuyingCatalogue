using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public class CompetitionAwardCriteriaModelValidator : AbstractValidator<CompetitionAwardCriteriaModel>
{
    internal const string AwardCriteriaMissingError = "Select an award criteria";

    public CompetitionAwardCriteriaModelValidator()
    {
        RuleFor(x => x.IncludesNonPrice)
            .NotNull()
            .WithMessage(AwardCriteriaMissingError);
    }
}
