using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders
{
    public class AdminViewOrderItem
    {
        public string Name { get; set; }

        public CatalogueItemType Type { get; set; }

        public OrderItemFundingType SelectedFundingType { get; set; }
    }
}
