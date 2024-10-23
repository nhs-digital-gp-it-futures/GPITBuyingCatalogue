using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public sealed class ConfirmSolutionsModelValidator : AbstractValidator<ConfirmSolutionsModel>
{
    internal const string ConfirmShortlistError = "Confirm you want to continue with this shortlist";

    public ConfirmSolutionsModelValidator()
    {
        RuleFor(x => x.ConfirmShortlist)
            .Equal(true)
            .WithMessage(ConfirmShortlistError);
    }
}
