using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    /// <summary>
    /// Contact Us settings.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class ContactUsSettings
    {
        public string AdminTemplateId { get; set; }

        public string UserTemplateId { get; set; }

        public EmailAddressTemplate GeneralQueriesRecipient { get; set; }
    }
}
