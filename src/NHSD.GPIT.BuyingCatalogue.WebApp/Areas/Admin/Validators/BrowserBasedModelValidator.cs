using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BrowserBasedModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators
{
    public sealed class BrowserBasedModelValidator : AbstractValidator<BrowserBasedModel>
    {
        public const string MandatoryRequiredMessage = "Mandatory information required";

        public BrowserBasedModelValidator()
        {
            RuleFor(m => m.IsComplete)
                .NotEmpty()
                .WithMessage(MandatoryRequiredMessage);
        }
    }
}
