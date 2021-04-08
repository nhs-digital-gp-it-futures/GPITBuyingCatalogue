using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email
{
    /// <summary>
    /// Defines a template that can be used to initialize a new <see cref="EmailMessage"/>.
    /// </summary>
    public sealed record EmailMessageTemplate
    {
        private EmailAddressTemplate? sender;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailMessageTemplate"/> class.
        /// </summary>
        /// <remarks>Required for deserialization.</remarks>
        public EmailMessageTemplate()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailMessageTemplate"/> class
        /// with the specified <paramref name="sender"/>.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        public EmailMessageTemplate(EmailAddressTemplate sender)
        {
            Sender = sender;
        }

        /// <summary>
        /// Gets or sets the sender (from address) of the message.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        public EmailAddressTemplate? Sender
        {
            get => sender;
            set
            {
                sender = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Gets the subject of the message.
        /// </summary>
        public string? Subject { get; init; }

        /// <summary>
        /// Gets the HTML body.
        /// </summary>
        public string? HtmlContent { get; init; }

        /// <summary>
        /// Gets the plain text body.
        /// </summary>
        public string? PlainTextContent { get; init; }

        /// <summary>
        /// Returns the <see cref="EmailAddress"/> representation of <see cref="Sender"/>.
        /// </summary>
        /// <returns>The <see cref="EmailAddress"/> representation of <see cref="Sender"/>.</returns>
        public EmailAddress? GetSenderAsEmailAddress() => Sender?.AsEmailAddress();
    }
}
