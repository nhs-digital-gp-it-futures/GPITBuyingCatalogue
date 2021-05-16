using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.DataAttributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.ViewModels;

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

            ProcessCheckboxFields(ClientApplication.ClientApplicationTypes);
        }

        public override bool? IsComplete => BrowserBased || NativeDesktop || NativeMobile;
        
        [Checkbox("Browser-based")]
        public bool BrowserBased { get; set; }

        [Checkbox("Native mobile or tablet")]
        public bool NativeMobile { get; set; }

        [Checkbox("Native desktop")]
        public bool NativeDesktop { get; set; }

        public PageTitleViewModel PageTitle() =>
            new()
            {
                Advice = "Let buyers know which types of application will work with your Catalogue Solution.",
                Title = "Client application type"
            };
    }
}
