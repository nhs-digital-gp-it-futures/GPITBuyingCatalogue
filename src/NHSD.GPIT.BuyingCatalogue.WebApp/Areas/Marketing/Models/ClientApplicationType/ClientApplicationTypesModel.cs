using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType
{
    public class ClientApplicationTypesModel : MarketingBaseModel
    {
        public ClientApplicationTypesModel() : base(null)
        {
        }

        public ClientApplicationTypesModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}";
                        
            BrowserBased = ClientApplication.ClientApplicationTypes.Any(x => x.Equals("browser-based", StringComparison.InvariantCultureIgnoreCase));
            NativeMobile = ClientApplication.ClientApplicationTypes.Any(x => x.Equals("native-mobile", StringComparison.InvariantCultureIgnoreCase));
            NativeDesktop = ClientApplication.ClientApplicationTypes.Any(x => x.Equals("native-desktop", StringComparison.InvariantCultureIgnoreCase));
        }

        public override bool? IsComplete => BrowserBased || NativeDesktop || NativeMobile;

        public bool BrowserBased { get; set; }

        public bool NativeMobile { get; set; }

        public bool NativeDesktop { get; set; }
    }
}
