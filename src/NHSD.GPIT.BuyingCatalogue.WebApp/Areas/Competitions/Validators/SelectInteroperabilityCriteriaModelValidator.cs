using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public class SelectInteroperabilityCriteriaModelValidator : AbstractValidator<SelectInteroperabilityCriteriaModel>
{
    internal const string PropertyName = "Im1Integrations[0].Selected|GpConnectIntegrations[0].Selected";
    internal const string MissingSelectionError = "Select an integration";

    public SelectInteroperabilityCriteriaModelValidator()
    {
        RuleFor(x => x)
            .Must(x => x.Im1Integrations.Any(y => y.Selected) || x.GpConnectIntegrations.Any(y => y.Selected))
            .WithMessage(MissingSelectionError)
            .OverridePropertyName(PropertyName);
    }
}
