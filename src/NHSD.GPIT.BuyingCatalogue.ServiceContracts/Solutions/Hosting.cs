using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    [ExcludeFromCodeCoverage]
    public class Hosting
    {
        public PublicCloud PublicCloud { get; set; }

        public PrivateCloud PrivateCloud { get; set; }

        public HybridHostingType HybridHostingType { get; set; }

        public OnPremise OnPremise { get; set; }
    }
}
