using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices
{
    public class EditTieredListPriceModelValidator : AbstractValidator<EditTieredListPriceModel>
    {
        internal const string CannotUnpublishError = "This list price cannot be unpublished as it is the only one for this Catalogue Solution";

        private const int StartingLowerRange = 1;

        private readonly IListPriceService listPriceService;
        private readonly ISolutionListPriceService solutionListPriceService;

        public EditTieredListPriceModelValidator(
            IListPriceService listPriceService,
            ISolutionListPriceService solutionListPriceService)
        {
            this.listPriceService = listPriceService;
            this.solutionListPriceService = solutionListPriceService;

            Include(new AddTieredListPriceModelValidator(listPriceService));

            RuleFor(m => m.SelectedPublicationStatus)
                .Cascade(CascadeMode.Stop)
                .MustAsync(NotBeTheLastRemainingListPrice)
                .WithMessage(CannotUnpublishError)
                .When(m => m.SelectedPublicationStatus == PublicationStatus.Unpublished)
                .MustAsync(HaveAtLeastOneTier)
                .WithMessage(SharedListPriceValidationErrors.MissingTiersError)
                .MustAsync(HaveTierWithStartingRange)
                .WithMessage(SharedListPriceValidationErrors.InvalidStartingRangeError)
                .MustAsync(HaveTierWithInfiniteRange)
                .WithMessage(SharedListPriceValidationErrors.InvalidEndingRangeError)
                .CustomAsync(EnsureNoGapsOrOverlap)
                .When(m => m.SelectedPublicationStatus == PublicationStatus.Published);
        }

        private async Task<bool> NotBeTheLastRemainingListPrice(EditTieredListPriceModel model, PublicationStatus status, CancellationToken token)
        {
            _ = token;

            return await listPriceService.GetNumberOfListPrices(model.CatalogueItemId, model.CataloguePriceId!.Value) > 0;
        }

        private async Task<CataloguePrice> GetCataloguePrice(CatalogueItemId solutionId, int? cataloguePriceId)
        {
            var solution = await solutionListPriceService.GetSolutionWithListPrices(solutionId);
            var price = solution.CataloguePrices.Single(p => p.CataloguePriceId == cataloguePriceId);
            return price;
        }

        private async Task<bool> HaveAtLeastOneTier(EditTieredListPriceModel model, PublicationStatus status, CancellationToken token)
        {
            _ = status;
            _ = token;

            var price = await GetCataloguePrice(model.CatalogueItemId, model.CataloguePriceId);

            return price.CataloguePriceTiers.Any();
        }

        private async Task<bool> HaveTierWithStartingRange(EditTieredListPriceModel model, PublicationStatus status, CancellationToken token)
        {
            _ = status;
            _ = token;

            var price = await GetCataloguePrice(model.CatalogueItemId, model.CataloguePriceId);

            return price.CataloguePriceTiers.Any(p => p.LowerRange == StartingLowerRange);
        }

        private async Task<bool> HaveTierWithInfiniteRange(EditTieredListPriceModel model, PublicationStatus status, CancellationToken token)
        {
            _ = status;
            _ = token;

            var price = await GetCataloguePrice(model.CatalogueItemId, model.CataloguePriceId);

            return price.CataloguePriceTiers.Any(p => p.UpperRange is null);
        }

        private async Task EnsureNoGapsOrOverlap(PublicationStatus status, ValidationContext<EditTieredListPriceModel> validationContext, CancellationToken token)
        {
            _ = status;
            _ = token;

            var model = validationContext.InstanceToValidate;

            var price = await GetCataloguePrice(model.CatalogueItemId, model.CataloguePriceId);

            (bool hasGaps, int? gapLowerTierIndex, int? gapUpperTierIndex) = price.HasTierRangeGaps();
            (bool hasOverlap, int? overlapLowerTierIndex, int? overlapUpperTierIndex) = price.HasTierRangeOverlap();
            if (hasGaps)
            {
                validationContext.AddFailure(string.Format(SharedListPriceValidationErrors.RangeGapError, gapLowerTierIndex, gapUpperTierIndex));

                return;
            }
            else if (hasOverlap)
            {
                validationContext.AddFailure(string.Format(SharedListPriceValidationErrors.RangeOverlapError, overlapLowerTierIndex, overlapUpperTierIndex));

                return;
            }
        }
    }
}
