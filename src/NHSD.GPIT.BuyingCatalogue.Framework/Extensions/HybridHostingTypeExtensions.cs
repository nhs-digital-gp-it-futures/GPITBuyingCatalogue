using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class HybridHostingTypeExtensions
    {
        public static TaskProgress Status(this HybridHostingType hybridHostingType)
        {
            if (hybridHostingType is null || string.IsNullOrEmpty(hybridHostingType.Summary) || string.IsNullOrEmpty(hybridHostingType.HostingModel))
                return TaskProgress.NotStarted;

            return TaskProgress.Completed;
        }
    }
}
