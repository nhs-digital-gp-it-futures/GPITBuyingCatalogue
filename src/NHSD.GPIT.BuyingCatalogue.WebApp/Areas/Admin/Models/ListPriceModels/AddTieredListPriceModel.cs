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

        public AddTieredListPriceModel(
            CatalogueItemId catalogueItemId,
            CatalogueItem service)
        {
            CatalogueItemId = catalogueItemId;
            CatalogueItemName = service.Name;
            CatalogueItemType = service.CatalogueItemType;
            ServiceId = service.Id;
        }

        public AddTieredListPriceModel(
            int supplierId,
            CatalogueItem service)
            : this(service)
        {
            SupplierId = supplierId;
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
            };

        public int? SupplierId { get; set; }

        public CatalogueItemId? ServiceId { get; set; }

        public override PricingUnit GetPricingUnit()
            => new()
            {
                Description = UnitDescription, Definition = UnitDefinition, RangeDescription = RangeDefinition,
            };
    }
}
