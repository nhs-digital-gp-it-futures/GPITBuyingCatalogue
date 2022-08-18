using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Capabilities;

public sealed class EditCapabilitiesModelValidator : AbstractValidator<EditCapabilitiesModel>
{
    public const string CapabilitiesCategoryPropertyName = "capability-categories-";
    public const string EditCapabilitiesPropertyName = "edit-capabilities";
    public const string NoSelectedCapabilitiesError = "Select a Capability";

    public EditCapabilitiesModelValidator()
    {
        RuleFor(m => m.CapabilityCategories)
            .Must(capability => capability.Any(c => c.Capabilities.Any(y => y.Selected)))
            .WithMessage(NoSelectedCapabilitiesError)
            .OverridePropertyName(EditCapabilitiesPropertyName);

        RuleForEach(m => m.CapabilityCategories)
            .OverrideIndexer((_, _, _, index) => index.ToString())
            .SetValidator(new CapabilityCategoryModelValidator())
            .OverridePropertyName(CapabilitiesCategoryPropertyName);
    }
}
