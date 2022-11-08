using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels
{
    public class UsersModel : NavBaseModel
    {
        public int OrganisationId { get; init; }

        public string OrganisationName { get; init; }

        public IEnumerable<AspNetUser> Users { get; init; }

        public string HomeLink { get; set; }

        public string ControllerName { get; set; }
    }
}
