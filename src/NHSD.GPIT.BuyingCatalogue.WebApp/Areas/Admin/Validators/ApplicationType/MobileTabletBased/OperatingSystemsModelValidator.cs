using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.MobileTabletBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ApplicationType.MobileTabletBased
{
    public class OperatingSystemsModelValidator : AbstractValidator<OperatingSystemsModel>
    {
        public OperatingSystemsModelValidator()
        {
            RuleFor(m => m.OperatingSystems)
                .Must(systems => systems.Any(os => os.Checked))
                .WithMessage("Select at least one supported operating system")
                .OverridePropertyName($"OperatingSystems[0].Checked");
        }
    }
}
