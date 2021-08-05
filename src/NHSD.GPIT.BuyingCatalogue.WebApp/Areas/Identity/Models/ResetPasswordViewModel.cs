using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.UI.Components.DataAttributes;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models
{
    public sealed class ResetPasswordViewModel
    {
        [Required(ErrorMessage = ErrorMessages.PasswordRequired)]
        [DisplayName("Enter a password")]
        [Password]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = ErrorMessages.PasswordMismatch)]
        [DisplayName("Confirm password")]
        [Password]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public static class ErrorMessages
        {
            public const string PasswordRequired = "Enter a password";
            public const string PasswordMismatch = "Passwords do not match";
        }
    }
}
