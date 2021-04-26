using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType
{
    public class PublicCloudModel : MarketingBaseModel
    {
        public PublicCloudModel() : base(null)
        {
            PublicCloud = new PublicCloud();
        }

        public PublicCloudModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                        
            PublicCloud = catalogueItem.Solution.GetHosting().PublicCloud;
        }

        public override bool? IsComplete
        {
            get 
            {
                return !string.IsNullOrWhiteSpace(PublicCloud.Summary) ||
                    !string.IsNullOrWhiteSpace(PublicCloud.Link) ||
                    !string.IsNullOrWhiteSpace(PublicCloud.RequiresHscn);
            }
        }        

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
