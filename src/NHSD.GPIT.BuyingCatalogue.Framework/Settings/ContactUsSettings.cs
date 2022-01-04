using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    /// <summary>
    /// Contact Us settings.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class ContactUsSettings
    {
        /// <summary>
        /// Gets or sets the sender, subject and content of
        /// the e-mail message to send to the configured users.
        /// </summary>
        public EmailMessageTemplate EmailMessage { get; set; }

        public string TechnicalFaultAddress { get; set; }

        public string OtherQueriesAddress { get; set; }
    }
}
