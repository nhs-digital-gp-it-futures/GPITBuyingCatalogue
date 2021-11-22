using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    /// <summary>
    /// SMTP server settings.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class SmtpSettings
    {
        /// <summary>
        /// Gets or sets the authentication settings for the SMTP server.
        /// </summary>
        public SmtpAuthenticationSettings Authentication { get; set; } = new();

        /// <summary>
        /// Gets or sets the host name of the SMTP server.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the port to use to connect to the SMTP server.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow connections
        /// to an SMTP server that does not present a valid/trusted certificate.
        /// </summary>
        /// <remarks>This should only be enabled in test environments.</remarks>
        public bool? AllowInvalidCertificate { get; set; }

        /// <summary>
        /// Gets the value used to prefix the subject in e-mails.
        /// </summary>
        public string EmailSubjectPrefix { get; init; }

        public string SenderAddress { get; set; }
    }
}
