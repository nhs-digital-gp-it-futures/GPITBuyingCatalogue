using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.AdditionalServices
{
    public class SelectFlatOnDemandQuantityModelValidator : AbstractValidator<SelectFlatOnDemandQuantityModel>
    {
        public SelectFlatOnDemandQuantityModelValidator()
        {
            RuleFor(m => m.EstimationPeriod)
                .NotNull()
                .WithMessage("Time Unit is Required");

            RuleFor(m => m.Quantity)
                .IsNumericAndNonZero("quantity");
        }
    }
}
