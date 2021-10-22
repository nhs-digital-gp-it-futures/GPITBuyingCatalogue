using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels
{
    public sealed class CapabilityCategoryModel
    {
        public string Name { get; init; }

        public string Description { get; init; }

        public IReadOnlyList<CapabilityModel> Capabilities { get; init; } = new List<CapabilityModel>();
    }
}
