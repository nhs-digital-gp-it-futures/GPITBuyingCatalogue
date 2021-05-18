using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
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
            Organisation = organisation;
            CatalogueAgreementSigned = organisation.CatalogueAgreementSigned;
            BackLink = $"/admin/organisations/{organisation.OrganisationId}";
        }

        public Organisation Organisation { get; set; }

        public Address OrganisationAddress
        {
            get { return Organisation.GetAddress(); }
        }

        public bool CatalogueAgreementSigned { get; set; }
    }
}
