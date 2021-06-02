using System;
using NHSD.GPIT.BuyingCatalogue.Components.DataAttributes;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType
{
    public class ClientApplicationTypesModel : MarketingBaseModel
    {
        public ClientApplicationTypesModel()
            : base(null)
        {
        }

        public override bool? IsComplete => BrowserBased || NativeDesktop || NativeMobile;

        [Checkbox("Browser-based")]
        public bool BrowserBased { get; set; }

        [Checkbox("Native mobile or tablet")]
        public bool NativeMobile { get; set; }

        [Checkbox("Native desktop")]
        public bool NativeDesktop { get; set; }
    }
}
