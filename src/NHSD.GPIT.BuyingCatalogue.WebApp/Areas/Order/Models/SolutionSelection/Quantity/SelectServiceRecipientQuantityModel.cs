using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Quantity
{
    public class SelectServiceRecipientQuantityModel : NavBaseModel
    {
        public string ItemName { get; set; }

        public string ItemType { get; set; }

        public ServiceRecipientQuantityModel[] ServiceRecipients { get; set; }
    }
}
