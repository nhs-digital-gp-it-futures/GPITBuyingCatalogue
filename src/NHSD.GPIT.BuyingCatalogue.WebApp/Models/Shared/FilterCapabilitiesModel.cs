using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared
{
    public class FilterCapabilitiesModel : FilterModel<Capability, CapabilityCategory>
    {
        public static readonly PageTitleModel SupplierDefinedEpicPageTitle = new()
        {
            Title = "Capabilities for this supplier defined Epic",
            Advice = "Select the Capabilities relating to this supplier defined Epic.",
        };

        public static readonly PageTitleModel FilterPageTitle = new()
        {
            Title = "Filter by Capabilities for Catalogue Solutions",
            Advice = "Capabilities describe business needs. Select the ones you want a Catalogue Solution to address.",
        };

        public FilterCapabilitiesModel()
        {
        }

        public FilterCapabilitiesModel(
            List<Capability> capabilities,
            bool isFilter,
            ICollection<int> selected,
            string search = null)
        {
            if (selected == null)
                selected = new List<int>();

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
                Selected = selected.Contains(x.Id),
            }).ToArray();

            Total = capabilities.Count;

            SearchTerm = search;

            IsFilter = isFilter;
        }

        public string BackLink { get; set; }

        public NavBaseModel NavModel => string.IsNullOrEmpty(BackLink)
            ? null
            : new NavBaseModel() { BackLink = BackLink };

        public bool IsFilter { get; set; }

        public static FilterCapabilitiesModel Build(List<Capability> capabilities, string selected = null, string search = null)
        {
            var capabilityAndEpicsIds = SolutionsFilterHelper.ParseCapabilityAndEpicIds(selected);
            return new(capabilities, true, capabilityAndEpicsIds.Keys, search);
        }

        public PageTitleModel GetPageTitle() => IsFilter
            ? FilterPageTitle
            : SupplierDefinedEpicPageTitle;
    }
}
