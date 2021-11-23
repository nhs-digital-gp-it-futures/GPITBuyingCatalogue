using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class UserDetailsModel : NavBaseModel
    {
        public UserDetailsModel()
        {
        }

        public UserDetailsModel(Organisation organisation, AspNetUser user)
        {
            Organisation = organisation;
            User = user;
        }

        public AspNetUser User { get; set; }

        public Organisation Organisation { get; set; }

        public string Toggle
        {
            get { return User.Disabled ? "UserEnabled" : "UserDisabled"; }
        }

        public string ToggleText
        {
            get { return User.Disabled ? "Re-enable account" : "Disable account"; }
        }
    }
}
