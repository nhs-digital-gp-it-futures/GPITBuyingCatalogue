using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSource;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.FundingSource
{
    public sealed class FundingSourceModelValidator : AbstractValidator<FundingSourceModel>
    {
        public FundingSourceModelValidator()
        {
            RuleFor(m => m.SelectedFundingSource)
                .NotNull()
                .WithMessage("Select a funding source");
        }
    }
}
