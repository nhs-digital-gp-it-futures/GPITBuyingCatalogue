using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters
{
    public class ManageFiltersModel : NavBaseModel
    {
        public ManageFiltersModel()
        {
        }

        public ManageFiltersModel(
            string internalOrgId,
            List<Filter> shortlists,
            string organisationName)
        {
            InternalOrgId = internalOrgId;
            Shortlists = shortlists;
            OrganisationName = organisationName;
        }

        public string InternalOrgId { get; set; }

        public string OrganisationName { get; set; }

        public List<Filter> Shortlists { get; init; }
    }
}
