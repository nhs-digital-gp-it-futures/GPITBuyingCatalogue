using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.BrowserBasedModels
{
    public class BrowserBasedModel : ApplicationTypeBaseModel
    {
        public BrowserBasedModel()
            : base(null)
        {
        }

        public BrowserBasedModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            ApplicationType = ClientApplicationType.BrowserBased;
        }

        public TaskProgress SupportedBrowsersStatus() => ClientApplicationProgress.SupportedBrowsersStatus();

        public TaskProgress PluginsStatus() => ClientApplicationProgress.PluginsStatus();

        public TaskProgress ConnectivityStatus() => ClientApplicationProgress.ConnectivityStatus();

        public TaskProgress HardwareRequirementsStatus() => ClientApplicationProgress.HardwareRequirementsStatus();

        public TaskProgress AdditionalInformationStatus() => ClientApplicationProgress.AdditionalInformationStatus();
    }
}
