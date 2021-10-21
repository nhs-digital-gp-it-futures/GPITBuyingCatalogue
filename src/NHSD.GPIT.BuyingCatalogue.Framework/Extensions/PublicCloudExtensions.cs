using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class PublicCloudExtensions
    {
        public static TaskProgress Status(this PublicCloud publicCloud)
        {
            if (publicCloud is null)
                throw new ArgumentNullException(nameof(publicCloud));

            return string.IsNullOrEmpty(publicCloud.Summary)
                ? TaskProgress.NotStarted
                : TaskProgress.Completed;
        }
    }
}
