using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    public class FilterEpicsModel : FilterModel<Epic, Capability>
    {
        public FilterEpicsModel()
        {
        }

        public FilterEpicsModel(
            List<Capability> capabilities,
            List<Epic> epics,
            string selectedIds = null,
            string search = null)
        {
            Groups = capabilities;
            CapabilityIds = Groups.Select(x => x.Id).ToFilterString();

            GroupedItems = Groups.ToDictionary(
                x => x.Id,
                x => epics.Where(c => c.Capability.Id == x.Id).OrderBy(c => c.Name));

            var selected = SolutionsFilterHelper.ParseEpicIds(selectedIds);

            SelectedItems = epics.Select(x => new SelectionModel
            {
                Id = x.Id,
                Selected = selected.Contains(x.Id),
            }).ToArray();

            EpicSelectedItemsMap = SelectedItems
                .Select((item, index) => new { item.Id, index })
                .ToDictionary(pair => pair.Id, pair => pair.index);

            Total = epics.Count;

            SearchTerm = search;
        }

        public string CapabilityIds { get; set; }

        public Dictionary<string, int> EpicSelectedItemsMap { get; set; }
    }
}
