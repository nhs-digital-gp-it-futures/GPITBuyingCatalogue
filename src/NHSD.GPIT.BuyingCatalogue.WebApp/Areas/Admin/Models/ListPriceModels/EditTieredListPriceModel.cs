using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public class EditTieredListPriceModel : AddTieredListPriceModel
    {
        public EditTieredListPriceModel()
        {
        }

        public EditTieredListPriceModel(CatalogueItem catalogueItem, CataloguePrice price, int maximumNumberOfTiers)
            : base(catalogueItem)
        {
            SetPriceDetails(price, maximumNumberOfTiers);
        }

        public EditTieredListPriceModel(
            CatalogueItemId catalogueItemId,
            CatalogueItem service,
            CataloguePrice price,
            int maximumNumberOfTiers)
            : base(catalogueItemId, service)
        {
            SetPriceDetails(price, maximumNumberOfTiers);
        }

        public EditTieredListPriceModel(
            int supplierId,
            CatalogueItem service,
            CataloguePrice price,
            int maximumNumberOfTiers)
            : this(service, price, maximumNumberOfTiers)
        {
            SupplierId = supplierId;
        }

        public IList<CataloguePriceTier> Tiers { get; set; }

        public int MaximumNumberOfTiers { get; set; }

        public string AddPricingTierUrl { get; set; }

        private void SetPriceDetails(CataloguePrice price, int maximumNumberOfTiers)
        {
            CataloguePriceId = price.CataloguePriceId;
            UnitDescription = price.PricingUnit.Description;
            UnitDefinition = price.PricingUnit.Definition;
            RangeDefinition = price.PricingUnit.RangeDescription;

            SelectedCalculationType = price.CataloguePriceCalculationType;
            SelectedProvisioningType = price.ProvisioningType;

            AssignBillingPeriod(price);
            AssignQuantityCalculationType(price);

            Tiers = price.CataloguePriceTiers.ToList();
            SelectedPublicationStatus
                = CataloguePricePublicationStatus
                    = price.PublishedStatus;

            MaximumNumberOfTiers = maximumNumberOfTiers;
        }
    }
}
