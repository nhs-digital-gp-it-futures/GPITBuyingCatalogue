using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public sealed class SelectSolutionsModelValidator : AbstractValidator<SelectSolutionsModel>
{
    public SelectSolutionsModelValidator()
    {
        When(
                x => x.HasSingleSolution(),
                () => RuleFor(x => x.IsDirectAward)
                    .NotNull()
                    .WithMessage("Select yes if you want to use a direct award"))
            .Otherwise(
                () => RuleFor(x => x.Solutions)
                    .Must(x => x.Count(y => y.Selected) >= 2)
                    .WithMessage("Select at least 2 Catalogue Solutions for your shortlist ")
                    .Must(x => x.Count(y => y.Selected) <= 8)
                    .WithMessage("You’ve selected more than 8 Catalogue Solutions")
                    .OverridePropertyName("Solutions[0].Selected"));
    }
}
