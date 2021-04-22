using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType
{
    public class HostingTypeHybridModel : MarketingBaseModel
    {
        public HostingTypeHybridModel() : base(null)
        {
        }

        public HostingTypeHybridModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                        
            HybridHostingType = CatalogueItem.Solution.GetHosting().HybridHostingType;
        }

        protected override bool IsComplete
        {
            get { throw new NotImplementedException(); }
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
