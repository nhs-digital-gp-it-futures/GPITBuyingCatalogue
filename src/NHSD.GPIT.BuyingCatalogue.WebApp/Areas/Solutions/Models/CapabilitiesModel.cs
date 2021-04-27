using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    [ExcludeFromCodeCoverage]
    public class CapabilitiesModel : NavBaseModel
    {
        public CapabilityModel[] LeftCapabilities { get; set; }

        public CapabilityModel[] RightCapabilities { get; set; }        
    }
}
