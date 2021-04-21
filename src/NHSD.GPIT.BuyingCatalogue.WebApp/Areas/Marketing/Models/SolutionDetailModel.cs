using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class SolutionDetailModel
    {
        public SolutionDetailModel(CatalogueItem catalogueItem)
        {
            CatalogueItem = catalogueItem;
            ClientApplication = catalogueItem.Solution.GetClientApplication();
        }

        public CatalogueItem CatalogueItem { get; private set; }

        public ClientApplication ClientApplication { get; private set; }

        public string SolutionDescriptionStatus
        {
            get 
            { 
                return string.IsNullOrWhiteSpace(CatalogueItem.Solution.Summary) ? "INCOMPLETE" : "COMPLETE";
            }
        }

        public string FeaturesStatus
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CatalogueItem.Solution.Features))
                    return "INCOMPLETE";

                var features = CatalogueItem.Solution.GetFeatures();

                return features.Any() ? "COMPLETE" : "INCOMPLETE";
            }
        }

        public string IntegrationsStatus
        {
            get
            {
                return string.IsNullOrWhiteSpace(CatalogueItem.Solution.IntegrationsUrl) ? "INCOMPLETE" : "COMPLETE";
            }
        }

        public string ImplementationTimescalesStatus
        {
            get
            {
                return string.IsNullOrWhiteSpace(CatalogueItem.Solution.ImplementationDetail) ? "INCOMPLETE" : "COMPLETE";
            }
        }

        public string RoadmapStatus
        {
            get
            {
                return string.IsNullOrWhiteSpace(CatalogueItem.Solution.RoadMap) ? "INCOMPLETE" : "COMPLETE";
            }
        }

        public string ClientApplicationTypeStatus
        {
            get
            {
                return ClientApplication.ClientApplicationTypes.Any() ? "COMPLETE" : "INCOMPLETE";
            }
        }

        public string AboutSupplierStatus
        {
            get
            {
                if (string.IsNullOrWhiteSpace(CatalogueItem.Supplier.SupplierUrl) && string.IsNullOrWhiteSpace(CatalogueItem.Supplier.Summary))
                    return "INCOMPLETE";

                return "COMPLETE";
            }
        }

        public string ContactDetailsStatus
        {
            get
            {
                return CatalogueItem.Solution.MarketingContacts.Any() ? "COMPLETE" : "INCOMPLETE";
            }
        }
    }
}
