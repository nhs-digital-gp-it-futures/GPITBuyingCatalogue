using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType
{
    public class HostingTypePublicCloudModel : NavBaseModel
    {
        public HostingTypePublicCloudModel()
        {
            PublicCloud = new PublicCloud();
        }

        public HostingTypePublicCloudModel(CatalogueItem catalogueItem)
        {
            BackLink = $"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}";
            BackLinkText = "Return to all sections";

            SolutionId = catalogueItem.CatalogueItemId;
            PublicCloud = catalogueItem.Solution.GetHosting().PublicCloud;
        }

        public string SolutionId { get; set; }

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
