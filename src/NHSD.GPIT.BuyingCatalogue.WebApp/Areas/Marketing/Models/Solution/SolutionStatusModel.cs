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

        public bool IsBrowserBased
        {
            get { return ClientApplication.ClientApplicationTypes.Any(x => x.Equals("browser-based", StringComparison.InvariantCultureIgnoreCase)); }
        }

        public bool IsNativeMobile
        {
            get { return ClientApplication.ClientApplicationTypes.Any(x => x.Equals("native-mobile", StringComparison.InvariantCultureIgnoreCase)); }
        }

        public bool IsNativeDesktop
        {
            get { return ClientApplication.ClientApplicationTypes.Any(x => x.Equals("native-desktop", StringComparison.InvariantCultureIgnoreCase)); }
        }

        public override bool? IsComplete
        {
            get { throw new NotImplementedException(); }
        }

        public string SolutionDescriptionStatus
        {
            get { return GetStatus(new SolutionDescriptionModel(CatalogueItem)); }
        }

        public string FeaturesStatus
        {
            get { return GetStatus(new FeaturesModel(CatalogueItem)); }
        }

        public string IntegrationsStatus
        {
            get { return GetStatus(new IntegrationsModel(CatalogueItem)); }
        }

        public string ImplementationTimescalesStatus
        {
            get { return GetStatus(new ImplementationTimescalesModel(CatalogueItem)); }
        }

        public string RoadmapStatus
        {
            get { return GetStatus(new RoadmapModel(CatalogueItem)); }
        }

        public string ClientApplicationTypeStatus
        {
            get { return GetStatus(new ClientApplicationTypesModel(CatalogueItem)); }
        }

        public string PublicCloudStatus
        {
            get { return GetStatus(new PublicCloudModel(CatalogueItem)); }
        }

        public string PrivateCloudStatus
        {
            get { return GetStatus(new PrivateCloudModel(CatalogueItem)); }
        }

        public string HybridStatus
        {
            get { return GetStatus(new HybridModel(CatalogueItem)); }
        }

        public string OnPremisesStatus
        {
            get { return GetStatus(new OnPremiseModel(CatalogueItem)); }
        }

        public string AboutSupplierStatus
        {
            get { return GetStatus(new AboutSupplierModel(CatalogueItem)); }
        }

        public string ContactDetailsStatus
        {
            get { return GetStatus(new ContactDetailsModel(CatalogueItem)); }
        }

        public string BrowserBasedStatus
        {
            get { return GetStatus(new BrowserBasedModel(CatalogueItem));  }
        }

        public string NativeDesktopStatus
        {
            get { return GetStatus(new NativeDesktopModel(CatalogueItem)); }
        }

        public string NativeMobileStatus
        {
            get { return GetStatus(new NativeMobileModel(CatalogueItem)); }
        }
    }
}
