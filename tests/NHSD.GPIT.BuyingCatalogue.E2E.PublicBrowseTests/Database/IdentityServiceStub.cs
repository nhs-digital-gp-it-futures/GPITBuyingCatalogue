using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Database
{
    public class IdentityServiceStub : IIdentityService
    {
        public (Guid UserId, string UserName) GetUserInfo()
        {
            return (Guid.Empty, "Mr Test");
        }
    }
}
