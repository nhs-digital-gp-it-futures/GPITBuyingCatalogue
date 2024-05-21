using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public class SelectFilterModelValidator : AbstractValidator<SelectFilterModel>
{
    internal const string SelectFilterError = "Select a shortlist";

    public SelectFilterModelValidator()
    {
        RuleFor(x => x.SelectedFilterId)
            .NotNull()
            .WithMessage(SelectFilterError);
    }
}
