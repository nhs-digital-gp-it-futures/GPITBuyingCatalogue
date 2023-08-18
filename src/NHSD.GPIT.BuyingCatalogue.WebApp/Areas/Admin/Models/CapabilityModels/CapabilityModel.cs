using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels
{
    public sealed class CapabilityModel
    {
        public CapabilityModel()
        {
        }

        public CapabilityModel(
            CatalogueItem catalogueItem,
            Capability capability)
        {
            Id = capability.Id;
            Name = capability.NameWithStatusSuffix;
            Status = capability.Status;
            CapabilityRef = capability.CapabilityRef;
            Selected = catalogueItem.CatalogueItemCapabilities.Any(
                itemCapability => itemCapability.CapabilityId == capability.Id);
            MayEpics = GetCapabilityEpicModels(catalogueItem, capability, capability.GetAllMayEpics());
            MustEpics = GetCapabilityEpicModels(catalogueItem, capability, capability.GetAllMustEpics());

            if (capability.Status == CapabilityStatus.Effective && !Selected)
                MustEpics.ForEach(e => e.Selected = true);
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public CapabilityStatus Status { get; set; }

        public string CapabilityRef { get; set; }

        public bool Selected { get; set; }

        public IList<CapabilityEpicModel> MustEpics { get; set; } = new List<CapabilityEpicModel>();

        public IList<CapabilityEpicModel> MayEpics { get; set; } = new List<CapabilityEpicModel>();

        public IEnumerable<CapabilityEpicModel> Epics => MustEpics.Concat(MayEpics).ToList();

        private static IList<CapabilityEpicModel> GetCapabilityEpicModels(CatalogueItem catalogueItem, Capability capability, IReadOnlyCollection<Epic> epics)
        {
            return epics
                .Select(
                    epic => new CapabilityEpicModel(catalogueItem, capability, epic))
                .Where(e => e.IsActive || (!e.IsActive && e.Selected))
                .OrderBy(e => e.Id)
                .ToList();
        }
    }
}
