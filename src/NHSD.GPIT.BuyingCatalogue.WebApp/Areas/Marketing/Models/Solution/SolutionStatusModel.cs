using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution
{
    public class SolutionStatusModel : MarketingBaseModel
    {
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

        public string SolutionDescriptionStatus => GetStatus(new SolutionDescriptionModel(CatalogueItem));

        public string FeaturesStatus => GetStatus(new FeaturesModel(CatalogueItem));

        public string IntegrationsStatus => GetStatus(new IntegrationsModel(CatalogueItem));

        public string ImplementationTimescalesStatus => GetStatus(new ImplementationTimescalesModel(CatalogueItem));

        public string RoadmapStatus => GetStatus(new RoadmapModel(CatalogueItem));

        public string ClientApplicationTypeStatus => GetStatus(new ClientApplicationTypesModel(CatalogueItem));

        public string PublicCloudStatus => GetStatus(new PublicCloudModel(CatalogueItem));

        public string PrivateCloudStatus => GetStatus(new PrivateCloudModel(CatalogueItem));

        public string HybridStatus => GetStatus(new HybridModel(CatalogueItem));

        public string OnPremisesStatus => GetStatus(new OnPremiseModel(CatalogueItem));

        public string AboutSupplierStatus => GetStatus(new AboutSupplierModel(CatalogueItem));

        public string ContactDetailsStatus => GetStatus(new ContactDetailsModel(CatalogueItem));

        public string BrowserBasedStatus => GetStatus(new BrowserBasedModel(CatalogueItem));

        public string NativeDesktopStatus => GetStatus(new NativeDesktopModel(CatalogueItem));

        public string NativeMobileStatus => GetStatus(new NativeMobileModel(CatalogueItem));
    }
}
