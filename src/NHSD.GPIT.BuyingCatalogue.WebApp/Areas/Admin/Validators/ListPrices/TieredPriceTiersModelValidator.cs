using System.Linq;
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
                .Must(HaveAtLeastOneTier)
                .WithMessage(SharedListPriceValidationErrors.MissingTiersError)
                .Must(HaveTierWithStartingRange)
                .WithMessage(SharedListPriceValidationErrors.InvalidStartingRangeError)
                .Must(HaveTierWithInfiniteRange)
                .WithMessage(SharedListPriceValidationErrors.InvalidEndingRangeError)
                .Custom(EnsureNoGapsOrOverlap)
                .When(m => m.SelectedPublicationStatus == PublicationStatus.Published);
        }

        private CataloguePrice GetCataloguePrice(CatalogueItemId solutionId, int cataloguePriceId)
        {
            var solution = solutionListPriceService.GetCatalogueItemWithListPrices(solutionId).GetAwaiter().GetResult();
            var price = solution.CataloguePrices.Single(p => p.CataloguePriceId == cataloguePriceId);
            return price;
        }

        private bool HaveAtLeastOneTier(TieredPriceTiersModel model, PublicationStatus? status)
        {
            var price = GetCataloguePrice(model.CatalogueItemId, model.CataloguePriceId);

            return price.CataloguePriceTiers.Any();
        }

        private bool HaveTierWithStartingRange(TieredPriceTiersModel model, PublicationStatus? status)
        {
            var price = GetCataloguePrice(model.CatalogueItemId, model.CataloguePriceId);

            return price.CataloguePriceTiers.Any(p => p.LowerRange == StartingLowerRange);
        }

        private bool HaveTierWithInfiniteRange(TieredPriceTiersModel model, PublicationStatus? status)
        {
            var price = GetCataloguePrice(model.CatalogueItemId, model.CataloguePriceId);

            return price.CataloguePriceTiers.Any(p => p.UpperRange is null);
        }

        private void EnsureNoGapsOrOverlap(PublicationStatus? status, ValidationContext<TieredPriceTiersModel> validationContext)
        {
            var model = validationContext.InstanceToValidate;

            var price = GetCataloguePrice(model.CatalogueItemId, model.CataloguePriceId);

            (bool hasGaps, int? gapLowerTierIndex, int? gapUpperTierIndex) = price.HasTierRangeGaps();
            (bool hasOverlap, int? overlapLowerTierIndex, int? overlapUpperTierIndex) = price.HasTierRangeOverlap();

            if (hasGaps)
                validationContext.AddFailure(string.Format(SharedListPriceValidationErrors.RangeGapError, gapLowerTierIndex, gapUpperTierIndex));
            else if (hasOverlap)
                validationContext.AddFailure(string.Format(SharedListPriceValidationErrors.RangeOverlapError, overlapLowerTierIndex, overlapUpperTierIndex));
        }
    }
}
