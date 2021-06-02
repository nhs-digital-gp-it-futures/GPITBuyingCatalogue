using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Errors;

namespace NHSD.GPIT.BuyingCatalogue.Services.CreateBuyer
{
    public static class AspNetUserErrors
    {
        public static ErrorDetails FirstNameRequired()
        {
            return new("FirstNameRequired", nameof(AspNetUser.FirstName));
        }

        public static ErrorDetails FirstNameTooLong()
        {
            return new("FirstNameTooLong", nameof(AspNetUser.FirstName));
        }

        public static ErrorDetails LastNameRequired()
        {
            return new("LastNameRequired", nameof(AspNetUser.LastName));
        }

        public static ErrorDetails LastNameTooLong()
        {
            return new("LastNameTooLong", nameof(AspNetUser.LastName));
        }

        public static ErrorDetails PhoneNumberRequired()
        {
            return new("PhoneNumberRequired", nameof(AspNetUser.PhoneNumber));
        }

        public static ErrorDetails PhoneNumberTooLong()
        {
            return new("PhoneNumberTooLong", nameof(AspNetUser.PhoneNumber));
        }

        public static ErrorDetails EmailRequired()
        {
            return new("EmailRequired", "EmailAddress");
        }

        public static ErrorDetails EmailTooLong()
        {
            return new("EmailTooLong", "EmailAddress");
        }

        public static ErrorDetails EmailInvalidFormat()
        {
            return new("EmailInvalidFormat", "EmailAddress");
        }

        public static ErrorDetails EmailAlreadyExists()
        {
            return new("EmailAlreadyExists", "EmailAddress");
        }
    }
}
