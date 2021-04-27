using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    [ExcludeFromCodeCoverage]
    public class CapabilityModel
    {
        public string CapabilityName { get; set; }

        public string CapabilityRef { get; set; }

        public bool Checked { get; set; }
    }
}
