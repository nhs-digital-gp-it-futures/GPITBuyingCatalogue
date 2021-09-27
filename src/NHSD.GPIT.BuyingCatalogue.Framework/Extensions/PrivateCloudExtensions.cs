using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class PrivateCloudExtensions
    {
        public static TaskProgress Status(this PrivateCloud privateCloud)
        {
            if (string.IsNullOrEmpty(privateCloud.Summary) || string.IsNullOrEmpty(privateCloud.HostingModel))
                return TaskProgress.NotStarted;

            return TaskProgress.Completed;
        }
    }
}
