using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.UI.Components.DataAttributes;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models
{
    public sealed class LoginViewModel
    {
        [Required(ErrorMessage = ErrorMessages.EmailAddressRequired)]
        [EmailAddress(ErrorMessage = ErrorMessages.EmailAddressInvalid)]
        [DisplayName("Email address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = ErrorMessages.PasswordRequired)]
        [Password]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public string Error { get; set; }

        public string DisabledError { get; set; }

#pragma warning disable SA1300
        // Required to force recaptcha errors to appear in the correct order
        public string recaptcha { get; set; }
#pragma warning restore SA1300

        public static class ErrorMessages
        {
            public const string EmailAddressRequired = "Enter your email address";
            public const string EmailAddressInvalid = "Enter a valid email address";
            public const string PasswordRequired = "Enter your password";
        }
    }
}
