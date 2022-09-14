using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Capabilities;

public sealed class CapabilityModelValidator : AbstractValidator<CapabilityModel>
{
    public const string SelectMustEpicError = "Select a Must Epic";

    public CapabilityModelValidator()
    {
        RuleFor(c => c.MustEpics)
            .Must(c => c.Any(e => e.Selected))
            .When(c => c.Selected && c.MustEpics.Any())
            .WithMessage(SelectMustEpicError)
            .OverridePropertyName(string.Empty);
    }
}
