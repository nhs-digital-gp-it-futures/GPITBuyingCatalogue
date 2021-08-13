using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Database
{
    public sealed class IdentityServiceStub : IIdentityService
    {
        public (int UserId, string UserName) GetUserInfo()
        {
            return (0, "Mr Test");
        }
    }
}
