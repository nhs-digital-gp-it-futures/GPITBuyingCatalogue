using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.BrowserBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class BrowserBasedModelValidator : AbstractValidator<BrowserBasedModel>
    {
        public const string MandatoryRequiredMessage = "Mandatory information required";

        public BrowserBasedModelValidator()
        {
            RuleFor(m => m.ApplicationTypeProgress.SupportedBrowsersStatus() == TaskProgress.Completed && m.ApplicationTypeProgress.PluginsStatus() == TaskProgress.Completed)
                .NotEmpty()
                .WithMessage(MandatoryRequiredMessage);
        }
    }
}
