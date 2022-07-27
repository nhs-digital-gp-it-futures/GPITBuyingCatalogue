using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.DesktopBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ClientApplicationType.DesktopBased
{
    public sealed class OperatingSystemsModelValidator : AbstractValidator<OperatingSystemsModel>
    {
        public const string EnterSupportedOperatingSystems = "Enter supported operating systems information";

        public OperatingSystemsModelValidator()
        {
            RuleFor(m => m.Description)
                .NotEmpty()
                .WithMessage(EnterSupportedOperatingSystems);
        }
    }
}
