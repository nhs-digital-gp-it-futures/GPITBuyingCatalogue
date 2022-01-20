using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class ReadyToStartModel : NavBaseModel
    {
        public ReadyToStartModel(string odsCode, TriageOption? option)
        {
            OdsCode = odsCode;
            Option = option;
        }

        public string OdsCode { get; set; }

        public TriageOption? Option { get; set; }
    }
}
