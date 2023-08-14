using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels
{
    public sealed class CapabilityEpicModel
    {
        public CapabilityEpicModel()
        {
        }

        public CapabilityEpicModel(
            CatalogueItem catalogueItem,
            Capability capability,
            Epic epic)
        {
            Id = epic.Id;
            Name = epic.Name;
            IsActive = epic.IsActive;

            Selected = catalogueItem.CatalogueItemEpics
                .Where(itemEpic => itemEpic.CapabilityId == capability.Id)
                .Any(
                    itemEpic => string.Equals(
                        itemEpic.EpicId,
                        epic.Id,
                        StringComparison.CurrentCultureIgnoreCase));
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public bool Selected { get; set; }
    }
}
