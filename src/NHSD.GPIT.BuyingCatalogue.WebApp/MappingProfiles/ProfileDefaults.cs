using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.MappingProfiles
{
    public static class ProfileDefaults
    {
        public static List<SelectListItem> ConnectionSpeeds = new List<SelectListItem>
        {
            new() { Text = "0.5Mbps", Value="0.5Mbps"},
            new() { Text = "1Mbps", Value="1Mbps"},
            new() { Text = "1.5Mbps", Value = "1.5Mbps" },
            new() { Text = "2Mbps", Value = "2Mbps" },
            new() { Text = "3Mbps", Value = "3Mbps" },
            new() { Text = "5Mbps", Value = "5Mbps" },
            new() { Text = "8Mbps", Value = "8Mbps" },
            new() { Text = "10Mbps", Value = "10Mbps" },
            new() { Text = "15Mbps", Value = "15Mbps" },
            new() { Text = "20Mbps", Value = "20Mbps" },
            new() { Text = "30Mbps", Value = "30Mbps" },
            new() { Text = "Higher than 30Mbps", Value = "Higher than 30Mbps" },
       };

        public static List<SelectListItem> ScreenResolutions = new List<SelectListItem>
        {
            new() { Text = "16:9 - 640 x 360", Value = "16:9 - 640 x 360" },
            new() { Text = "4:3 - 800 x 600", Value = "4:3 - 800 x 600" },
            new() { Text = "4:3 - 1024 x 768", Value = "4:3 - 1024 x 768" },
            new() { Text = "16:9 - 1280 x 720", Value = "16:9 - 1280 x 720" },
            new() { Text = "16:10 - 1280 x 800", Value = "16:10 - 1280 x 800" },
            new() { Text = "5:4 - 1280 x 1024", Value = "5:4 - 1280 x 1024" },
            new() { Text = "16:9 - 1360 x 768", Value = "16:9 - 1360 x 768" },
            new() { Text = "16:9 - 1366 x 768", Value = "16:9 - 1366 x 768" },
            new() { Text = "16:10 - 1440 x 900", Value = "16:10 - 1440 x 900" },
            new() { Text = "16:9 - 1536 x 864", Value = "16:9 - 1536 x 864" },
            new() { Text = "16:9 - 1600 x 900", Value = "16:9 - 1600 x 900" },
            new() { Text = "16:10 - 1680 x 1050", Value = "16:10 - 1680 x 1050" },
            new() { Text = "16:9 - 1920 x 1080", Value = "16:9 - 1920 x 1080" },
            new() { Text = "16:10 - 1920 x 1200", Value = "16:10 - 1920 x 1200" },
            new() { Text = "16:9 - 2048 x 1152", Value = "16:9 - 2048 x 1152" },
            new() { Text = "21:9 - 2560 x 1080", Value = "21:9 - 2560 x 1080" },
            new() { Text = "16:9 - 2560 x 1440", Value = "16:9 - 2560 x 1440" },
            new() { Text = "21:9 - 3440 x 1440", Value = "21:9 - 3440 x 1440" },
            new() { Text = "16:9 - 3840 x 2160", Value = "16:9 - 3840 x 2160" }
        };

        public static List<SelectListItem> MemorySizes = new List<SelectListItem> {
            new SelectListItem{ Text = "256MB", Value = "256MB"},
            new SelectListItem{ Text = "512MB", Value = "512MB"},
            new SelectListItem{ Text = "1GB", Value = "1GB"},
            new SelectListItem{ Text = "2GB", Value = "2GB"},
            new SelectListItem{ Text = "4GB", Value = "4GB"},
            new SelectListItem{ Text = "8GB", Value = "8GB"},
            new SelectListItem{ Text = "16GB or higher", Value = "16GB or higher"}
        };

        public static SupportedBrowserModel[] SupportedBrowsers = new SupportedBrowserModel[]
        {
            new() {BrowserName = "Google Chrome"},
            new() {BrowserName = "Microsoft Edge"},
            new() { BrowserName = "Mozilla Firefox" },
            new() { BrowserName = "Opera" },
            new() { BrowserName = "Safari" },
            new() { BrowserName = "Chromium" },
            new() { BrowserName = "Internet Explorer 11" },
            new() { BrowserName = "Internet Explorer 10" }
        };

        public static SupportedOperatingSystemModel[] SupportedOperatingSystems = new SupportedOperatingSystemModel[]
        {
            new SupportedOperatingSystemModel{ OperatingSystemName = "Apple IOS" },
            new SupportedOperatingSystemModel{ OperatingSystemName = "Android" },
            new SupportedOperatingSystemModel{ OperatingSystemName = "Other" }
        };

        public static ConnectionTypeModel[] MobileConnectionTypes = new ConnectionTypeModel[]
        {
            new() { ConnectionType = "GPRS" },
            new() { ConnectionType = "3G" },
            new() { ConnectionType = "LTE" },
            new() { ConnectionType = "4G" },
            new() { ConnectionType = "5G" },
            new() { ConnectionType = "Bluetooth" },
            new() { ConnectionType = "Wifi" }
        };

        public static string GetBrowserBasedBackLink(string CatalogueItemId)
        {
            return $"/marketing/supplier/solution/{CatalogueItemId}/section/browser-based";
        }

        public static string GetNativeDesktopBackLink(string CatalogueItemId)
        {
            return $"/marketing/supplier/solution/{CatalogueItemId}/section/native-desktop";
        }

        public static string GetNativeMobileBackLink(string CatalogueItemId)
        {
            return $"/marketing/supplier/solution/{CatalogueItemId}/section/native-mobile";
        }

        public static string GetSolutionBackLink(string CatalogueItemId)
        {
            return $"/marketing/supplier/solution/{CatalogueItemId}";
        }
    }
}
