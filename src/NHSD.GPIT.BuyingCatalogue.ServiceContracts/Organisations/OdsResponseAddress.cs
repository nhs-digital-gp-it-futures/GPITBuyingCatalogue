using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
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
}
