using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration
{
    class IdentityServiceStub : IIdentityService
    {
        public int? GetUserId()
        {
            return null;
        }
    }
}
