using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public sealed class EditListPriceStatusModel : NavBaseModel
    {
        public EditListPriceStatusModel()
        {
        }

        public EditListPriceStatusModel(CatalogueItem catalogueItem)
        {
            CatalogueItemId = catalogueItem.Id;
            SolutionName = catalogueItem.Name;
        }

        public EditListPriceStatusModel(CatalogueItem catalogueItem, CataloguePrice cataloguePrice)
            : this(catalogueItem)
        {
            CataloguePriceId = cataloguePrice.CataloguePriceId;
            AvailableStatuses = cataloguePrice.IsLocked ? LockedItemStatues : UnlockedItemStatuses;
            Status = cataloguePrice.PublishedStatus;
        }

        public EditListPriceStatusModel(CatalogueItem catalogueItem, CataloguePrice cataloguePrice, CatalogueItemId relatedCatalogueItemId)
            : this(catalogueItem, cataloguePrice)
        {
            RelatedCatalogueItemId = relatedCatalogueItemId;
        }

        public static IEnumerable<PublicationStatus> UnlockedItemStatuses => new PublicationStatus[]
        {
            PublicationStatus.Published,
            PublicationStatus.Draft,
        };

        public static IEnumerable<PublicationStatus> LockedItemStatues => new PublicationStatus[]
        {
            PublicationStatus.Published,
            PublicationStatus.Unpublished,
        };

        public string Title { get; init; }

        public CatalogueItemId CatalogueItemId { get; init; }

        public CatalogueItemId RelatedCatalogueItemId { get; }

        public int CataloguePriceId { get; init; }

        public IEnumerable<PublicationStatus> AvailableStatuses { get; init; }

        public string SolutionName { get; init; }

        public string Advice { get; init; }

        public PublicationStatus Status { get; init; }

        public int? ListPriceId { get; init; }

        public EditListPriceSource CalledFrom { get; init; }
    }
}
