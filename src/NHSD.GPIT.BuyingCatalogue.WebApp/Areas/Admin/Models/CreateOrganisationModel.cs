using System;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class CreateOrganisationModel : NavBaseModel
    {
        public CreateOrganisationModel()
        {
        }

        public CreateOrganisationModel(OdsOrganisation organisation)
        {
            OdsOrganisation = organisation ?? throw new ArgumentNullException(nameof(organisation));
            BackLink = $"/admin/organisations/find/select?ods={organisation.OdsCode}";
        }

        public OdsOrganisation OdsOrganisation { get; set; }

        public bool CatalogueAgreementSigned { get; set; }
    }
}
