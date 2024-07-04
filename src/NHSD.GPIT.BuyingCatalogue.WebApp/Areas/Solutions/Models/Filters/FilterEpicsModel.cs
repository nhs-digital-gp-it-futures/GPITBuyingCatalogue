using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    public class FilterEpicsModel
    {
        public FilterEpicsModel()
        {
        }

        public FilterEpicsModel(
            List<Capability> orderedCapabilities,
            List<Epic> epics,
            Dictionary<int, string[]> selected = null,
            string search = null)
        {
            selected ??= new Dictionary<int, string[]>();

            EpicGroups = orderedCapabilities
                .Select(c => new IdAndNameModel<int> { Id = c.Id, Name = c.Name })
                .ToList();

            EpicGroupsAndItems = EpicGroups.ToDictionary(
                x => x.Id,
                x => epics
                .Where(e => e.Capabilities.Any(y => y.Id == x.Id))
                .OrderBy(e => e.Name)
                .Select(e => new IdAndNameModel<string>() { Id = e.Id, Name = e.Name })
                .ToList());

            EpicSelectionItems = EpicGroupsAndItems.SelectMany(
                kv => kv.Value.Select(
                    e => new SelectionModel
                    {
                        Id = $"{kv.Key},{e.Id}",
                        Selected = selected.GetValueOrDefault(kv.Key)?.Contains(e.Id) ?? false,
                    })).ToArray();
        }

        public List<IdAndNameModel<int>> EpicGroups { get; set; }

        public SelectionModel[] EpicSelectionItems { get; set; }

        public Dictionary<int, List<IdAndNameModel<string>>> EpicGroupsAndItems { get; set; } = new();

        public string BackLink { get; set; }

        public NavBaseModel NavModel => new NavBaseModel() { BackLink = BackLink };

        public List<IdAndNameModel<string>> Items(int groupId) => EpicGroupsAndItems.ContainsKey(groupId)
            ? EpicGroupsAndItems[groupId]
            : new List<IdAndNameModel<string>>();
    }
}
