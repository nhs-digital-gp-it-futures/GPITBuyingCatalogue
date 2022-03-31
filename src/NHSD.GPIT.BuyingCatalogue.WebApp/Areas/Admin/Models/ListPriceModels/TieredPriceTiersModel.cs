using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public class TieredPriceTiersModel : NavBaseModel
    {
        public TieredPriceTiersModel()
        {
        }

        public TieredPriceTiersModel(CatalogueItem catalogueItem, CataloguePrice price)
        {
            CataloguePriceId = price.CataloguePriceId;
            CatalogueItemId = catalogueItem.Id;
            CatalogueItemName = catalogueItem.Name;
            Tiers = price.CataloguePriceTiers?.ToList();
        }

        public static IList<SelectableRadioOption<PublicationStatus>> AvailablePublicationStatuses => new List<SelectableRadioOption<PublicationStatus>>
        {
            new(PublicationStatus.Draft.Description(), PublicationStatus.Draft),
            new(PublicationStatus.Published.Description(), PublicationStatus.Published),
        };

        public int CataloguePriceId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string CatalogueItemName { get; set; }

        public IList<CataloguePriceTier> Tiers { get; set; }

        public PublicationStatus? SelectedPublicationStatus { get; set; } = PublicationStatus.Draft;
    }
}
