using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class ReadyToStartModel : NavBaseModel
    {
        public ReadyToStartModel(string odsCode)
        {
            OdsCode = odsCode;
        }

        public string OdsCode { get; set; }
    }
}
