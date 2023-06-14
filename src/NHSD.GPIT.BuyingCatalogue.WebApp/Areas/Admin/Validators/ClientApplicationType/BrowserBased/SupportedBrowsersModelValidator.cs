using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.BrowserBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ApplicationType.BrowserBased
{
    public sealed class SupportedBrowsersModelValidator : AbstractValidator<SupportedBrowsersModel>
    {
        public const string MandatoryRequiredMessage = "Select at least one browser type";

        public SupportedBrowsersModelValidator()
        {
            RuleFor(m => m.MobileResponsive)
                .NotEmpty()
                .WithMessage("Select yes if your Catalogue Solution is mobile responsive");

            RuleFor(m => m.Browsers)
                .Must(b => b.Any(c => c.Checked == true))
                .WithMessage(MandatoryRequiredMessage)
                .OverridePropertyName("Browsers[0].Checked");
        }
    }
}
