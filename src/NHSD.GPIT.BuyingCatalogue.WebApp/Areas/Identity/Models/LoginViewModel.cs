using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.WebApp.DataAttributes;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models
{
    public sealed class LoginViewModel
    {
        [Required(ErrorMessage = ErrorMessages.EmailAddressRequired)]
        [EmailAddress(ErrorMessage = ErrorMessages.EmailAddressInvalid)]
        [DisplayName("Email address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = ErrorMessages.PasswordRequired)]
        [DataType(DataType.Password)]

        [Password]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public string Error { get; set; }

        public string DisabledError { get; set; }

        public static class ErrorMessages
        {
            public const string EmailAddressRequired = "Enter your email address";
            public const string EmailAddressInvalid = "Enter a valid email address";
            public const string PasswordRequired = "Enter your password";
        }
    }
}
