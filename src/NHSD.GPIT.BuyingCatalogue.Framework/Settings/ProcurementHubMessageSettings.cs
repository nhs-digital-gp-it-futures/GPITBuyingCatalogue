using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Settings
{
    public class ProcurementHubMessageSettings
    {
        public string TemplateId { get; set; }

        public EmailAddressTemplate Recipient { get; set; }
    }
}
