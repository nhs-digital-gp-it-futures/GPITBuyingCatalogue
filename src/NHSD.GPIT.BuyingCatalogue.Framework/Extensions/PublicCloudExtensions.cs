﻿using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class PublicCloudExtensions
    {
        public static TaskProgress Status(this PublicCloud publicCloud) => string.IsNullOrEmpty(publicCloud.Summary)
            ? TaskProgress.NotStarted
            : TaskProgress.Completed;
    }
}
