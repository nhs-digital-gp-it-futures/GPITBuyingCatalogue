using System;
using System.Collections.Generic;
using System.Linq;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels
{
    public sealed class EditCapabilitiesModel : NavBaseModel
    {
        public EditCapabilitiesModel()
        {
        }

        public EditCapabilitiesModel(CatalogueItem catalogueItem, IReadOnlyList<CapabilityCategory> capabilityCategories)
            : this()
        {
            CatalogueItemType = catalogueItem.CatalogueItemType.AsString(EnumFormat.DisplayName);

            Title = catalogueItem.CatalogueItemType == EntityFramework.Catalogue.Models.CatalogueItemType.Solution
                ? "Capabilities and Epics"
                : $"{catalogueItem.Name} Capabilities and Epics";

            CapabilityCategories = GetCapabilities(catalogueItem, capabilityCategories);

            if (!CapabilityCategories.Any(cc => cc.Capabilities.Any(c => c.Selected)))
                SelectMustEpics(CapabilityCategories);
        }

        public string Title { get; init; }

        public string SolutionName { get; init; }

        public string CatalogueItemType { get; init; }

        public IList<CapabilityCategoryModel> CapabilityCategories { get; init; }

        private static IList<CapabilityCategoryModel> GetCapabilities(CatalogueItem catalogueItem, IEnumerable<CapabilityCategory> capabilityCategories)
        {
            var categories = capabilityCategories.Where(cc => cc.Capabilities.Any())
                .Select(
                    cc => new CapabilityCategoryModel(
                        catalogueItem,
                        cc))
                .OrderBy(cc => cc.Name)
                .ToList();

            return categories;
        }

        private static void SelectMustEpics(IEnumerable<CapabilityCategoryModel> capabilityCategories)
        {
            foreach (var epic in capabilityCategories.SelectMany(cc => cc.Capabilities).SelectMany(c => c.MustEpics))
            {
                epic.Selected = true;
            }
        }
    }
}
