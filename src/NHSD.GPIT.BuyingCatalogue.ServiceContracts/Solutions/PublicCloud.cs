using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    [ExcludeFromCodeCoverage]
    public class PublicCloud
    {
        public string Summary { get; set; }

        public string Link { get; set; }

        public string RequiresHscn { get; set; }
    }
}
