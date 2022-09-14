using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.Capabilities;

public sealed class CapabilityCategoryModelValidator : AbstractValidator<CapabilityCategoryModel>
{
    public CapabilityCategoryModelValidator()
    {
        RuleForEach(m => m.Capabilities)
            .OverrideIndexer((_, _, _, _) => string.Empty)
            .SetValidator(new CapabilityModelValidator())
            .OverridePropertyName(string.Empty);
    }
}
