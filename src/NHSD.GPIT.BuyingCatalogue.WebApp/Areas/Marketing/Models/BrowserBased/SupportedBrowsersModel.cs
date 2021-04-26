using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class SupportedBrowsersModel : MarketingBaseModel
    {
        public SupportedBrowsersModel() : base(null)
        {
        }

        public SupportedBrowsersModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/browser-based";

            Browsers = new SupportedBrowserModel[]
            {
                new SupportedBrowserModel{ BrowserName = "Google Chrome" },
                new SupportedBrowserModel{ BrowserName = "Microsoft Edge" },
                new SupportedBrowserModel{ BrowserName = "Mozilla Firefox" },
                new SupportedBrowserModel{ BrowserName = "Opera" },
                new SupportedBrowserModel{ BrowserName = "Safari" },
                new SupportedBrowserModel{ BrowserName = "Chromium" },
                new SupportedBrowserModel{ BrowserName = "Internet Explorer 11" },
                new SupportedBrowserModel{ BrowserName = "Internet Explorer 10" }
            };

            CheckBrowsers();

            if (ClientApplication.MobileResponsive.HasValue)
                MobileResponsive = ClientApplication.MobileResponsive.ToYesNo();
        }

        public override bool? IsComplete
        {
            get 
            {
                if (ClientApplication == null || ClientApplication.BrowsersSupported == null || !ClientApplication.BrowsersSupported.Any())
                    return false;

                return ClientApplication.MobileResponsive.HasValue;
            }
        }

        public SupportedBrowserModel[] Browsers { get; set; }

        public string MobileResponsive { get; set; }

        private void CheckBrowsers()
        {
            foreach( var browser in Browsers )
            {
                if( ClientApplication.BrowsersSupported != null && 
                    ClientApplication.BrowsersSupported.Any(x=>x.Equals(browser.BrowserName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    browser.Checked = true;
                }
            }
        }
    }
}
