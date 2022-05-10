using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Quantity
{
    public class SelectOrderItemQuantityModel : NavBaseModel
    {
        public string ItemName { get; set; }

        public string ItemType { get; set; }

        public string Quantity { get; set; }
    }
}
