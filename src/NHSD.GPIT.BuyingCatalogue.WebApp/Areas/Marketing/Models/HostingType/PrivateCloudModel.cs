using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType
{
    public class PrivateCloudModel : MarketingBaseModel
    {
        public PrivateCloudModel() : base(null)
        {
        }

        public PrivateCloudModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                     
            PrivateCloud = catalogueItem.Solution.GetHosting().PrivateCloud;
        }

        protected override bool IsComplete
        {
            get { throw new NotImplementedException(); }
        }        

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
