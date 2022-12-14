using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Prices;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.SolutionSelection.Prices
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
