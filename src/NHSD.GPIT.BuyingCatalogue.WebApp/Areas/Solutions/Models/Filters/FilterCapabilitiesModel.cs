using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    public class FilterCapabilitiesModel : FilterModel<Capability, CapabilityCategory>
    {
        public FilterCapabilitiesModel()
        {
        }

        public FilterCapabilitiesModel(List<Capability> capabilities, string selectedIds = null, string search = null)
        {
            Groups = capabilities
                .Select(x => x.Category.Id)
                .Distinct()
                .Select(x => capabilities.First(c => c.Category.Id == x).Category)
                .OrderBy(x => x.Name);

            GroupedItems = Groups.ToDictionary(
                x => x.Id,
                x => capabilities.Where(c => c.Category.Id == x.Id).OrderBy(c => c.Name));

            var selected = SolutionsFilterHelper.ParseCapabilityIds(selectedIds);

            SelectedItems = capabilities.Select(x => new SelectionModel
            {
                Id = $"{x.Id}",
                Selected = selected.Contains(x.Id),
            }).ToArray();

            Total = capabilities.Count;

            SearchTerm = search;
        }
    }
}
