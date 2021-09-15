﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BrowserBasedModels
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
            BackLinkText = "Go back";
        }

        public SupportedBrowsersModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            Browsers = supportedBrowsers;

            CheckBrowsers();

            if (ClientApplication.MobileResponsive.HasValue)
                MobileResponsive = ClientApplication.MobileResponsive.ToYesNo();

            BackLinkText = "Go back";
        }

        public SupportedBrowserModel[] Browsers { get; set; }

        [Required(ErrorMessage = "Select yes if your Catalogue Solution is mobile responsive")]
        public string MobileResponsive { get; set; }

        private void CheckBrowsers()
        {
            foreach (var browser in Browsers)
            {
                browser.Checked = ClientApplication.BrowsersSupported is not null &&
                    ClientApplication.BrowsersSupported.Contains(browser.BrowserName);
            }
        }
    }
}
