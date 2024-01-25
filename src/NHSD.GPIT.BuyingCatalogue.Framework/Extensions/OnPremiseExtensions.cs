using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class OnPremiseExtensions
    {
        public static TaskProgress Status(this OnPremise onPremise)
        {
            if (onPremise is null || string.IsNullOrEmpty(onPremise.Summary) || string.IsNullOrEmpty(onPremise.HostingModel))
                return TaskProgress.NotStarted;

            return TaskProgress.Completed;
        }
    }
}
