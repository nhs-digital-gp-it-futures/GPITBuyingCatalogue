using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.ViewModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased
{
    public class SupportedBrowsersModel : MarketingBaseModel
    {
        public SupportedBrowserModel[] Browsers { get; set; }

        public string MobileResponsive { get; set; }

        private readonly SupportedBrowserModel[] SupportedBrowsers = new SupportedBrowserModel[]
            {
                new() { BrowserName = "Google Chrome" },
                new() { BrowserName = "Microsoft Edge" },
                new() { BrowserName = "Mozilla Firefox" },
                new() { BrowserName = "Opera" },
                new() { BrowserName = "Safari" },
                new() { BrowserName = "Chromium" },
                new() { BrowserName = "Internet Explorer 11" },
                new() { BrowserName = "Internet Explorer 10" }
            };

        public SupportedBrowsersModel() : base(null)
        {
        }

        public SupportedBrowsersModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/browser-based";

            Browsers = SupportedBrowsers;

            CheckBrowsers();

            if (ClientApplication.MobileResponsive.HasValue)
                MobileResponsive = ClientApplication.MobileResponsive.ToYesNo();
        }

        public override bool? IsComplete =>
            ClientApplication?.BrowsersSupported != null && ClientApplication.BrowsersSupported.Any() &&
            ClientApplication.MobileResponsive.HasValue;


        private void CheckBrowsers()
        {
            foreach( var browser in Browsers )
            {
                if(ClientApplication.BrowsersSupported != null && 
                    ClientApplication.BrowsersSupported.Any(x=>x.Equals(browser.BrowserName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    browser.Checked = true;
                }
            }
        }

        public PageTitleViewModel PageTitle() =>
            new()
            {
                Advice = "Let buyers know which types of browser will work with your Catalogue Solution.",
                Title = "Browser-based application – supported browsers",
            };
    }
}
