using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Validation.Pricing
{
    public class SelectPriceModelValidator : AbstractValidator<SelectPriceModel>
    {
        public const string NoSelectionMadeErrorMessage = "Select a price";

        public SelectPriceModelValidator()
        {
            RuleFor(sp => sp.SelectedPriceId)
                .NotNull()
                .WithMessage(NoSelectionMadeErrorMessage);
        }
    }
}
