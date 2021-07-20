using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class AddUserModel : NavBaseModel
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
