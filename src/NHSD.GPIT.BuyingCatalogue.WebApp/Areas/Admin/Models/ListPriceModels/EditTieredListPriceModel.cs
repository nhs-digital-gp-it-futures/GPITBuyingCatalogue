using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public class EditTieredListPriceModel : AddTieredListPriceModel
    {
        public EditTieredListPriceModel()
        {
        }

        public EditTieredListPriceModel(CatalogueItem catalogueItem, CataloguePrice price, int maximumNumberOfTiers)
            : base(catalogueItem, price)
        {
            Tiers = price.CataloguePriceTiers.ToList();
            SelectedPublicationStatus
                = CataloguePricePublicationStatus
                = price.PublishedStatus;

            MaximumNumberOfTiers = maximumNumberOfTiers;
        }

        public EditTieredListPriceModel(
            CatalogueItem solution,
            CatalogueItem service,
            CataloguePrice price,
            int maximumNumberOfTiers)
            : base(solution, service, price)
        {
            Tiers = price.CataloguePriceTiers.ToList();
            SelectedPublicationStatus
                = CataloguePricePublicationStatus
                    = price.PublishedStatus;

            MaximumNumberOfTiers = maximumNumberOfTiers;
        }

        public IList<CataloguePriceTier> Tiers { get; set; }

        public int MaximumNumberOfTiers { get; set; }

        public string AddPricingTierUrl { get; set; }
    }
}
