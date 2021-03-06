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

            CapabilityCategories = capabilityCategories.Where(cc => cc.Capabilities.Any()).Select(cc => new CapabilityCategoryModel
            {
                Name = cc.Name,
                Description = cc.Description,
                Capabilities = cc.Capabilities.Select(c => new CapabilityModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    CapabilityRef = c.CapabilityRef,
                    Selected = catalogueItem.CatalogueItemCapabilities.Any(itemCapability => itemCapability.CapabilityId == c.Id),
                    Epics = c.Epics.Select(epic => new CapabilityEpicModel
                    {
                        Id = epic.Id,
                        Name = epic.Name,
                        Selected = catalogueItem.CatalogueItemEpics
                            .Where(itemEpic => itemEpic.CapabilityId == c.Id)
                            .Any(itemEpic => string.Equals(itemEpic.EpicId, epic.Id, StringComparison.CurrentCultureIgnoreCase)),
                    }).OrderBy(e => e.Id).ToList(),
                }).OrderBy(c => c.Id).ToList(),
            }).OrderBy(cc => cc.Name).ToList();
        }

        public string Title { get; init; }

        public string SolutionName { get; init; }

        public string CatalogueItemType { get; init; }

        public IList<CapabilityCategoryModel> CapabilityCategories { get; init; }
    }
}
