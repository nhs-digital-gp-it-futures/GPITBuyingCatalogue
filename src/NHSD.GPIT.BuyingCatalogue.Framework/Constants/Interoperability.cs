using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Constants
{
    [ExcludeFromCodeCoverage]
    public static class Interoperability
    {
        public const string IM1IntegrationType = "IM1";

        public const string GpConnectIntegrationType = "GP Connect";

        public const string NhsAppIntegrationType = "NHS App";

        public const string Consumer = nameof(Consumer);

        public const string Provider = nameof(Provider);
    }
}
