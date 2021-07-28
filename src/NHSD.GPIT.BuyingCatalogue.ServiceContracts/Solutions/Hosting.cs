using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public sealed class Hosting
    {
        public PublicCloud PublicCloud { get; set; }

        public PrivateCloud PrivateCloud { get; set; }

        public HybridHostingType HybridHostingType { get; set; }

        public OnPremise OnPremise { get; set; }

        public IReadOnlyList<HostingType> AvailableHosting
        {
            get
            {
                var result = new List<HostingType>(4);

                if (PublicCloud?.IsValid() ?? false)
                    result.Add(HostingType.PublicCloud);

                if (PrivateCloud?.IsValid() ?? false)
                    result.Add(HostingType.PrivateCloud);

                if (HybridHostingType?.IsValid() ?? false)
                    result.Add(HostingType.Hybrid);

                if (OnPremise?.IsValid() ?? false)
                    result.Add(HostingType.OnPremise);

                return result;
            }
        }
    }
}
