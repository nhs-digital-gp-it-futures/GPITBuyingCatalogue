using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.UserModels
{
    public class ResetPasswordModel : NavBaseModel
    {
        public int UserId { get; set; }

        public string Email { get; set; }
    }
}
