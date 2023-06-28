using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.DesktopBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ApplicationType.DesktopBased
{
    public sealed class ConnectivityModelValidator : AbstractValidator<ConnectivityModel>
    {
        public ConnectivityModelValidator()
        {
            RuleFor(m => m.SelectedConnectionSpeed)
                .NotEmpty()
                .WithMessage("Select a connection speed");
        }
    }
}
