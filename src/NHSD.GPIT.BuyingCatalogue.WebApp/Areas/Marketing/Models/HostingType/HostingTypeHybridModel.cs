using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType
{
    public class HostingTypeHybridModel : NavBaseModel
    {
        public HostingTypeHybridModel()
        {
        }

        public HostingTypeHybridModel(CatalogueItem catalogueItem)
        {
            BackLink = $"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}";
            BackLinkText = "Return to all sections";

            SolutionId = catalogueItem.CatalogueItemId;
            HybridHostingType = catalogueItem.Solution.GetHosting().HybridHostingType;
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
