using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class ReadyToStartModel : NavBaseModel
    {
        public string InternalOrgId { get; set; }

        public TriageOption? Option { get; set; }
    }
}
