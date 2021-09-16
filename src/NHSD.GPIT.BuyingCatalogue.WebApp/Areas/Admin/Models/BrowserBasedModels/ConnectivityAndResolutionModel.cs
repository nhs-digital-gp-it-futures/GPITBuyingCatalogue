using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.BrowserBasedModels
{
    public class ConnectivityAndResolutionModel : ApplicationTypeBaseModel
    {
        private readonly List<SelectListItem> connectionSpeeds = new()
        {
            new() { Text = "0.5Mbps", Value = "0.5Mbps" },
            new() { Text = "1Mbps", Value = "1Mbps" },
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

        private List<SelectListItem> screenResolutions = new()
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
            new() { Text = "16:9 - 3840 x 2160", Value = "16:9 - 3840 x 2160" },
        };

        public ConnectivityAndResolutionModel()
            : base()
        {
        }

        public ConnectivityAndResolutionModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            SelectedConnectionSpeed = ClientApplication?.MinimumConnectionSpeed;

            SelectedScreenResolution = ClientApplication?.MinimumDesktopResolution;

            ConnectionSpeeds = connectionSpeeds;

            ScreenResolutions = screenResolutions;
        }

        [Required(ErrorMessage = "Select a connection speed")]
        public string SelectedConnectionSpeed { get; set; }

        public List<SelectListItem> ConnectionSpeeds { get; set; }

        public string SelectedScreenResolution { get; set; }

        public List<SelectListItem> ScreenResolutions { get; set; }
    }
}
