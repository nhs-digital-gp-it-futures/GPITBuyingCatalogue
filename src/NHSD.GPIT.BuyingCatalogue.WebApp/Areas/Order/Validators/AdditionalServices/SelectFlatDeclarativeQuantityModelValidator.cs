using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AdditionalServices
{
    public class SelectFlatDeclarativeQuantityModelValidator : AbstractValidator<SelectFlatDeclarativeQuantityModel>
    {
        public SelectFlatDeclarativeQuantityModelValidator()
        {
            RuleFor(m => m.Quantity)
                .IsValidQuantity();
        }
    }
}
