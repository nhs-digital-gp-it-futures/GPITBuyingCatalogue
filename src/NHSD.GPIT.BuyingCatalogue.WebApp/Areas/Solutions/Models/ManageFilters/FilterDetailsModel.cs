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

            var capabilities = Filter.FilterCapabilities.Select(x => x.Capability).ToList();
            var epics = Filter.FilterEpics != null ? Filter.FilterEpics.Select(x => x.Epic).ToList() : new List<Epic>();

            GroupedCapabilities = capabilities.ToDictionary(
                x => x.Name,
                x => epics.Where(c => c.Capability.Id == x.Id).OrderBy(c => c.Name).ToList().AsEnumerable<Epic>());
        }

        public string OrganisationName { get; set; }

        public int FilterId { get; set; }

        public Filter Filter { get; init; }

        public Dictionary<string, IEnumerable<Epic>> GroupedCapabilities { get; init; }

        public bool FilterContainsEpics => Filter.FilterEpics != null && Filter.FilterEpics.Any();

        public bool FilterContainsAdditionalFilters =>
            (Filter.FilterClientApplicationTypes != null && Filter.FilterClientApplicationTypes.Any()) ||
            (Filter.Framework != null && !string.IsNullOrEmpty(Filter.Framework.ShortName)) ||
            (Filter.FilterHostingTypes != null && Filter.FilterHostingTypes.Any());
    }
}
