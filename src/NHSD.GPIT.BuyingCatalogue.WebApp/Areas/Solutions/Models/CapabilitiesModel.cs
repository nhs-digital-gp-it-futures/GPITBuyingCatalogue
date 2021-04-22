using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class CapabilitiesModel : NavBaseModel
    {
        public CapabilityModel[] LeftCapabilities { get; set; }

        public CapabilityModel[] RightCapabilities { get; set; }        
    }
}
