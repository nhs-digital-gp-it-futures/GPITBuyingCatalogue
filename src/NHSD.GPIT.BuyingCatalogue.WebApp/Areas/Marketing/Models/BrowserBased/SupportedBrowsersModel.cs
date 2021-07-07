using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.MappingProfiles;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class SupportedBrowsersModel : MarketingBaseModel
    {
        private readonly SupportedBrowserModel[] supportedBrowsers = ProfileDefaults.SupportedBrowsers;

        public SupportedBrowsersModel()
            : base(null)
        {
        }

        public SupportedBrowsersModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/browser-based";

            Browsers = supportedBrowsers;

            CheckBrowsers();

            if (ClientApplication.MobileResponsive.HasValue)
                MobileResponsive = ClientApplication.MobileResponsive.ToYesNo();
        }

        public SupportedBrowserModel[] Browsers { get; set; }

        public string MobileResponsive { get; set; }

        public override bool? IsComplete =>
            ClientApplication?.BrowsersSupported != null && ClientApplication.BrowsersSupported.Any() &&
            ClientApplication.MobileResponsive.HasValue;

        private void CheckBrowsers()
        {
            foreach (var browser in Browsers)
            {
                if (ClientApplication.BrowsersSupported != null &&
                    ClientApplication.BrowsersSupported.Any(x => x.Equals(browser.BrowserName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    browser.Checked = true;
                }
            }
        }
    }
}
