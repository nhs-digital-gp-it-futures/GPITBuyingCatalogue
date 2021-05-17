using MimeKit;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.Services.Email
{
    /// <summary>
    /// Extension methods for <see cref="MimeKit"/> classes.
    /// </summary>
    public static class MimeKitExtensions
    {
        /// <summary>
        /// Returns the receiver as a <see cref="MailboxAddress"/>.
        /// </summary>
        /// <param name="address">The receiving <see cref="EmailAddress"/> instance.</param>
        /// <returns>the corresponding <see cref="MailboxAddress"/>.</returns>
        public static MailboxAddress AsMailboxAddress(this EmailAddress address)
            => new(address.DisplayName, address.Address);

        /// <summary>
        /// Returns the receiver as a <see cref="MimeMessage"/>.
        /// </summary>
        /// <param name="emailMessage">The receiving <see cref="EmailMessage"/> instance.</param>
        /// <param name="emailSubjectPrefix">The text used as a prefix to the e-mail subject.</param>
        /// <returns>the corresponding <see cref="MimeMessage"/>.</returns>
        public static MimeMessage AsMimeMessage(
            this EmailMessage emailMessage,
            string emailSubjectPrefix = null)
        {
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = emailMessage.HtmlBody?.ToString(),
                TextBody = emailMessage.TextBody?.ToString(),
            };

            AddAttachment(bodyBuilder.Attachments, emailMessage);

            var message = new MimeMessage
            {
                Body = bodyBuilder.ToMessageBody(),
                Subject = $"{emailSubjectPrefix} {emailMessage.Subject}".Trim(),
            };

            message.From.Add(emailMessage.Sender.AsMailboxAddress());

            foreach (var recipient in emailMessage.Recipients)
            {
                message.To.Add(recipient.AsMailboxAddress());
            }

            return message;
        }

        private static void AddAttachment(AttachmentCollection attachments, EmailMessage emailMessage)
        {
            if (!emailMessage.HasAttachments)
                return;

            foreach (var attachment in emailMessage.Attachments)
            {
                attachments.Add(attachment.FileName, attachment.Content);
            }
        }
    }
}
