using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using System;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models
{
    public class HostingTypePrivateCloudModel
    {
        public HostingTypePrivateCloudModel()
        {
        }

        public HostingTypePrivateCloudModel(CatalogueItem catalogueItem)
        {
            Id = catalogueItem.CatalogueItemId;
            PrivateCloud = JsonConvert.DeserializeObject<Hosting>(catalogueItem.Solution.Hosting).PrivateCloud;            
        }

        public string Id { get; set; }

        public PrivateCloud PrivateCloud { get; set; }

        public bool RequiresHscnChecked
        {
            get { return !string.IsNullOrWhiteSpace(PrivateCloud.RequiresHscn); }
            set
            {
                if (value)
                    PrivateCloud.RequiresHscn = "End user devices must be connected to HSCN/N3";
                else
                    PrivateCloud.RequiresHscn = null;
            }
        }
    }
}
