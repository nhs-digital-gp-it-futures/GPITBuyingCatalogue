using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    /// <summary>
    /// Password reset settings.
    /// </summary>
    public sealed class PasswordResetSettings
    {
        /// <summary>
        /// Gets or sets the sender, subject and content of
        /// the password reset e-mail message.
        /// </summary>
        public EmailMessageTemplate EmailMessageTemplate { get; set; }
    }
}
