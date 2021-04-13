using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models
{
    public sealed class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = ErrorMessages.EmailAddressRequired)]
        [EmailAddress(ErrorMessage = ErrorMessages.EmailAddressInvalid)]
        [DisplayName("Email address")]
        public string EmailAddress { get; set; }

        public static class ErrorMessages
        {
            public const string EmailAddressRequired = "Enter an email address";
            public const string EmailAddressInvalid = "Enter an email address in the correct format, like name@example.com";
        }
    }
}
