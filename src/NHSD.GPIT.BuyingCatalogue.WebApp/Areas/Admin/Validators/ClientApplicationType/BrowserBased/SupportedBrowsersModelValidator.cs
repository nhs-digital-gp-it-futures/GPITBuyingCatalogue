using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.BrowserBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ClientApplicationType.BrowserBased
{
    public sealed class SupportedBrowsersModelValidator : AbstractValidator<SupportedBrowsersModel>
    {
        public const string MandatoryRequiredMessage = "Select at least one browser type";
        public const string SolutionMobileResponsiveError = "Select yes if your Catalogue Solution is mobile responsive";

        public SupportedBrowsersModelValidator()
        {
            RuleFor(m => m.MobileResponsive)
                .NotEmpty()
                .WithMessage(SolutionMobileResponsiveError);

            RuleFor(m => m.Browsers)
                .Must(b => b.Any(c => c.Checked == true))
                .WithMessage(MandatoryRequiredMessage)
                .OverridePropertyName("Browsers[0].Checked");
        }
    }
}
