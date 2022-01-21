using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Solutions
{
    public sealed class SelectSolutionPriceModelValidator : AbstractValidator<SelectSolutionPriceModel>
    {
        public SelectSolutionPriceModelValidator()
        {
            RuleFor(m => m.SelectedPrice)
                .NotNull()
                .WithMessage("Select a price");
        }
    }
}
