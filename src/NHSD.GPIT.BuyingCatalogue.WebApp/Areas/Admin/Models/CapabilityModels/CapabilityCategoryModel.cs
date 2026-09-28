using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;

public sealed class CapabilityCategoryModel
{
    public CapabilityCategoryModel()
    {
    }

    public CapabilityCategoryModel(
        CatalogueItem catalogueItem,
        CapabilityCategory capabilityCategory)
    {
        Id = capabilityCategory.Id;
        Name = capabilityCategory.Name;
        Capabilities = [.. capabilityCategory.Capabilities.Select(c => new CapabilityModel(catalogueItem, c))
            .Where(c => c.Status == CapabilityStatus.Effective
                || (c.Status == CapabilityStatus.Expired && (c.Selected || c.Epics.Any(e => e.Selected))))
            .OrderBy(c => c.Id)];
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public IList<CapabilityModel> Capabilities { get; set; } = [];
}
