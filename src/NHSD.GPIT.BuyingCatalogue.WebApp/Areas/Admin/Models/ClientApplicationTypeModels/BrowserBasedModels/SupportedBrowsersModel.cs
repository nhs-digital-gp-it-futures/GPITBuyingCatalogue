using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.BrowserBasedModels
{
    public class SupportedBrowsersModel : ApplicationTypeBaseModel
    {
        private readonly SupportedBrowserModel[] supportedBrowsers =
        {
            new() { BrowserName = "Google Chrome" },
            new() { BrowserName = "Microsoft Edge" },
            new() { BrowserName = "Mozilla Firefox" },
            new() { BrowserName = "Opera" },
            new() { BrowserName = "Safari" },
            new() { BrowserName = "Chromium" },
            new() { BrowserName = "Internet Explorer 11" },
            new() { BrowserName = "Internet Explorer 10" },
        };

        public SupportedBrowsersModel()
            : base()
        {
        }

        public SupportedBrowsersModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            Browsers = supportedBrowsers;

            CheckBrowsers();

            if (ApplicationTypeDetail.MobileResponsive.HasValue)
                MobileResponsive = ApplicationTypeDetail.MobileResponsive.ToYesNo();
        }

        public SupportedBrowserModel[] Browsers { get; set; }

        public string MobileResponsive { get; set; }

        private void CheckBrowsers()
        {
            foreach (var browser in Browsers)
            {
                browser.Checked = ApplicationTypeDetail.BrowsersSupported is not null &&
                    ApplicationTypeDetail.BrowsersSupported.Any(bs => bs.BrowserName.Contains(browser.BrowserName));

                if (ApplicationTypeDetail.BrowsersSupported.Any(bs => bs.BrowserName.Contains(browser.BrowserName)))
                {
                    browser.MinimumBrowserVersion = ApplicationTypeDetail.BrowsersSupported
                        .FirstOrDefault(bs => bs.BrowserName.Contains(browser.BrowserName)).MinimumBrowserVersion;
                }
            }
        }
    }
}
