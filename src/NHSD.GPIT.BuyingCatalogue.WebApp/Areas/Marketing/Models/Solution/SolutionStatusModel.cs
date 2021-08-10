using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution
{
    public class SolutionStatusModel : MarketingBaseModel
    {
        public SolutionStatusModel()
            : base(null)
        {
        }

        public string CatalogueItemName { get; set; }

        public string SupplierName { get; set; }

        public bool IsBrowserBased => ClientApplication != null && ClientApplication.ClientApplicationTypes.Any(
            s => s.EqualsIgnoreCase("browser-based"));

        public bool IsNativeDesktop => ClientApplication != null && ClientApplication.ClientApplicationTypes.Any(
            s => s.EqualsIgnoreCase("native-desktop"));

        public bool IsNativeMobile => ClientApplication != null && ClientApplication.ClientApplicationTypes.Any(
            s => s.EqualsIgnoreCase("native-mobile"));

        public override bool? IsComplete => throw new NotImplementedException();

        public string SolutionDescriptionStatus { get; set; }

        public string FeaturesStatus { get; set; }

        public string IntegrationsStatus { get; set; }

        public string ImplementationTimescalesStatus { get; set; }

        public string RoadmapStatus { get; set; }

        public string ClientApplicationTypeStatus { get; set; }

        public string PublicCloudStatus { get; set; }

        public string PrivateCloudStatus { get; set; }

        public string HybridStatus { get; set; }

        public string OnPremisesStatus { get; set; }

        public string AboutSupplierStatus { get; set; }

        public string ContactDetailsStatus { get; set; }

        public string BrowserBasedStatus { get; set; }

        public string NativeDesktopStatus { get; set; }

        public string NativeMobileStatus { get; set; }
    }
}
