using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

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
            Dictionary<int, string[]> selected = null,
            string search = null)
        {
            selected ??= new Dictionary<int, string[]>();

            Groups = capabilities;
            Selected = selected.ToFilterString();
            ClearEpics = new Dictionary<int, string[]>(selected.Keys.Select(c => new KeyValuePair<int, string[]>(c, null)))
                .ToFilterString();

            GroupedItems = Groups.ToDictionary(
                x => x.Id,
                x => epics.Where(c => c.Capabilities.Any(y => y.Id == x.Id)).OrderBy(c => c.Name));

            SelectedItems = GroupedItems.SelectMany(
                kv => kv.Value.Select(
                    e => new SelectionModel
                    {
                        Id = $"{kv.Key},{e.Id}",
                        Selected = selected.GetValueOrDefault(kv.Key)?.Contains(e.Id) ?? false,
                    })).ToArray();

            EpicSelectedItemsMap = SelectedItems
                .Select((item, index) => new { item.Id, index })
                .ToDictionary(pair => pair.Id, pair => pair.index);

            Total = SelectedItems.Count();

            SearchTerm = search;
        }

        public string Selected { get; set; }

        public string ClearEpics { get; set; }

        public Dictionary<string, int> EpicSelectedItemsMap { get; set; }
    }
}
