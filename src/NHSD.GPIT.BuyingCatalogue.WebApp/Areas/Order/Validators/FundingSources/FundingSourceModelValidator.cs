using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSources;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.FundingSources
{
    public sealed class FundingSourceModelValidator : AbstractValidator<FundingSource>
    {
        public const string FundingSourceMissingErrorMessage = "Select a funding source";

        public FundingSourceModelValidator()
        {
            RuleFor(fs => fs.SelectedFundingType)
                .NotEqual(EntityFramework.Ordering.Models.OrderItemFundingType.None)
                .WithMessage(FundingSourceMissingErrorMessage);
        }
    }
}
