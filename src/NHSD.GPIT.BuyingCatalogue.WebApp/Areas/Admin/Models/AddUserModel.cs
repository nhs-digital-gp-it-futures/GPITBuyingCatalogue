using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class AddUserModel : NavBaseModel
    {
        public AddUserModel()
        {
        }

        public AddUserModel(Organisation organisation)
        {
            Organisation = organisation;
            BackLink = $"/admin/organisations/{organisation.OrganisationId}";
        }

        public Organisation Organisation { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string TelephoneNumber { get; set; }

        public string EmailAddress { get; set; }
    }
}
