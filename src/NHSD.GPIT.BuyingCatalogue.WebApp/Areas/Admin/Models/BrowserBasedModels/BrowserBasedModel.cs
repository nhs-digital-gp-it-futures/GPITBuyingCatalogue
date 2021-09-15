using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BrowserBasedModels
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
            var clientApplicationTypes = catalogueItem.Solution?.GetClientApplication()?.ClientApplicationTypes;

            if (clientApplicationTypes?.Any(type => type.Equals("browser-based", StringComparison.OrdinalIgnoreCase)) ?? false)
                BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type";
            else
                BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/add-application-type";

            ApplicationType = ClientApplicationType.BrowserBased;
        }

        public TaskProgress SupportedBrowsersStatus() => ClientApplication.SupportedBrowsersStatus();

        public TaskProgress PluginsStatus() => ClientApplication.PluginsStatus();

        public TaskProgress ConnectivityStatus() => ClientApplication.ConnectivityStatus();

        public TaskProgress HardwareRequirementsStatus() => ClientApplication.HardwareRequirementsStatus();

        public TaskProgress AdditionalInformationStatus() => ClientApplication.AdditionalInformationStatus();
    }
}
