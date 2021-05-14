using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop
{
    public class MemoryAndStorageModel : MarketingBaseModel
    {
        public MemoryAndStorageModel() : base(null)
        {
        }

        public MemoryAndStorageModel(CatalogueItem catalogueItem) : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-desktop";

            MemorySizes = new List<SelectListItem>
            {
                new() { Text = "Please select"},
                new() { Text = "256MB", Value = "256MB"},
                new() { Text = "512MB", Value = "512MB"},
                new() { Text = "1GB", Value = "1GB"},
                new() { Text = "2GB", Value = "2GB"},
                new() { Text = "4GB", Value = "4GB"},
                new() { Text = "8GB", Value = "8GB"},
                new() { Text = "16GB or higher", Value = "16GB or higher"}
            };

            SelectedMemorySize = ClientApplication?.NativeDesktopMemoryAndStorage?.MinimumMemoryRequirement;

            StorageDescription = ClientApplication?.NativeDesktopMemoryAndStorage?.StorageRequirementsDescription;

            MinimumCpu = ClientApplication?.NativeDesktopMemoryAndStorage?.MinimumCpu;

            ScreenResolutions = new List<SelectListItem>
            {
                new() { Text = "Please select", Value = "" },
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

            SelectedScreenResolution = ClientApplication?.NativeDesktopMemoryAndStorage?.RecommendedResolution;
        }

        public override bool? IsComplete =>
            !string.IsNullOrWhiteSpace(ClientApplication?.NativeDesktopMemoryAndStorage?.MinimumMemoryRequirement)
            && !string.IsNullOrWhiteSpace(ClientApplication?.NativeDesktopMemoryAndStorage?.StorageRequirementsDescription)
            && !string.IsNullOrWhiteSpace(ClientApplication?.NativeDesktopMemoryAndStorage?.MinimumCpu);

        public string SelectedMemorySize { get; set; }
        public List<SelectListItem> MemorySizes { get; set; }

        public string StorageDescription { get; set; }

        public string MinimumCpu { get; set; }

        public string SelectedScreenResolution { get; set; }
        public List<SelectListItem> ScreenResolutions { get; set; }
    }
}
