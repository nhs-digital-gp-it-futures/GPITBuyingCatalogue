using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType
{
    public class HostingTypeOnPremiseModel : MarketingBaseModel
    {
        public HostingTypeOnPremiseModel() : base(null)
        {
        }

        public HostingTypeOnPremiseModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                        
            OnPremise = catalogueItem.Solution.GetHosting().OnPremise;          
        }

        protected override bool IsComplete
        {
            get { throw new NotImplementedException(); }
        }

        public OnPremise OnPremise { get; set; }

        public bool RequiresHscnChecked
        {
            get { return !string.IsNullOrWhiteSpace(OnPremise.RequiresHscn); }
            set
            {
                if (value)
                    OnPremise.RequiresHscn = "End user devices must be connected to HSCN/N3";
                else
                    OnPremise.RequiresHscn = null;
            }
        }
    }
}
