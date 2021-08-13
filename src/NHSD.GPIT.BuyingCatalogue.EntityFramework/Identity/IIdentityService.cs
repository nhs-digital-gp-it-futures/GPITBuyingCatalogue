namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity
{
    public interface IIdentityService
    {
        (int UserId, string UserName) GetUserInfo();
    }
}
