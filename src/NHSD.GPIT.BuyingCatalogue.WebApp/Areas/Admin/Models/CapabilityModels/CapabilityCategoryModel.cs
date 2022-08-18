using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels
{
    public sealed class CapabilityCategoryModel
    {
        public CapabilityCategoryModel()
        {
        }

        public CapabilityCategoryModel(
            CatalogueItem catalogueItem,
            CapabilityCategory capabilityCategory)
        {
            Name = capabilityCategory.Name;
            Capabilities = capabilityCategory.Capabilities.Select(c => new CapabilityModel(catalogueItem, c))
                .OrderBy(c => c.Id)
                .ToList();
        }

        public string Name { get; set; }

        public IList<CapabilityModel> Capabilities { get; set; } = new List<CapabilityModel>();
    }
}
