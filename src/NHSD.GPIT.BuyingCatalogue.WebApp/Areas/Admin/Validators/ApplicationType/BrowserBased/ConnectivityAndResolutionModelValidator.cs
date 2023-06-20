using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.BrowserBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ApplicationType.BrowserBased
{
    public sealed class ConnectivityAndResolutionModelValidator : AbstractValidator<ConnectivityAndResolutionModel>
    {
        public ConnectivityAndResolutionModelValidator()
        {
            RuleFor(m => m.SelectedConnectionSpeed)
                .NotEmpty()
                .WithMessage("Select a connection speed");
        }
    }
}
