using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class RemoveAnOrganisationModel : NavBaseModel
    {
        public RemoveAnOrganisationModel()
        {
        }

        public RemoveAnOrganisationModel(int organisationId, Organisation relatedOrganisation)
        {
            OrganisationId = organisationId;
            RelatedOrganisation = relatedOrganisation;
            BackLink = $"/admin/organisations/{organisationId}";
        }

        public int OrganisationId { get; set; }

        public Organisation RelatedOrganisation { get; set; }

        public List<Organisation> AvailableOrganisations { get; set; }

        public int SelectedOrganisation { get; set; }
    }
}
