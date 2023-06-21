using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.BrowserBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class BrowserBasedModelValidator : AbstractValidator<BrowserBasedModel>
    {
        public const string MandatoryRequiredMessage = "Mandatory information required";

        public BrowserBasedModelValidator()
        {
            RuleFor(m => m.ClientApplicationProgress.SupportedBrowsersStatus() == TaskProgress.Completed && m.ClientApplicationProgress.PluginsStatus() == TaskProgress.Completed)
                .NotEmpty()
                .WithMessage(MandatoryRequiredMessage);
        }
    }
}
