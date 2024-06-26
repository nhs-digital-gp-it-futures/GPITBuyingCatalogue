using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public class SelectInteroperabilityCriteriaModelValidator : AbstractValidator<SelectInteroperabilityCriteriaModel>
{
    internal const string MissingSelectionError = "Select an integration";

    public SelectInteroperabilityCriteriaModelValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Integrations.SelectMany(y => y.Value).Any(y => y.Selected))
            .WithMessage(MissingSelectionError);
    }
}
