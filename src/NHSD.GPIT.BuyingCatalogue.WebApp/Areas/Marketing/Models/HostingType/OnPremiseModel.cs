using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType
{
    public class OnPremiseModel : MarketingBaseModel
    {
        public OnPremiseModel()
            : base(null)
        {
        }

        public override bool? IsComplete => OnPremise?.IsValid();

        public OnPremise OnPremise { get; set; }

        public bool RequiresHscnChecked
        {
            get => !string.IsNullOrWhiteSpace(OnPremise?.RequiresHscn);

            set => OnPremise.RequiresHscn = value ? "End user devices must be connected to HSCN/N3" : null;
        }
    }
}
