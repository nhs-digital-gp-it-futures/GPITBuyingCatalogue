using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType
{
    public class HostingTypePrivateCloudModel : NavBaseModel
    {
        public HostingTypePrivateCloudModel()
        {
        }

        public HostingTypePrivateCloudModel(CatalogueItem catalogueItem)
        {
            BackLink = $"/marketing/supplier/solution/{catalogueItem.CatalogueItemId}";
            BackLinkText = "Return to all sections";

            SolutionId = catalogueItem.CatalogueItemId;
            PrivateCloud = catalogueItem.Solution.GetHosting().PrivateCloud;
        }

        public string SolutionId { get; set; }

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
