using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSource;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators
{
    public sealed class FundingSourceModelValidator : AbstractValidator<FundingSourceModel>
    {
        public FundingSourceModelValidator()
        {
            RuleFor(m => m.FundingSourceOnlyGms)
                .NotEmpty()
                .WithMessage("Select yes if you're paying for this order in full using your GP IT Futures centrally held funding allocation");
        }
    }
}
