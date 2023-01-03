using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Validators.FundingSources
{
    public sealed class FundingSourceModelValidator : AbstractValidator<FundingSource>
    {
        public const string FundingSourceMissingErrorMessage = "Select a funding source";

        public FundingSourceModelValidator()
        {
            RuleFor(fs => fs.SelectedFundingType)
                .NotEqual(OrderItemFundingType.None)
                .WithMessage(FundingSourceMissingErrorMessage);
        }
    }
}
