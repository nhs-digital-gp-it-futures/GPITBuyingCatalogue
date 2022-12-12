using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public class AddTieredListPriceModel : AddEditFlatListPriceModel
    {
        public AddTieredListPriceModel()
        {
        }

        public AddTieredListPriceModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
        }

        public AddTieredListPriceModel(CatalogueItem catalogueItem, CataloguePrice cataloguePrice)
            : this(cataloguePrice)
        {
            CatalogueItemId = catalogueItem.Id;
            CatalogueItemName = catalogueItem.Name;
            CatalogueItemType = catalogueItem.CatalogueItemType;
        }

        public AddTieredListPriceModel(
            CatalogueItem solution,
            CatalogueItem service)
        {
            CatalogueItemId = solution.Id;
            CatalogueItemName = service.Name;
            CatalogueItemType = service.CatalogueItemType;
            ServiceId = service.Id;
        }

        public AddTieredListPriceModel(
            CatalogueItem solution,
            CatalogueItem service,
            CataloguePrice cataloguePrice)
            : this(cataloguePrice)
        {
            CatalogueItemId = solution.Id;
            CatalogueItemName = service.Name;
            CatalogueItemType = service.CatalogueItemType;
            ServiceId = service.Id;
        }

        private AddTieredListPriceModel(CataloguePrice cataloguePrice)
        {
            CataloguePriceId = cataloguePrice.CataloguePriceId;
            UnitDescription = cataloguePrice.PricingUnit.Description;
            UnitDefinition = cataloguePrice.PricingUnit.Definition;
            RangeDefinition = cataloguePrice.PricingUnit.RangeDescription;

            SelectedCalculationType = cataloguePrice.CataloguePriceCalculationType;
            SelectedProvisioningType = cataloguePrice.ProvisioningType;

            AssignBillingPeriod(cataloguePrice);
            AssignQuantityCalculationType(cataloguePrice);
        }

        public override IEnumerable<SelectOption<CataloguePriceCalculationType>> AvailableCalculationTypes =>
            new List<SelectOption<CataloguePriceCalculationType>>
            {
                new(
                    CataloguePriceCalculationType.SingleFixed.Name(),
                    CataloguePriceCalculationType.SingleFixed.Description(),
                    CataloguePriceCalculationType.SingleFixed),
                new(
                    CataloguePriceCalculationType.Cumulative.Name(),
                    CataloguePriceCalculationType.Cumulative.Description(),
                    CataloguePriceCalculationType.Cumulative),
                new(
                    CataloguePriceCalculationType.Volume.Name(),
                    CataloguePriceCalculationType.Volume.Description(),
                    CataloguePriceCalculationType.Volume),
            };

        public CatalogueItemId? ServiceId { get; set; }

        public override PricingUnit GetPricingUnit()
            => new()
            {
                Description = UnitDescription,
                Definition = UnitDefinition,
                RangeDescription = RangeDefinition,
            };
    }
}
