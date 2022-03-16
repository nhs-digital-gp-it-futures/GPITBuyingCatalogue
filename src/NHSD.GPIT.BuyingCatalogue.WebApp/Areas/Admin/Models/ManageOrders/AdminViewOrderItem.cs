using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ManageOrders
{
    public class AdminViewOrderItem
    {
        public string Name { get; set; }

        public CatalogueItemType Type { get; set; }

        public string Framework { get; set; }

        public bool? FundingSourceOnlyGms { get; set; }

        public bool? ConfirmedFundingSource { get; set; }
    }
}
