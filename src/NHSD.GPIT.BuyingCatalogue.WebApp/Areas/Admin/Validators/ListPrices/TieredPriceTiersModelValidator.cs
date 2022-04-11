using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices
{
    public class TieredPriceTiersModelValidator : AbstractValidator<TieredPriceTiersModel>
    {
        internal const string MissingTiersError = "Add at least 1 tier";
        internal const string InvalidStartingRangeError = "Lowest tier must have a low range of 1";
        internal const string InvalidEndingRangeError = "Highest tier must have an infinite upper range";
        internal const string RangeOverlapError = "A tier’s lower range overlaps with another tier’s upper range";
        internal const string RangeGapError = "There’s a gap between a tier’s upper range and another tier’s lower range";

        private const int StartingLowerRange = 1;
        private readonly ISolutionListPriceService solutionListPriceService;

        public TieredPriceTiersModelValidator(ISolutionListPriceService solutionListPriceService)
        {
            this.solutionListPriceService = solutionListPriceService;

            RuleFor(m => m.SelectedPublicationStatus)
                .Cascade(CascadeMode.Stop)
                .MustAsync(HaveAtLeastOneTier)
                .WithMessage(MissingTiersError)
                .MustAsync(HaveTierWithStartingRange)
                .WithMessage(InvalidStartingRangeError)
                .MustAsync(HaveTierWithInfiniteRange)
                .WithMessage(InvalidEndingRangeError)
                .MustAsync(NotHaveRangeGaps)
                .WithMessage(RangeGapError)
                .MustAsync(NotHaveRangeOverlap)
                .WithMessage(RangeOverlapError)
                .When(m => m.SelectedPublicationStatus == PublicationStatus.Published);
        }

        private async Task<CataloguePrice> GetCataloguePrice(CatalogueItemId solutionId, int cataloguePriceId)
        {
            var solution = await solutionListPriceService.GetSolutionWithListPrices(solutionId);
            var price = solution.CataloguePrices.Single(p => p.CataloguePriceId == cataloguePriceId);
            return price;
        }

        private async Task<bool> HaveAtLeastOneTier(TieredPriceTiersModel model, PublicationStatus? status, CancellationToken token)
        {
            _ = status;
            _ = token;

            var price = await GetCataloguePrice(model.CatalogueItemId, model.CataloguePriceId);

            return price.CataloguePriceTiers.Any();
        }

        private async Task<bool> HaveTierWithStartingRange(TieredPriceTiersModel model, PublicationStatus? status, CancellationToken token)
        {
            _ = status;
            _ = token;

            var price = await GetCataloguePrice(model.CatalogueItemId, model.CataloguePriceId);

            return price.CataloguePriceTiers.Any(p => p.LowerRange == StartingLowerRange);
        }

        private async Task<bool> HaveTierWithInfiniteRange(TieredPriceTiersModel model, PublicationStatus? status, CancellationToken token)
        {
            _ = status;
            _ = token;

            var price = await GetCataloguePrice(model.CatalogueItemId, model.CataloguePriceId);

            return price.CataloguePriceTiers.Any(p => p.UpperRange is null);
        }

        private async Task<bool> NotHaveRangeOverlap(TieredPriceTiersModel model, PublicationStatus? status, CancellationToken token)
        {
            _ = status;
            _ = token;

            var price = await GetCataloguePrice(model.CatalogueItemId, model.CataloguePriceId);
            var tiers = price.CataloguePriceTiers.OrderBy(t => t.LowerRange).ToList();

            for (int i = 0; i < tiers.Count; ++i)
            {
                if (i == (tiers.Count - 1))
                    continue;

                var next = tiers[i + 1];
                var current = tiers[i];

                if ((next.LowerRange - current.UpperRange) < 1)
                    return false;
            }

            return true;
        }

        private async Task<bool> NotHaveRangeGaps(TieredPriceTiersModel model, PublicationStatus? status, CancellationToken token)
        {
            _ = status;
            _ = token;

            var price = await GetCataloguePrice(model.CatalogueItemId, model.CataloguePriceId);
            var tiers = price.CataloguePriceTiers.OrderBy(t => t.LowerRange).ToList();

            for (int i = 0; i < tiers.Count; ++i)
            {
                if (i == (tiers.Count - 1))
                    continue;

                var next = tiers[i + 1];
                var current = tiers[i];

                if ((next.LowerRange - current.UpperRange) > 1)
                    return false;
            }

            return true;
        }
    }
}
