using System;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels
{
    public sealed class CreateOrganisationModel : NavBaseModel
    {
        public CreateOrganisationModel()
        {
        }

        public CreateOrganisationModel(OdsOrganisation organisation)
        {
            OdsOrganisation = organisation ?? throw new ArgumentNullException(nameof(organisation));
        }

        public OdsOrganisation OdsOrganisation { get; set; }
    }
}
