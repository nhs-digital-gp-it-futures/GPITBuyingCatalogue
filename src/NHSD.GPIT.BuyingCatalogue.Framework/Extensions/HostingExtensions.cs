using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class HostingExtensions
    {
        public static IReadOnlyList<HostingType> AvailableHosting(this Hosting hosting)
        {
            if (hosting is null)
                throw new ArgumentNullException(nameof(hosting));

            var result = new List<HostingType>(4);

            if (hosting.PublicCloud.Status() == TaskProgress.Completed)
                result.Add(HostingType.PublicCloud);

            if (hosting.PrivateCloud.Status() == TaskProgress.Completed)
                result.Add(HostingType.PrivateCloud);

            if (hosting.HybridHostingType.Status() == TaskProgress.Completed)
                result.Add(HostingType.Hybrid);

            if (hosting.OnPremise.Status() == TaskProgress.Completed)
                result.Add(HostingType.OnPremise);

            return result;
        }

        public static TaskProgress HostingTypeStatus(this Hosting hosting, HostingType hostingType)
        {
            if (hosting is null)
                throw new ArgumentNullException(nameof(hosting));

            return hostingType switch
            {
                HostingType.Hybrid => hosting.HybridHostingType?.Status() ?? TaskProgress.NotStarted,
                HostingType.OnPremise => hosting.OnPremise?.Status() ?? TaskProgress.NotStarted,
                HostingType.PrivateCloud => hosting.PrivateCloud?.Status() ?? TaskProgress.NotStarted,
                HostingType.PublicCloud => hosting.PublicCloud?.Status() ?? TaskProgress.NotStarted,
                _ => TaskProgress.NotStarted,
            };
        }

        public static bool HasHostingType(this Hosting hosting, HostingType hostingType)
        {
            if (hosting is null)
                throw new ArgumentNullException(nameof(hosting));

            return hostingType switch
            {
                HostingType.Hybrid => hosting.HybridHostingType != null && hosting.HybridHostingType.IsValid(),
                HostingType.OnPremise => hosting.OnPremise != null && hosting.OnPremise.IsValid(),
                HostingType.PrivateCloud => hosting.PrivateCloud != null && hosting.PrivateCloud.IsValid(),
                HostingType.PublicCloud => hosting.PublicCloud != null && hosting.PublicCloud.IsValid(),
                _ => false,
            };
        }
    }
}
