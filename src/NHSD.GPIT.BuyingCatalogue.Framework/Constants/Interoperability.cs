using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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

        public static Dictionary<string, string> Im1Integrations => new()
        {
            { "Bulk", "IM1 Bulk" },
            { "Transactional", "IM1 Transactional" },
            { "Patient Facing", "IM1 Patient Facing" },
        };

        public static Dictionary<string, string> GpConnectIntegrations => new()
        {
            { "Access Record HTML", "GP Connect - Access Record HTML" },
            { "Appointment Management", "GP Connect - Appointment Management" },
            { "Access Record Structured", "GP Connect - Access Record Structured" },
            { "Send Document", "GP Connect - Send Document" },
            { "Update Record", "GP Connect - Update Record" },
        };

        public static Dictionary<string, string> NhsAppIntegrationDescriptions => new()
        {
            {
                "Online consultations",
                "Local integrations that allow NHS App users to submit medical or admin queries to their GP surgery through an online questionnaire."
            },
            {
                "Personal health records/care plans",
                "Local integrations for primary and secondary care that allow NHS App users to manage their own health and care by accessing messages, letters, test results and allowing users to upload their own information."
            },
            {
                "Primary care notifications and messaging",
                "An integration that allows this solution to use the NHS App messaging service to send notifications and messages to users."
            },
            {
                "Secondary care notifications and messaging",
                "An integration that allows this solution to use the NHS App messaging service to send secondary care appointment related messages to users."
            },
        };

        public static IEnumerable<string> NhsAppIntegrations => NhsAppIntegrationDescriptions.Keys.Order();
    }
}
