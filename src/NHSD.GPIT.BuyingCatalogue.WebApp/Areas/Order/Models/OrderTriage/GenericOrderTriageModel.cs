using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage
{
    public class GenericOrderTriageModel : NavBaseModel
    {
        public string OdsCode { get; set; }

        public string OrdersDashboardLink { get; set; }
    }
}
