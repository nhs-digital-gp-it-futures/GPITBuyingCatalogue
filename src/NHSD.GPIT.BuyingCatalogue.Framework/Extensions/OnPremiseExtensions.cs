using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class OnPremiseExtensions
    {
        public static TaskProgress Status(this OnPremise onPremise)
        {
            if (string.IsNullOrEmpty(onPremise.Summary) || string.IsNullOrEmpty(onPremise.HostingModel))
                return TaskProgress.NotStarted;

            return TaskProgress.Completed;
        }
    }
}
