using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class AspNetUserExtensions
    {
        public static string GetDisplayName(this AspNetUser user)
        {
            return $"{user.FirstName} {user.LastName}";
        }
    }
}
