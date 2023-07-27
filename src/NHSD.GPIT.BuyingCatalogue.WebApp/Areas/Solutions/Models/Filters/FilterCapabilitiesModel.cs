using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    public class FilterCapabilitiesModel : FilterModel<Capability, CapabilityCategory>
    {
        public FilterCapabilitiesModel()
        {
        }

        public FilterCapabilitiesModel(
            List<Capability> capabilities,
            Dictionary<int, string[]> selected,
            string search = null)
        {
            if (selected == null)
            {
                selected = new Dictionary<int, string[]>();
            }

            Groups = capabilities
                .Select(x => x.Category.Id)
                .Distinct()
                .Select(x => capabilities.First(c => c.Category.Id == x).Category)
                .OrderBy(x => x.Name);

            GroupedItems = Groups.ToDictionary(
                x => x.Id,
                x => capabilities.Where(c => c.Category.Id == x.Id).OrderBy(c => c.Name));

            SelectedItems = capabilities.Select(x => new SelectionModel
            {
                Id = $"{x.Id}",
                Selected = selected.Keys.Contains(x.Id),
            }).ToArray();

            Total = capabilities.Count;

            SearchTerm = search;
        }
    }
}
