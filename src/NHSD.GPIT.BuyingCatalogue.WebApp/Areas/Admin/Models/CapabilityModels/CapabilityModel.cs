using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels
{
    public sealed class CapabilityModel
    {
        public int Id { get; init; }

        public string Name { get; init; }

        public bool Selected { get; init; }

        public IReadOnlyList<CapabilityEpicModel> Epics { get; init; } = new List<CapabilityEpicModel>();
    }
}
