using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.Solutions
{
    public class SelectFlatOnDemandQuantityModelValidator : AbstractValidator<SelectFlatOnDemandQuantityModel>
    {
        public SelectFlatOnDemandQuantityModelValidator()
        {
            RuleFor(m => m.Quantity)
                .IsNumericAndNonZero("quantity");

            RuleFor(m => m.EstimationPeriod)
                .NotNull()
                .WithMessage("Time Unit is required");
        }
    }
}
