using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class EditCapabilitiesModelValidator : AbstractValidator<EditCapabilitiesModel>
    {
        public EditCapabilitiesModelValidator()
        {
            RuleFor(m => m.CapabilityCategories)
                .Must(capability => capability.Any(x => x.Capabilities.Any(y => y.Selected)))
                .WithMessage("Select a Capability")
                .OverridePropertyName("edit-capabilities");
        }
    }
}
