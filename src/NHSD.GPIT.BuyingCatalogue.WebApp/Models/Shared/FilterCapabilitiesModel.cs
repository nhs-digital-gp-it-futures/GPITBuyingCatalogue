using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared
{
    public class FilterCapabilitiesModel : FilterModel<string, IdAndNameModel<int>>
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

        public void PopulateCapabilities(List<Capability> capabilities, ICollection<int> selected = null)
        {
            if (selected == null)
                selected = new List<int>();
            Groups = capabilities
                .Select(x => new IdAndNameModel<int> { Id = x.Category.Id, Name = x.Category.Name })
                .DistinctBy(x => x.Id)
                .OrderBy(x => x.Name)
                .ToList();

            GroupedItems = Groups.ToDictionary(
                x => x.Id,
                x => capabilities.Where(c => c.Category.Id == x.Id).OrderBy(c => c.Name).Select(c => c.Name).ToList());

            SelectedItems = capabilities.Select(x => new SelectionModel
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
