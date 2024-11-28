using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public sealed class CompetitionReviewCriteriaModelValidator : AbstractValidator<CompetitionReviewCriteriaModel>
{
    internal const string ConfirmCriteriaError = "Select if you want to proceed with this competition criteria";

    public CompetitionReviewCriteriaModelValidator()
    {
        RuleFor(x => x.ConfirmCriteria)
            .Equal(true)
            .Unless(x => x.HasReviewedCriteria)
            .WithMessage(ConfirmCriteriaError);
    }
}
