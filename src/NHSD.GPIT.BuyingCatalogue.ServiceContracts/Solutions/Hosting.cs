using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    [ExcludeFromCodeCoverage]
    public class Hosting
    {
        public Hosting()
        {
            PublicCloud = new PublicCloud();
            PrivateCloud = new PrivateCloud();
            HybridHostingType = new HybridHostingType();
            OnPremise = new OnPremise();
        }

        public virtual PublicCloud PublicCloud { get; set; }

        public virtual PrivateCloud PrivateCloud { get; set; }

        public virtual HybridHostingType HybridHostingType { get; set; }

        public virtual OnPremise OnPremise { get; set; }
    }
}
