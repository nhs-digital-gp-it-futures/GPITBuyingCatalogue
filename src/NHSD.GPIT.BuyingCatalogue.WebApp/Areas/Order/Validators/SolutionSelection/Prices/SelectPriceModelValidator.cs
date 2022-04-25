using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Prices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.SolutionSelection.Prices
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
