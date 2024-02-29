using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared
{
    public class FilterCapabilitiesModel
    {
        public static readonly PageTitleModel SupplierDefinedEpicPageTitle = new()
        {
            Title = "Capabilities for this supplier defined Epic",
            Advice = "Select the Capabilities relating to this supplier defined Epic.",
        };

        public static readonly PageTitleModel FilterPageTitle = new()
        {
            Title = "Select Capabilities",
            Advice = "Select Capabilities and apply them as a filter.",
        };

        public FilterCapabilitiesModel()
        {
        }

        public FilterCapabilitiesModel(
            List<Capability> capabilities,
            ICollection<int> selected)
        {
            PopulateCapabilities(capabilities, selected);
        }

        public string BackLink { get; set; }

        public NavBaseModel NavModel => new NavBaseModel() { BackLink = BackLink };

        public bool IsFilter { get; set; }

        public List<IdAndNameModel<int>> CapabilityGroups { get; set; }

        public SelectionModel[] CapabilitySelectionItems { get; set; }

        public Dictionary<int, List<string>> CapabilityGroupsAndItems { get; set; } = new();

        public List<string> Items(int groupId) => CapabilityGroupsAndItems.ContainsKey(groupId)
            ? CapabilityGroupsAndItems[groupId]
            : new List<string>();

        public void PopulateCapabilities(List<Capability> capabilities, ICollection<int> selected = null)
        {
            if (selected == null)
                selected = new List<int>();
            CapabilityGroups = capabilities
                .Select(x => new IdAndNameModel<int> { Id = x.Category.Id, Name = x.Category.Name })
                .DistinctBy(x => x.Id)
                .OrderBy(x => x.Name)
                .ToList();

            CapabilityGroupsAndItems = CapabilityGroups.ToDictionary(
                x => x.Id,
                x => capabilities.Where(c => c.Category.Id == x.Id).OrderBy(c => c.Name).Select(c => c.Name).ToList());

            CapabilitySelectionItems = capabilities.Select(x => new SelectionModel
            {
                Id = $"{x.Id}",
                Selected = selected.Contains(x.Id),
            }).ToArray();
        }

        public PageTitleModel GetPageTitle() => IsFilter
            ? FilterPageTitle
            : SupplierDefinedEpicPageTitle;
    }
}
