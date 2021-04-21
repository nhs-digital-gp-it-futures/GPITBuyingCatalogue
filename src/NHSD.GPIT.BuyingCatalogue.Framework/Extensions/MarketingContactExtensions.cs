using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions
{
    public static class MarketingContactExtensions
    {
        public static bool IsEmpty(this MarketingContact marketingContact)
        {
            if (string.IsNullOrWhiteSpace(marketingContact.FirstName)
                && string.IsNullOrWhiteSpace(marketingContact.LastName)
                && string.IsNullOrWhiteSpace(marketingContact.Department)
                && string.IsNullOrWhiteSpace(marketingContact.PhoneNumber)
                && string.IsNullOrWhiteSpace(marketingContact.Email))
                return true;

            return false;
        }
    }
}
