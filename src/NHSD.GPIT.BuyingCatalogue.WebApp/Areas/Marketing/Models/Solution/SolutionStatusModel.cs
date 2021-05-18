using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution
{
    public class SolutionStatusModel : MarketingBaseModel
    {
        public SolutionStatusModel()
            : base(null)
        {
        }

        public SolutionStatusModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            CatalogueItemName = CatalogueItem.Name;
            SupplierName = CatalogueItem.Supplier.Name;
        }

        public string CatalogueItemName { get; set; }

        public string SupplierName { get; set; }

        public bool IsBrowserBased => ClientApplication.ClientApplicationTypes.Any(x =>
            x.Equals("browser-based", StringComparison.InvariantCultureIgnoreCase));

        public bool IsNativeMobile => ClientApplication.ClientApplicationTypes.Any(x =>
            x.Equals("native-mobile", StringComparison.InvariantCultureIgnoreCase));

        public bool IsNativeDesktop => ClientApplication.ClientApplicationTypes.Any(x =>
            x.Equals("native-desktop", StringComparison.InvariantCultureIgnoreCase));

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
