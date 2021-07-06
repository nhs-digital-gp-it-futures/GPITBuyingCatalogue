using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class UserEnablingModel : NavBaseModel
    {
        public UserEnablingModel(Organisation organisation, AspNetUser user)
        {
            Organisation = organisation;
            User = user;
            BackLink = $"/admin/organisations/{organisation.OrganisationId}/{user.Id}";
        }

        public AspNetUser User { get; set; }

        public Organisation Organisation { get; set; }
    }
}
