using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using System;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class HostingTypeHybridModel
    {
        public HostingTypeHybridModel()
        {
        }

        public HostingTypeHybridModel(CatalogueItem catalogueItem)
        {
            Id = catalogueItem.CatalogueItemId;
            HybridHostingType = JsonConvert.DeserializeObject<Hosting>(catalogueItem.Solution.Hosting).HybridHostingType;            
        }

        public string Id { get; set; }

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
