using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Validators;

public class AddNonPriceElementModelValidator : AbstractValidator<AddNonPriceElementModel>
{
    internal const string NoSelectionError = "Select a non-price element";

    public AddNonPriceElementModelValidator()
    {
        RuleFor(x => x.AvailableNonPriceElements)
            .Must(x => x.Any(y => y.Selected))
            .WithMessage(NoSelectionError)
            .OverridePropertyName("AvailableNonPriceElements[0].Selected");
    }
}
