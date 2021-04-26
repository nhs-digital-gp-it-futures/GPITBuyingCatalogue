using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType
{
    public class HybridModel : MarketingBaseModel
    {
        public HybridModel() : base(null)
        {
        }

        public HybridModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                        
            HybridHostingType = CatalogueItem.Solution.GetHosting().HybridHostingType;
        }

        public override bool? IsComplete
        {
            get 
            {
                return !string.IsNullOrWhiteSpace(HybridHostingType.Summary) ||
                    !string.IsNullOrWhiteSpace(HybridHostingType.Link) ||
                    !string.IsNullOrWhiteSpace(HybridHostingType.RequiresHscn);                                                                
            }
        }        

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
