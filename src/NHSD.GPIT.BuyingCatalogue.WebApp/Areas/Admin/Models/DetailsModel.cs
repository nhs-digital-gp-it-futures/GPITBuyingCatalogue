using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class DetailsModel : NavBaseModel
    {
        public DetailsModel()
        {
        }

        public DetailsModel(Organisation organisation)
        {
            Organisation = organisation;
            BackLink = "/admin/organisations";
        }

        public Organisation Organisation { get; set; }

        public Address OrganisationAddress { get { return Organisation.GetAddress(); } }
    }
}
