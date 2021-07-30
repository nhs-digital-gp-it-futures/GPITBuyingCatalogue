using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BrowserBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class SupportedBrowsersModelValidator : AbstractValidator<SupportedBrowsersModel>
    {
        public const string MandatoryRequiredMessage = "Select at least one browser";

        public SupportedBrowsersModelValidator()
        {
            RuleFor(m => m.Browsers)
                .Must(b => b.Any(c => c.Checked == true))
                .WithMessage(MandatoryRequiredMessage);
        }
    }
}
