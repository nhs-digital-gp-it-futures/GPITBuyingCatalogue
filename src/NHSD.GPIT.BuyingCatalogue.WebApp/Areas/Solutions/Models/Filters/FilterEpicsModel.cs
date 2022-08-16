using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;

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
            string selectedIds = null)
        {
            Groups = capabilities;
            CapabilityIds = string.Join(FilterConstants.Delimiter, Groups.Select(x => x.Id));

            GroupedItems = Groups.ToDictionary(
                x => x.Id,
                x => epics.Where(c => c.Capability.Id == x.Id).OrderBy(c => c.Name));

            var selected = selectedIds?.Split(FilterConstants.Delimiter) ?? Enumerable.Empty<string>();

            SelectedItems = epics.Select(x => new SelectionModel
            {
                Id = x.Id,
                Selected = selected.Contains(x.Id),
            }).ToArray();

            Total = epics.Count;
        }

        public string CapabilityIds { get; set; }
    }
}
