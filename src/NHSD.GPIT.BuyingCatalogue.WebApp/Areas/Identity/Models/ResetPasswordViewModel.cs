using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models
{
    public sealed class ResetPasswordViewModel
    {
        [Required(ErrorMessage = ErrorMessages.PasswordRequired)]
        [DataType(DataType.Password)]
        [DisplayName("Enter a password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = ErrorMessages.PasswordMismatch)]
        [DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        private static class ErrorMessages
        {
            internal const string PasswordRequired = "Enter a password";
            internal const string PasswordMismatch = "Passwords do not match";
        }
    }
}
