using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email
{
    /// <summary>
    /// Represents body of an e-mail message.
    /// </summary>
    public sealed class EmailMessageBody
    {
        private readonly List<object> formatItems = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailMessageBody"/> class
        /// with the specified <paramref name="content"/> and optional format items.
        /// </summary>
        /// <param name="content">The content of the message body.</param>
        /// <param name="formatItems">Any format items to format the content with.</param>
        public EmailMessageBody(string? content, params object[] formatItems)
        {
            Content = string.IsNullOrWhiteSpace(content) ? string.Empty : content;
            this.formatItems.AddRange(formatItems);
        }

        /// <summary>
        /// Gets the list of format items.
        /// </summary>
        public IReadOnlyList<object> FormatItems => formatItems;

        /// <summary>
        /// Gets the content of the message body.
        /// </summary>
        /// <remarks>Accepts format items; see <see cref="string.Format(string, object[])"/> for formatting
        /// options.</remarks>
        public string Content { get; }

        /// <summary>
        /// Returns a string that represents the content of the message.
        /// </summary>
        /// <returns>A string that represents the content of the message.</returns>
        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Content)
                ? string.Empty
                : string.Format(CultureInfo.CurrentCulture, Content, FormatItems.ToArray());
        }
    }
}
