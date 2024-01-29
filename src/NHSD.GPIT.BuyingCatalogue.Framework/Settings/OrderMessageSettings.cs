using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    [ExcludeFromCodeCoverage]
    public sealed class OrderMessageSettings
    {
        public string SingleCsvTemplateId { get; set; }

        public string UserTemplateId { get; set; }

        public string UserAssociatedServiceTemplateId { get; set; }

        public string UserAmendTemplateId { get; set; }

        public string OrderTerminatedAdminTemplateId { get; set; }

        public string OrderTerminatedUserTemplateId { get; set; }

        public EmailAddressTemplate Recipient { get; set; }
    }
}
