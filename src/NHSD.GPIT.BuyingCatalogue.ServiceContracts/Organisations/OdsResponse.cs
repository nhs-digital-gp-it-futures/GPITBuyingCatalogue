using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    [ExcludeFromCodeCoverage]
    public sealed class OdsResponse
    {
        public OdsResponseOrganisation Organisation { get; init; }
    }

    [ExcludeFromCodeCoverage]
    public sealed class OdsResponseOrganisation
    {
        public string Name { get; init; }

        public string Status { get; init; }

        public GeoLoc GeoLoc { get; init; }

        public OdsResponseRoles Roles { get; init; }
    }

    [ExcludeFromCodeCoverage]
    public sealed class GeoLoc
    {
        public OdsResponseAddress Location { get; init; }
    }

    [ExcludeFromCodeCoverage]
    public sealed class OdsResponseAddress
    {
        public string AddrLn1 { get; init; }
        
        public string AddrLn2 { get; init; }

        public string AddrLn3 { get; init; }
        
        public string AddrLn4 { get; init; }

        public string Town { get; init; }

        public string County { get; init; }

        public string PostCode { get; init; }

        public string Country { get; init; }
    }

    [ExcludeFromCodeCoverage]
    public sealed class OdsResponseRoles
    {
        public List<OdsResponseRole> Role { get; init; }
    }

    [ExcludeFromCodeCoverage]
    public sealed class OdsResponseRole
    {
        public string id { get; init; }

        public bool primaryRole { get; init; }
    }
}
