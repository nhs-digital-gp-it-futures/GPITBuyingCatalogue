using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;

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

    public static void RepopulateCapabilityCategories(EditCapabilitiesModel model, List<CapabilityCategory> capabilityCategories)
    {
        foreach (var capabilityCategory in model.CapabilityCategories)
        {
            var matchingCategory = capabilityCategories.FirstOrDefault(c => c.Id == capabilityCategory.Id);
            if (matchingCategory is not null)
            {
                capabilityCategory.Name = matchingCategory.Name;
                RepopulateCapabilities(matchingCategory, capabilityCategory);
            }
        }
    }

    private static void RepopulateCapabilities(CapabilityCategory matchingCategory, CapabilityCategoryModel capabilityCategory)
    {
        foreach (var capability in capabilityCategory.Capabilities)
        {
            var matchingCapability = matchingCategory.Capabilities.FirstOrDefault(c => c.Id == capability.Id);
            if (matchingCapability is not null)
            {
                capability.Name = matchingCapability.Name;
                RepopulateEpics(matchingCapability, capability);
            }
        }
    }

    private static void RepopulateEpics(Capability matchingCapability, CapabilityModel capabilityModel)
    {
        foreach (var epic in capabilityModel.Epics)
        {
            var matchingEpic = matchingCapability.Epics.FirstOrDefault(e => e.Id == epic.Id);
            if (matchingEpic is not null)
            {
                epic.Name = matchingEpic.Name;
            }
        }
    }

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
