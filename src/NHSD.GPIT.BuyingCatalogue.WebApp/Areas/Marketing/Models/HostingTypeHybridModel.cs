using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class HostingTypeHybridModel
    {
        public HostingTypeHybridModel()
        {
        }

        public HostingTypeHybridModel(CatalogueItem catalogueItem)
        {
            SolutionId = catalogueItem.CatalogueItemId;

            if (!string.IsNullOrWhiteSpace(catalogueItem.Solution.Hosting))
                HybridHostingType = JsonConvert.DeserializeObject<Hosting>(catalogueItem.Solution.Hosting).HybridHostingType;
            else
                HybridHostingType = new HybridHostingType();
        }

        public string SolutionId { get; set; }

        public HybridHostingType HybridHostingType { get; set; }

        public bool RequiresHscnChecked
        {
            get { return !string.IsNullOrWhiteSpace(HybridHostingType.RequiresHscn); }
            set
            {
                if (value)
                    HybridHostingType.RequiresHscn = "End user devices must be connected to HSCN/N3";
                else
                    HybridHostingType.RequiresHscn = null;
            }
        }
    }
}
