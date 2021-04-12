using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MailKit;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.Services.Email
{
    /// <summary>
    /// A service for sending e-mails using MailKit.
    /// </summary>
    public sealed class MailKitEmailService : IEmailService
    {
        private readonly IMailTransport client;
        private readonly ILogger<MailKitEmailService> logger;
        private readonly SmtpSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="MailKitEmailService" /> class
        /// using the provided <paramref name="client" /> and <paramref name="settings" />.
        /// </summary>
        /// <param name="client">The mail transport to use to send e-mail.</param>
        /// <param name="settings">The SMTP configuration.</param>
        /// <param name="logger">logger for log messages.</param>
        /// <exception cref="ArgumentNullException"><paramref name="client" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="settings" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="logger" /> is <see langword="null" />.</exception>
        [SuppressMessage(
            "Security",
            "CA5359:Do Not Disable Certificate Validation",
            Justification = "Certificate validation only disabled when specified in configuration (for use in test environments only)")]
        public MailKitEmailService(IMailTransport client, SmtpSettings settings, ILogger<MailKitEmailService> logger)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.settings = settings ?? throw new ArgumentNullException(nameof(client));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (settings.AllowInvalidCertificate.GetValueOrDefault())
                client.ServerCertificateValidationCallback = (_, _, _, _) => true;
        }

        /// <summary>
        /// Sends an e-mail message asynchronously using the
        /// provided <see cref="IMailTransport" /> instance.
        /// </summary>
        /// <param name="emailMessage">The e-mail message to send asynchronously.</param>
        /// <returns>An asynchronous <see cref="Task"/> context.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="emailMessage" /> is
        /// <see langword="null" />.</exception>
        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            if (emailMessage is null)
                throw new ArgumentNullException(nameof(emailMessage));

            await client.ConnectAsync(settings.Host, settings.Port).ConfigureAwait(false);

            try
            {
                var authentication = settings.Authentication;
                if (authentication.IsRequired)
                    await client.AuthenticateAsync(authentication.UserName, authentication.Password).ConfigureAwait(false);

                var mimeMessage = emailMessage.AsMimeMessage(settings.EmailSubjectPrefix);
                await client.SendAsync(mimeMessage).ConfigureAwait(false);

                logger.LogInformation(
                    "SendEmailAsync: Sent: {server}:{port} with auth required {isRequired} and U:{user}, Sent Message {mimeMessage}",
                    settings.Host,
                    settings.Port,
                    authentication.IsRequired,
                    authentication.UserName,
                    mimeMessage);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "SendEmailAsync: Failed: {server}:{port} with auth required {isRequired} and U:{user}, Sending Message {@mimeMessage}",
                    settings.Host,
                    settings.Port,
                    settings.Authentication.IsRequired,
                    settings.Authentication.UserName,
                    emailMessage);

                throw;
            }
            finally
            {
                if (client.IsConnected)
                    await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}
