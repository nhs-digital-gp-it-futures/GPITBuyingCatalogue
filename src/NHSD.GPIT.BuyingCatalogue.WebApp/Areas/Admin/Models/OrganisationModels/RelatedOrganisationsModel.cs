using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.OrganisationModels
{
    public class RelatedOrganisationsModel : NavBaseModel
    {
        public int OrganisationId { get; init; }

        public string OrganisationName { get; init; }

        public IEnumerable<Organisation> RelatedOrganisations { get; init; }
    }
}
