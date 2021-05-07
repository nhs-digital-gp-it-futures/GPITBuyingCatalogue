using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class UserEnablingModel : NavBaseModel
    {
        public UserEnablingModel(Organisation organisation, AspNetUser user)
        {            
            User = user;
            BackLink = $"/admin/organisations/{organisation.OrganisationId}/{user.Id}";
        }
        
        public AspNetUser User { get; set; }                  
    }
}
