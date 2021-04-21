using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class NativeMobileModel
    { 
        public NativeMobileModel(CatalogueItem catalogueItem)
        {
            SolutionId = catalogueItem.CatalogueItemId;

            if (!string.IsNullOrWhiteSpace(catalogueItem?.Solution?.ClientApplication))
                ClientApplication = JsonConvert.DeserializeObject<ClientApplication>(catalogueItem.Solution.ClientApplication);
            else
                ClientApplication = new ClientApplication();
        }

        public string SolutionId { get; set; }

        private ClientApplication ClientApplication { get; set; }

        public string SupportedOperatingSystemsStatus
        {
            get { return "TODO"; }
        }

        public string MobileFirstApproachStatus
        {
            get { return "TODO"; }
        }

        public string ConnectivityStatus
        {
            get { return "TODO"; }
        }

        public string MemoryAndStorageStatus
        {
            get { return "TODO"; }
        }

        public string ThirdPartyStatus
        {
            get { return "TODO"; }
        }

        public string HardwareRequirementsStatus
        {
            get { return "TODO"; }
        }

        public string AdditionalInformationStatus
        {
            get { return "TODO"; }
        }
    }
}
