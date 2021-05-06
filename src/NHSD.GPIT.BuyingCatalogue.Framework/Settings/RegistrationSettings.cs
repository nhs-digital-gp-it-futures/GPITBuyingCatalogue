using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    /// <summary>
    /// Registration settings.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class RegistrationSettings
    {
        /// <summary>
        /// Gets or sets the sender, subject and content of
        /// the e-mail message to send to a new user.
        /// </summary>
        public EmailMessageTemplate EmailMessage { get; set; }
    }
}
