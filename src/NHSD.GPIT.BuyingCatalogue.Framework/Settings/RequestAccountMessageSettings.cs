using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    [ExcludeFromCodeCoverage]
    public sealed class RequestAccountMessageSettings
    {
        public string AdminTemplateId { get; set; }

        public string UserTemplateId { get; set; }

        public EmailAddressTemplate AdminRecipient { get; set; }
    }
}
