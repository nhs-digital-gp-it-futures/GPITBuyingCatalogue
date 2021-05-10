using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution
{
    public class SolutionStatusModel : MarketingBaseModel
    {
        public SolutionStatusModel() : base(null)
        {
        }
        
        public SolutionStatusModel(CatalogueItem catalogueItem) : base(catalogueItem)
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

        public string SolutionDescriptionStatus { get; set; } //=> GetStatus(new SolutionDescriptionModel(CatalogueItem));

        public string FeaturesStatus { get; set; } //=> GetStatus(new FeaturesModel(CatalogueItem));

        public string IntegrationsStatus { get; set; } //=> GetStatus(new IntegrationsModel(CatalogueItem));

        public string ImplementationTimescalesStatus { get; set; } //=> GetStatus(new ImplementationTimescalesModel(CatalogueItem));

        public string RoadmapStatus { get; set; } //=> GetStatus(new RoadmapModel(CatalogueItem));

        public string ClientApplicationTypeStatus { get; set; } //=> GetStatus(new ClientApplicationTypesModel(CatalogueItem));

        public string PublicCloudStatus { get; set; } //=> GetStatus(new PublicCloudModel(CatalogueItem));

        public string PrivateCloudStatus { get; set; } //=> GetStatus(new PrivateCloudModel(CatalogueItem));

        public string HybridStatus { get; set; } //=> GetStatus(new HybridModel(CatalogueItem));

        public string OnPremisesStatus { get; set; } //=> GetStatus(new OnPremiseModel(CatalogueItem));

        public string AboutSupplierStatus { get; set; } //=> GetStatus(new AboutSupplierModel(CatalogueItem));

        public string ContactDetailsStatus { get; set; } //=> GetStatus(new ContactDetailsModel(CatalogueItem));

        public string BrowserBasedStatus { get; set; } //=> GetStatus(new BrowserBasedModel(CatalogueItem));

        public string NativeDesktopStatus { get; set; } //=> GetStatus(new NativeDesktopModel(CatalogueItem));

        public string NativeMobileStatus { get; set; } //=> GetStatus(new NativeMobileModel(CatalogueItem));
    }
}
