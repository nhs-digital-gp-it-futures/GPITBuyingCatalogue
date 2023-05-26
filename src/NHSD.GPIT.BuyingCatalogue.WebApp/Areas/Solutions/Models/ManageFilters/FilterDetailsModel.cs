using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters
{
    public class FilterDetailsModel : NavBaseModel
    {
        public FilterDetailsModel()
        {
        }

        public FilterDetailsModel(
            Filter filter,
            string organisationName)
        {
            Filter = filter;
            FilterId = filter.Id;

            OrganisationName = organisationName;

            GroupedCapabilities = Filter.Capabilities.ToDictionary(
                x => x.Name,
                x => Filter.Epics.Where(c => c.Capability.Id == x.Id).OrderBy(c => c.Name).ToList());
        }

        public string OrganisationName { get; set; }

        public int FilterId { get; set; }

        public Filter Filter { get; init; }

        public Dictionary<string, List<Epic>> GroupedCapabilities { get; init; }

        public bool FilterContainsEpics => Filter.Epics != null && Filter.Epics.Any();

        public bool FilterContainsAdditionalFilters =>
            (Filter.FilterClientApplicationTypes != null && Filter.FilterClientApplicationTypes.Any()) ||
            (Filter.Framework != null && !string.IsNullOrEmpty(Filter.Framework.ShortName)) ||
            (Filter.FilterHostingTypes != null && Filter.FilterHostingTypes.Any());
    }
}
