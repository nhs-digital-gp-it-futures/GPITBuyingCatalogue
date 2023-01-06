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
    }
}
