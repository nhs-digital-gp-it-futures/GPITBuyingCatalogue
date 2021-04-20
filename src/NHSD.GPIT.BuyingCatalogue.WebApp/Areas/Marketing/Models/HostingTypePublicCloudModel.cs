using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using System;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class HostingTypePublicCloudModel
    {
        public HostingTypePublicCloudModel()
        {
            PublicCloud = new PublicCloud();
        }

        public HostingTypePublicCloudModel(CatalogueItem catalogueItem)
        {
            Id = catalogueItem.CatalogueItemId;
            PublicCloud = JsonConvert.DeserializeObject<Hosting>(catalogueItem.Solution.Hosting).PublicCloud;                        
        }

        public string Id { get; set; }

        public PublicCloud PublicCloud { get; set; }   
        
        public bool RequiresHscnChecked 
        {
            get { return !string.IsNullOrWhiteSpace(PublicCloud.RequiresHscn); }
            set
            {
                if (value)
                    PublicCloud.RequiresHscn = "End user devices must be connected to HSCN/N3";
                else
                    PublicCloud.RequiresHscn = null;
            }
        }
    }
}
