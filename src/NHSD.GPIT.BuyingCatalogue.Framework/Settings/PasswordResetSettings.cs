using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    /// <summary>
    /// Password reset settings.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class PasswordResetSettings
    {
        /// <summary>
        /// Gets or sets the sender, subject and content of
        /// the password reset e-mail message.
        /// </summary>
        public EmailMessageTemplate EmailMessageTemplate { get; set; }
    }
}
