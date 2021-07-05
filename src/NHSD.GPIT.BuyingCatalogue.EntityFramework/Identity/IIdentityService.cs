using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity
{
    public interface IIdentityService
    {
        (Guid UserId, string UserName) GetUserInfo();
    }
}
