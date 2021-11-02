using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Database
{
    public sealed class IdentityServiceStub : IIdentityService
    {
        public int GetUserId()
        {
            return UserSeedData.BobId;
        }
    }
}
