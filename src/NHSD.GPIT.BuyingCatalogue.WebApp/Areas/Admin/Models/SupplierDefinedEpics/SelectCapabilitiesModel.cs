using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierDefinedEpics
{
    public class SelectCapabilitiesModel : NavBaseModel
    {
        public SelectCapabilitiesModel()
        {
        }

        public SelectCapabilitiesModel(List<Capability> capabilities, string selectedIds = null)
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

            Title = "Capabilities for this supplier defined Epic";
            Advice = "Select the Capabilities relating to this supplier defined Epic.";
        }

        public string Id { get; set; }

        public IEnumerable<CapabilityCategory> Groups { get; set; }

        public int Total { get; set; }

        public SelectionModel[] SelectedItems { get; set; }

        public int? SelectedCapabilityId { get; set; }

        [StringLength(500)]
        public string Name { get; set; }

        [StringLength(1500)]
        public string Description { get; set; }

        public bool? IsActive { get; set; }

        public IList<SelectOption<string>> Capabilities { get; set; }

        protected Dictionary<int, IOrderedEnumerable<Capability>> GroupedItems { get; set; } = new();

        public IList<SelectOption<string>> ActiveOptions => new List<SelectOption<string>>
        {
            new("Active", true.ToString()),
            new("Inactive", false.ToString()),
        };

        public List<Capability> Items(int groupId) => GroupedItems.ContainsKey(groupId)
            ? GroupedItems[groupId].ToList()
            : new List<Capability>();

        public SelectCapabilitiesModel WithSelectListCapabilities(List<Capability> capabilities)
        {
            Capabilities = capabilities
                .OrderBy(c => c.Name)
                .Select(c => new SelectOption<string>(c.Name, c.Id.ToString()))
                .ToList();

            return this;
        }
    }
}
