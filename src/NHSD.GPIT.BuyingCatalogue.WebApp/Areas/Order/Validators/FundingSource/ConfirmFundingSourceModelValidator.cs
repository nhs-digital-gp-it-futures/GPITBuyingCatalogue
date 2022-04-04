using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSource;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.FundingSource
{
    public sealed class ConfirmFundingSourceModelValidator : AbstractValidator<ConfirmFundingSourceModel>
    {
        public ConfirmFundingSourceModelValidator()
        {
            Include(new FundingSourceModelValidator());
        }
    }
}
