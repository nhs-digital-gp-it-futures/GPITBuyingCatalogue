using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class SelectOrganisationModel : NavBaseModel
    {
        public SelectOrganisationModel()
        {
        }

        public SelectOrganisationModel(OdsOrganisation organisation)
        {
            OdsOrganisation = organisation;
            BackLink = "/admin/organisations/find";
        }

        public OdsOrganisation OdsOrganisation { get; set; }
    }
}
