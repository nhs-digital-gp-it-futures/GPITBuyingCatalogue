using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class UserEnablingModel : NavBaseModel
    {
        public UserEnablingModel(Organisation organisation, AspNetUser user)
        {
            Organisation = organisation;
            User = user;
        }

        public AspNetUser User { get; set; }

        public Organisation Organisation { get; set; }
    }
}
