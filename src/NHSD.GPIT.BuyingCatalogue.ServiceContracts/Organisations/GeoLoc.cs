using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    [ExcludeFromCodeCoverage]
    public sealed class GeoLoc
    {
        public OdsResponseAddress Location { get; init; }
    }
}
