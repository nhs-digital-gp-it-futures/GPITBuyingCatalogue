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
            ApplicationType = ApplicationType.BrowserBased;
        }

        public TaskProgress SupportedBrowsersStatus() => ClientApplication.SupportedBrowsersStatus();

        public TaskProgress PluginsStatus() => ClientApplication.PluginsStatus();

        public TaskProgress ConnectivityStatus() => ClientApplication.ConnectivityStatus();

        public TaskProgress HardwareRequirementsStatus() => ClientApplication.HardwareRequirementsStatus();

        public TaskProgress AdditionalInformationStatus() => ClientApplication.AdditionalInformationStatus();
    }
}
