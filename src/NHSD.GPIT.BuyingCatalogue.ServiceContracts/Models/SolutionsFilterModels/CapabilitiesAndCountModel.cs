using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SolutionsFilterModels
{
    [ExcludeFromCodeCoverage]
    public sealed class CapabilitiesAndCountModel
    {
        public CapabilitiesAndCountModel()
        {
        }

        public CapabilitiesAndCountModel(int id, string name, int count)
        {
            CapabilityId = id;
            CapabilityName = name;
            CountOfEpics = count;
        }

        public int CapabilityId { get; init; }

        public string CapabilityName { get; init; }

        public int CountOfEpics { get; init; }
    }
}
