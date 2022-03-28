using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSources;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Validators.FundingSources
{
    public sealed class FundingSourceModelValidator : AbstractValidator<FundingSource>
    {
        public const string FundingSourceMissingErrorMessage = "Select a funding source";
        public const string FundingSourceCentrallyAllocatedAmountMissingErrorMessage = "Enter an amount for how much of your central funding you’re using";
        public const string FundingSourceAmountMustBeANumberErrorMessage = "Amount must be a number";
        public const string FundingSourceAmountMustBeGreaterThanZeroErrorMessage = "Amount must be more than zero";
        public const string FundingSourceAmountTo4DeciamlPlacesErrorMessage = "Amount must be to a maximum of 4 decimal places";
        public const string FundingSourceAmountCannotExceedeTotalCost = "Amount must be less than the total cost of the solution or service";

        public FundingSourceModelValidator()
        {
            RuleFor(fs => fs.SelectedFundingType)
                .NotEqual(EntityFramework.Ordering.Models.OrderItemFundingType.None)
                .WithMessage(FundingSourceMissingErrorMessage);

            RuleFor(fs => fs.AmountOfCentralFunding)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .When(fs => fs.SelectedFundingType == EntityFramework.Ordering.Models.OrderItemFundingType.MixedFunding)
                .WithMessage(FundingSourceCentrallyAllocatedAmountMissingErrorMessage)
                .GreaterThan(0)
                .When(fs => fs.SelectedFundingType == EntityFramework.Ordering.Models.OrderItemFundingType.MixedFunding)
                .WithMessage(FundingSourceAmountMustBeGreaterThanZeroErrorMessage)
                .ScalePrecision(4, 18)
                .When(fs => fs.SelectedFundingType == EntityFramework.Ordering.Models.OrderItemFundingType.MixedFunding)
                .WithMessage(FundingSourceAmountTo4DeciamlPlacesErrorMessage)
                .LessThan(fs => fs.TotalCost)
                .When(fs => fs.SelectedFundingType == EntityFramework.Ordering.Models.OrderItemFundingType.MixedFunding)
                .WithMessage(FundingSourceAmountCannotExceedeTotalCost);
        }
    }
}
