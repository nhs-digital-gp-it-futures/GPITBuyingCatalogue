using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public sealed class SelectSolutionsModelValidator : AbstractValidator<SelectSolutionsModel>
{
    internal const string DirectAwardSelectionMissingError = "Select yes if you want to use a direct award";
    internal const string NotEnoughSelectionsError = "Select at least 2 Catalogue Solutions for your shortlist";
    internal const string TooManySelectionsError = "You’ve selected more than 8 Catalogue Solutions";

    internal const string SelectedSolutionsPropertyName = "Solutions[0].Selected";

    public SelectSolutionsModelValidator()
    {
        When(
            x => x.HasSingleSolution(),
            () => RuleFor(x => x.IsDirectAward)
            .NotNull()
            .WithMessage(DirectAwardSelectionMissingError))
        .Otherwise(
            () => RuleFor(x => x.Solutions)
            .Must(x => x.Count(y => y.Selected) >= 2)
            .WithMessage(NotEnoughSelectionsError)
            .Must(x => x.Count(y => y.Selected) <= 8)
            .WithMessage(TooManySelectionsError)
            .OverridePropertyName(SelectedSolutionsPropertyName));
    }
}
