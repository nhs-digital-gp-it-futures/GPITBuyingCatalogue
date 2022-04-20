using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Validation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices
{
    public class AddEditTieredPriceTierModelValidator : AbstractValidator<AddEditTieredPriceTierModel>
    {
        internal const string LowerRangeMissing = "Enter a lower range";
        internal const string UpperRangeMissing = "Enter an upper range";
        internal const string RangeTypeMissing = "Select how you want to define the upper range";
        internal const string DuplicateListPriceTierError = "A tier with these details already exists";

        private readonly IListPriceService listPriceService;

        public AddEditTieredPriceTierModelValidator(IListPriceService listPriceService)
        {
            this.listPriceService = listPriceService;

            RuleFor(m => m.Price)
                .IsValidPrice();

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

        private async Task<bool> NotBeADuplicate(AddEditTieredPriceTierModel model, CancellationToken token)
        {
            _ = token;

            return !await listPriceService.HasDuplicatePriceTier(
                model.CatalogueItemId,
                model.CataloguePriceId,
                model.TierId,
                model.LowerRange!.Value,
                model.UpperRange);
        }
    }
}
