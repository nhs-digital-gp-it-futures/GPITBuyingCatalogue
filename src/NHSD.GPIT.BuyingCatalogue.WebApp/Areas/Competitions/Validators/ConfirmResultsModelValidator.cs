using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public sealed class ConfirmResultsModelValidator : AbstractValidator<ConfirmResultsModel>
{
    internal const string CompleteCompetitionError = "Select if you want to confirm this competition";

    public ConfirmResultsModelValidator()
    {
        RuleFor(x => x.CompleteCompetition)
            .Equal(true)
            .WithMessage(CompleteCompetitionError);
    }
}
