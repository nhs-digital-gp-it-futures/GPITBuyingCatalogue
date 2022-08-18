using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Capabilities;

public sealed class EditCapabilitiesModelValidator : AbstractValidator<EditCapabilitiesModel>
{
    public EditCapabilitiesModelValidator()
    {
        RuleFor(m => m.CapabilityCategories)
            .Must(capability => capability.Any(c => c.Capabilities.Any(y => y.Selected)))
            .WithMessage("Select a Capability")
            .OverridePropertyName("edit-capabilities");

        RuleForEach(m => m.CapabilityCategories)
            .OverrideIndexer((_, _, _, index) => index.ToString())
            .SetValidator(new CapabilityCategoryModelValidator())
            .OverridePropertyName("capability-categories-");
    }
}
