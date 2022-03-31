using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices
{
    public class ListPriceTypeModelValidator : AbstractValidator<ListPriceTypeModel>
    {
        internal const string SelectedCataloguePriceTypeError = "Select a price type";

        public ListPriceTypeModelValidator()
        {
            RuleFor(m => m.SelectedCataloguePriceType)
                .NotNull()
                .WithMessage(SelectedCataloguePriceTypeError);
        }
    }
}
