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
            List<Filter> filters,
            string organisationName)
        {
            Filters = filters;
            OrganisationName = organisationName;
        }

        public string OrganisationName { get; set; }

        public List<Filter> Filters { get; init; }
    }
}
