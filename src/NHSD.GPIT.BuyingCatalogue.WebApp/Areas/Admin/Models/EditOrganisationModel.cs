using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class EditOrganisationModel : NavBaseModel
    {
        public EditOrganisationModel()
        {
        }

        public EditOrganisationModel(Organisation organisation)
        {
            CatalogueAgreementSigned = organisation.CatalogueAgreementSigned;
            BackLink = $"/admin/organisations/{organisation.OrganisationId}";
            Organisation = organisation;
            OrganisationAddress = organisation.Address;
        }

        public Organisation Organisation { get; set; }

        public Address OrganisationAddress { get; set; }

        public bool CatalogueAgreementSigned { get; set; }
    }
}
