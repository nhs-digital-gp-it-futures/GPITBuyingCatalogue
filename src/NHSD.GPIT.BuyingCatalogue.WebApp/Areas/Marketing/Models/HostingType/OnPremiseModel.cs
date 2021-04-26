using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType
{
    public class OnPremiseModel : MarketingBaseModel
    {
        public OnPremiseModel() : base(null)
        {
        }

        public OnPremiseModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";                        
            OnPremise = catalogueItem.Solution.GetHosting().OnPremise;          
        }

        public override bool? IsComplete
        {
            get 
            {
                return !string.IsNullOrWhiteSpace(OnPremise.Summary) ||
                    !string.IsNullOrWhiteSpace(OnPremise.Link) ||
                    !string.IsNullOrWhiteSpace(OnPremise.RequiresHscn) ||
                    !string.IsNullOrWhiteSpace(OnPremise.HostingModel);
            }
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
