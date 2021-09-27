using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class HybridHostingTypeExtensions
    {
        public static TaskProgress Status(this HybridHostingType hybridHostingType)
        {
            if (string.IsNullOrEmpty(hybridHostingType.Summary) || string.IsNullOrEmpty(hybridHostingType.HostingModel))
                return TaskProgress.NotStarted;

            return TaskProgress.Completed;
        }
    }
}
