using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
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
            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/add-application-type";
        }

        public override bool IsComplete =>
            ClientApplication is null ? false : BrowserBasedModelComplete();

        public string SupportedBrowsersStatus => (ClientApplication?.SupportedBrowsersComplete()).ToStatus();

        public string MobileFirstApproachStatus => (ClientApplication?.MobileFirstDesignComplete()).ToStatus();

        public string PlugInsStatus => (ClientApplication?.PlugInsComplete()).ToStatus();

        public string ConnectivityStatus => (ClientApplication?.ConnectivityAndResolutionComplete()).ToStatus();

        public string HardwareRequirementsStatus => (ClientApplication?.HardwareRequirementsComplete()).ToStatus();

        public string AdditionalInformationStatus => (ClientApplication?.AdditionalInformationComplete()).ToStatus();

        private bool BrowserBasedModelComplete() =>
            SupportedBrowsersComplete() &&
            PlugInsComplete().GetValueOrDefault();

        private bool SupportedBrowsersComplete() =>
            ClientApplication.BrowsersSupported is not null &&
            ClientApplication.BrowsersSupported.Any() &&
            ClientApplication.MobileResponsive.HasValue;

        private bool? PlugInsComplete() => ClientApplication?.Plugins?.Required.HasValue;
    }
}
