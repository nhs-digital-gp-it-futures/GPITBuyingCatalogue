using System.Linq;
using FluentValidation;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Validators.ListPrices
{
    public class EditTieredListPriceModelValidator : AbstractValidator<EditTieredListPriceModel>
    {
        internal const string CannotUnpublishError = "This list price cannot be unpublished as it is the only one for this Catalogue Solution";

        private const int StartingLowerRange = 1;

        private readonly IListPriceService listPriceService;

        public EditTieredListPriceModelValidator(
            IListPriceService listPriceService)
        {
            this.listPriceService = listPriceService;

            Include(new AddTieredListPriceModelValidator(listPriceService));

            RuleFor(m => m.SelectedPublicationStatus)
                .Must(NotBeTheLastRemainingListPrice)
                .WithMessage(CannotUnpublishError)
                .When(m => m.SelectedPublicationStatus == PublicationStatus.Unpublished)
                .Must(HaveAtLeastOneTier)
                .WithMessage(SharedListPriceValidationErrors.MissingTiersError)
                .Must(HaveTierWithStartingRange)
                .WithMessage(SharedListPriceValidationErrors.InvalidStartingRangeError)
                .Must(HaveTierWithInfiniteRange)
                .WithMessage(SharedListPriceValidationErrors.InvalidEndingRangeError)
                .Custom(EnsureNoGapsOrOverlap)
                .When(m => m.SelectedPublicationStatus == PublicationStatus.Published);
        }

        private static CatalogueItemId GetCatalogueItemId(EditTieredListPriceModel model) =>
            model.CatalogueItemType == CatalogueItemType.Solution
                ? model.CatalogueItemId
                : model.ServiceId!.Value;

        private bool NotBeTheLastRemainingListPrice(EditTieredListPriceModel model, PublicationStatus? status)
        {
            return listPriceService.GetNumberOfListPrices(GetCatalogueItemId(model), model.CataloguePriceId!.Value).GetAwaiter().GetResult() > 0;
        }

        private CataloguePrice GetCataloguePrice(EditTieredListPriceModel model, int? cataloguePriceId)
        {
            var catalogueItemId = GetCatalogueItemId(model);

            var catalogueItem = listPriceService.GetCatalogueItemWithListPrices(catalogueItemId).GetAwaiter().GetResult();
            var price = catalogueItem.CataloguePrices.First(p => p.CataloguePriceId == cataloguePriceId);
            return price;
        }

        private bool HaveAtLeastOneTier(EditTieredListPriceModel model, PublicationStatus? status)
        {
            var price = GetCataloguePrice(model, model.CataloguePriceId);

            return price.CataloguePriceTiers.Any();
        }

        private bool HaveTierWithStartingRange(EditTieredListPriceModel model, PublicationStatus? status)
        {
            var price = GetCataloguePrice(model, model.CataloguePriceId);

            return price.CataloguePriceTiers.Any(p => p.LowerRange == StartingLowerRange);
        }

        private bool HaveTierWithInfiniteRange(EditTieredListPriceModel model, PublicationStatus? status)
        {
            var price = GetCataloguePrice(model, model.CataloguePriceId);

            return price.CataloguePriceTiers.Any(p => p.UpperRange is null);
        }

        private void EnsureNoGapsOrOverlap(PublicationStatus? status, ValidationContext<EditTieredListPriceModel> validationContext)
        {
            var model = validationContext.InstanceToValidate;

            var price = GetCataloguePrice(model, model.CataloguePriceId);

            (bool hasGaps, int? gapLowerTierIndex, int? gapUpperTierIndex) = price.HasTierRangeGaps();
            (bool hasOverlap, int? overlapLowerTierIndex, int? overlapUpperTierIndex) = price.HasTierRangeOverlap();

            if (hasGaps)
                validationContext.AddFailure(string.Format(SharedListPriceValidationErrors.RangeGapError, gapLowerTierIndex, gapUpperTierIndex));
            else if (hasOverlap)
                validationContext.AddFailure(string.Format(SharedListPriceValidationErrors.RangeOverlapError, overlapLowerTierIndex, overlapUpperTierIndex));
        }
    }
}
