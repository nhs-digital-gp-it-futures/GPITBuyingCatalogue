using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class UserDetailsModel : NavBaseModel
    {
        public UserDetailsModel()
        {
        }

        public UserDetailsModel(Organisation organisation, AspNetUser user)
        {
            Organisation = organisation;
            User = user;
            BackLink = $"/admin/organisations/{organisation.OrganisationId}";
        }
        
        public AspNetUser User { get; set; }  
        
        public Organisation Organisation { get; set; }

        public string Toggle
        {
            get { return User.Disabled ? "enable" : "disable"; }
        }

        public string ToggleText
        {
            get { return User.Disabled ? "Re-enable account" : "Disable account"; }
        }
    }
}
