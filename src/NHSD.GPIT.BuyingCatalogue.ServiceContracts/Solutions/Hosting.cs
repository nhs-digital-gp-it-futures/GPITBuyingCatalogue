using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public sealed class Hosting
    {
        public PublicCloud PublicCloud { get; set; } = new();

        public PrivateCloud PrivateCloud { get; set; } = new();

        public HybridHostingType HybridHostingType { get; set; } = new();

        public OnPremise OnPremise { get; set; } = new();

        public IReadOnlyList<HostingType> AvailableHosting
        {
            get
            {
                var result = new List<HostingType>(4);

                if (PublicCloud.Status() == TaskProgress.Completed)
                    result.Add(HostingType.PublicCloud);

                if (PrivateCloud.Status() == TaskProgress.Completed)
                    result.Add(HostingType.PrivateCloud);

                if (HybridHostingType.Status() == TaskProgress.Completed)
                    result.Add(HostingType.Hybrid);

                if (OnPremise.Status() == TaskProgress.Completed)
                    result.Add(HostingType.OnPremise);

                return result;
            }
        }

        public TaskProgress HostingTypeStatus(HostingType hostingType)
        {
            return hostingType switch
            {
                HostingType.Hybrid => HybridHostingType.Status(),
                HostingType.OnPremise => PrivateCloud.Status(),
                HostingType.PrivateCloud => PrivateCloud.Status(),
                HostingType.PublicCloud => PublicCloud.Status(),
                _ => TaskProgress.NotStarted,
            };
        }
    }
}
