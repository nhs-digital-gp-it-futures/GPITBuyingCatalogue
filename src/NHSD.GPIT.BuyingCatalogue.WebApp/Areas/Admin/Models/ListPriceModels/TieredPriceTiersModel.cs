using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public class TieredPriceTiersModel : NavBaseModel
    {
        public TieredPriceTiersModel()
        {
        }

        public TieredPriceTiersModel(CatalogueItem catalogueItem, CataloguePrice price, int maximumNumberOfTiers)
            : this(price, maximumNumberOfTiers)
        {
            CatalogueItemId = catalogueItem.Id;
            CatalogueItemName = catalogueItem.Name;
            CatalogueItemType = catalogueItem.CatalogueItemType;
        }

        public TieredPriceTiersModel(
            CatalogueItem solution,
            CatalogueItem service,
            CataloguePrice price,
            int maximumNumberOfTiers)
        : this(price, maximumNumberOfTiers)
        {
            CatalogueItemId = solution.Id;
            CatalogueItemName = service.Name;
            CatalogueItemType = service.CatalogueItemType;
            ServiceId = service.Id;
        }

        private TieredPriceTiersModel(CataloguePrice price, int maximumNumberOfTiers)
        {
            CataloguePriceId = price.CataloguePriceId;
            Tiers = price.CataloguePriceTiers?.ToList();
            MaximumNumberOfTiers = maximumNumberOfTiers;
        }

        public static IList<SelectOption<PublicationStatus>> AvailablePublicationStatuses => new List<SelectOption<PublicationStatus>>
        {
            new(PublicationStatus.Draft.Description(), PublicationStatus.Draft),
            new(PublicationStatus.Published.Description(), PublicationStatus.Published),
        };

        public int CataloguePriceId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public CatalogueItemId? ServiceId { get; set; }

        public string CatalogueItemName { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public IList<CataloguePriceTier> Tiers { get; set; }

        public PublicationStatus? SelectedPublicationStatus { get; set; } = PublicationStatus.Draft;

        public int MaximumNumberOfTiers { get; set; }

        public string AddTieredPriceTierUrl { get; set; }
    }
}
