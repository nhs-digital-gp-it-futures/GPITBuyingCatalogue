using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.OrganisationModels
{
    public class RelatedOrganisationsModel
    {
        public int OrganisationId { get; init; }

        public string OrganisationName { get; init; }

        public IEnumerable<Organisation> RelatedOrganisations { get; init; }

        public string HomeLink { get; set; }
    }
}
