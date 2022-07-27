using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.BrowserBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ClientApplicationType.BrowserBased
{
    public sealed class PlugInsOrExtensionsModelValidator : AbstractValidator<PlugInsOrExtensionsModel>
    {
        public const string PluginsOrExtensionsRequiredError = "Select yes if any plug-ins or extensions are required";

        public PlugInsOrExtensionsModelValidator()
        {
            RuleFor(m => m.PlugInsRequired)
                .NotEmpty()
                .WithMessage(PluginsOrExtensionsRequiredError);
        }
    }
}
