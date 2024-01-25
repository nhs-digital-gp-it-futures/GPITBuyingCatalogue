using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class PrivateCloudExtensions
    {
        public static TaskProgress Status(this PrivateCloud privateCloud)
        {
            if (privateCloud is null || string.IsNullOrEmpty(privateCloud.Summary) || string.IsNullOrEmpty(privateCloud.HostingModel))
                return TaskProgress.NotStarted;

            return TaskProgress.Completed;
        }
    }
}
