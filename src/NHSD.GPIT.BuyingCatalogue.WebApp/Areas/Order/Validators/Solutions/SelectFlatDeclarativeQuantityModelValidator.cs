using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Solutions
{
    public class SelectFlatDeclarativeQuantityModelValidator : AbstractValidator<SelectFlatDeclarativeQuantityModel>
    {
        public SelectFlatDeclarativeQuantityModelValidator()
        {
            RuleFor(m => m.Quantity)
                .IsNumericAndNonZero("quantity");
        }
    }
}
