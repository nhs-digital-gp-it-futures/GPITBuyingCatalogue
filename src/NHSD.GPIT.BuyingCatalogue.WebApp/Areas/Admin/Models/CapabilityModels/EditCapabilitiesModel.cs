using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels
{
    public sealed class EditCapabilitiesModel : NavBaseModel
    {
        public EditCapabilitiesModel()
        {
        }

        public EditCapabilitiesModel(CatalogueItem catalogueItem, IEnumerable<CapabilityCategory> capabilityCategories)
            : this()
        {
            CatalogueItemType = catalogueItem.CatalogueItemType.Name();

            Title = catalogueItem.CatalogueItemType == EntityFramework.Catalogue.Models.CatalogueItemType.Solution
                ? "Capabilities and Epics"
                : $"{catalogueItem.Name} Capabilities and Epics";

            CapabilityCategories = GetCapabilities(catalogueItem, capabilityCategories);
        }

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
    }
}
