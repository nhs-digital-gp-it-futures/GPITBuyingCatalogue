using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.BrowserBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ClientApplicationType.BrowserBased
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
