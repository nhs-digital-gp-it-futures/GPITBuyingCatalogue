﻿using System.Linq;
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
        private const int StartingLowerRange = 1;
        private readonly ISolutionListPriceService solutionListPriceService;

        public TieredPriceTiersModelValidator(ISolutionListPriceService solutionListPriceService)
        {
            this.solutionListPriceService = solutionListPriceService;

            RuleFor(m => m.SelectedPublicationStatus)
                .Cascade(CascadeMode.Stop)
                .MustAsync(HaveAtLeastOneTier)
                .WithMessage(SharedListPriceValidationErrors.MissingTiersError)
                .MustAsync(HaveTierWithStartingRange)
                .WithMessage(SharedListPriceValidationErrors.InvalidStartingRangeError)
                .MustAsync(HaveTierWithInfiniteRange)
                .WithMessage(SharedListPriceValidationErrors.InvalidEndingRangeError)
                .MustAsync(NotHaveRangeGaps)
                .WithMessage(SharedListPriceValidationErrors.RangeGapError)
                .MustAsync(NotHaveRangeOverlap)
                .WithMessage(SharedListPriceValidationErrors.RangeOverlapError)
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

            return !price.HasTierRangeOverlap();
        }

        private async Task<bool> NotHaveRangeGaps(TieredPriceTiersModel model, PublicationStatus? status, CancellationToken token)
        {
            _ = status;
            _ = token;

            var price = await GetCataloguePrice(model.CatalogueItemId, model.CataloguePriceId);

            return !price.HasTierRangeGaps();
        }
    }
}
