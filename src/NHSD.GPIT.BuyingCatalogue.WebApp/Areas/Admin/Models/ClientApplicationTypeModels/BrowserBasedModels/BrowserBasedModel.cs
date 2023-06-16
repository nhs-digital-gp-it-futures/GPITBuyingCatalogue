using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.BrowserBasedModels
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

        public TaskProgress SupportedBrowsersStatus() => ApplicationTypes.SupportedBrowsersStatus();

        public TaskProgress PluginsStatus() => ApplicationTypes.PluginsStatus();

        public TaskProgress ConnectivityStatus() => ApplicationTypes.ConnectivityStatus();

        public TaskProgress HardwareRequirementsStatus() => ApplicationTypes.HardwareRequirementsStatus();

        public TaskProgress AdditionalInformationStatus() => ApplicationTypes.AdditionalInformationStatus();
    }
}
