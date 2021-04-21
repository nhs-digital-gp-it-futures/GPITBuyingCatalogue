using System.Linq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class SolutionDetailModel
    {
        public SolutionDetailModel(CatalogueItem catalogueItem)
        {
            CatalogueItem = catalogueItem;

            DecodeClientApplication();
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
                
                var features = JsonConvert.DeserializeObject<string[]>(CatalogueItem.Solution.Features);

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

        private void DecodeClientApplication()
        {
            if (!string.IsNullOrWhiteSpace(CatalogueItem?.Solution?.ClientApplication))
                ClientApplication = JsonConvert.DeserializeObject<ClientApplication>(CatalogueItem.Solution.ClientApplication);
            else
                ClientApplication = new ClientApplication();
        }
    }
}
