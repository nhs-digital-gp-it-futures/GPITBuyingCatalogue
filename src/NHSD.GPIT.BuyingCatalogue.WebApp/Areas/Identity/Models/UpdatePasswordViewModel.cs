using System.Linq;
using Microsoft.AspNetCore.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Identity;
using NHSD.GPIT.BuyingCatalogue.UI.Components.DataAttributes;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Identity.Models
{
    public sealed class UpdatePasswordViewModel
    {
        [Password]
        public string CurrentPassword { get; set; }

        [Password]
        public string NewPassword { get; set; }

        [Password]
        public string ConfirmPassword { get; set; }

        public IdentityResult IdentityResult { get; set; }

        public bool PasswordUsedBefore => IdentityResult?.Errors.FirstOrDefault(error => error.Code == PasswordValidator.PasswordAlreadyUsedCode) != null;

        public bool IncorrectPasswordError => IdentityResult?.Errors.FirstOrDefault(error => error.Code == PasswordValidator.PasswordMismatchCode) != null;

        public bool InvalidPasswordError => IdentityResult?.Errors.FirstOrDefault(error => error.Code == PasswordValidator.InvalidPasswordCode) != null;
    }
}
