using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Constants
{
    public static class Interoperability
    {
        public const string IM1IntegrationType = "IM1";

        public const string GpConnectIntegrationType = "GP Connect";

        public const string Consumer = nameof(Consumer);

        public const string Provider = nameof(Provider);

        public static Dictionary<string, string> Im1Integrations => new()
        {
            { "Bulk", "IM1 Bulk" },
            { "Transactional", "IM1 Transactional" },
            { "Patient Facing", "IM1 Patient Facing" },
        };

        public static Dictionary<string, string> GpConnectIntegrations => new()
        {
            { "HTML View", "GP Connect - HTML View" },
            { "Appointment Booking", "GP Connect - Appointment Booking" },
            { "Structured Record", "GP Connect - Structured Record" },
        };
    }
}
