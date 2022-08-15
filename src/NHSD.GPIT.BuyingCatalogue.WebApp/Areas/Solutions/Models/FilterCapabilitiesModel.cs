using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class FilterCapabilitiesModel
    {
        public const string FilterDelimiter = "|";

        private readonly Dictionary<int, IOrderedEnumerable<Capability>> capabilitiesByCategory = new();

        public FilterCapabilitiesModel()
        {
        }

        public FilterCapabilitiesModel(List<Capability> capabilities, string selectedIds = null)
        {
            if (capabilities == null)
            {
                throw new ArgumentNullException(nameof(capabilities));
            }

            Categories = capabilities
                .Select(x => x.Category.Id)
                .Distinct()
                .Select(x => capabilities.First(c => c.Category.Id == x).Category)
                .OrderBy(x => x.Name);

            capabilitiesByCategory = Categories.ToDictionary(
                x => x.Id,
                x => capabilities.Where(c => c.Category.Id == x.Id).OrderBy(c => c.Name));

            var selected = selectedIds?.Split(FilterDelimiter)
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse) ?? Enumerable.Empty<int>();

            Items = capabilities.Select(x => new SelectionModel
            {
                Id = x.Id,
                Selected = selected.Contains(x.Id),
            }).ToArray();

            TotalCapabilities = capabilities.Count;
        }

        public IEnumerable<CapabilityCategory> Categories { get; set; }

        public SelectionModel[] Items { get; set; }

        public int TotalCapabilities { get; set; }

        public List<Capability> Capabilities(int categoryId) => capabilitiesByCategory.ContainsKey(categoryId)
            ? capabilitiesByCategory[categoryId].ToList()
            : new List<Capability>();
    }
}
