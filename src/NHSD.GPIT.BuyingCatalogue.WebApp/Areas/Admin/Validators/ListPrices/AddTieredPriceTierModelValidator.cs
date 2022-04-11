using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices
{
    public class AddTieredPriceTierModelValidator : AbstractValidator<AddTieredPriceTierModel>
    {
        internal const string PriceEmptyError = "Enter a price";
        internal const string PriceNegativeError = "Price cannot be negative";
        internal const string PriceGreaterThanDecimalPlacesError = "Price must be to a maximum of 4 decimal places";
        internal const string LowerRangeMissing = "Enter a lower range";
        internal const string UpperRangeMissing = "Enter an upper range";
        internal const string RangeTypeMissing = "Select how you want to define the upper range";
        internal const string DuplicateListPriceTierError = "A tier with these details already exists";

        private readonly IDuplicateListPriceService duplicateListPriceService;

        public AddTieredPriceTierModelValidator(IDuplicateListPriceService duplicateListPriceService)
        {
            this.duplicateListPriceService = duplicateListPriceService;

            RuleFor(m => m.Price)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(PriceEmptyError)
                .GreaterThanOrEqualTo(0)
                .WithMessage(PriceNegativeError)
                .Must(p => Regex.IsMatch(p.ToString(), @"^\d+.?\d{0,4}$"))
                .WithMessage(PriceGreaterThanDecimalPlacesError);

            RuleFor(m => m.LowerRange)
                .NotNull()
                .WithMessage(LowerRangeMissing);

            RuleFor(m => m.UpperRange)
                .NotNull()
                .WithMessage(UpperRangeMissing)
                .When(m => m.IsInfiniteRange.HasValue && m.IsInfiniteRange is false);

            RuleFor(m => m.IsInfiniteRange)
                .NotNull()
                .WithMessage(RangeTypeMissing);

            RuleFor(m => m)
                .MustAsync(NotBeADuplicate)
                .WithMessage(DuplicateListPriceTierError)
                .When(m => m.Price is not null && m.LowerRange is not null && m.IsInfiniteRange.HasValue && m.IsInfiniteRange is false)
                .OverridePropertyName(
                    m => m.Price,
                    m => m.LowerRange,
                    m => m.UpperRange,
                    m => m.IsInfiniteRange);

            RuleFor(m => m)
                .MustAsync(NotBeADuplicate)
                .WithMessage(DuplicateListPriceTierError)
                .When(m => m.Price is not null && m.LowerRange is not null && m.IsInfiniteRange.HasValue && m.IsInfiniteRange is true)
                .OverridePropertyName(
                    m => m.Price,
                    m => m.LowerRange,
                    m => m.IsInfiniteRange);
        }

        private async Task<bool> NotBeADuplicate(AddTieredPriceTierModel model, CancellationToken token)
        {
            _ = token;

            return !await duplicateListPriceService.HasDuplicatePriceTier(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.Price!.Value,
                model.LowerRange!.Value,
                model.UpperRange);
        }
    }
}
