using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    [ExcludeFromCodeCoverage]
    public sealed class OdsResponseRole
    {
        public string Id { get; init; }

        public bool PrimaryRole { get; init; }
    }
}
