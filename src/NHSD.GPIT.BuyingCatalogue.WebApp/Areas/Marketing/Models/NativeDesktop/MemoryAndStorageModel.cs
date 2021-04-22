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
            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-desktop";

            MemorySizes = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Please select"},
                new SelectListItem{ Text = "256MB", Value = "256MB"},
                new SelectListItem{ Text = "512MB", Value = "512MB"},
                new SelectListItem{ Text = "1GB", Value = "1GB"},
                new SelectListItem{ Text = "2GB", Value = "2GB"},
                new SelectListItem{ Text = "4GB", Value = "4GB"},
                new SelectListItem{ Text = "8GB", Value = "8GB"},
                new SelectListItem{ Text = "16GB or higher", Value = "16GB or higher"}
            };

            SelectedMemorySize = ClientApplication.NativeDesktopMemoryAndStorage?.MinimumMemoryRequirement;

            StorageDescription = ClientApplication.NativeDesktopMemoryAndStorage?.StorageRequirementsDescription;

            MinimumCpu = ClientApplication.NativeDesktopMemoryAndStorage?.MinimumCpu;

            ScreenResolutions = new List<SelectListItem>
            {
                new SelectListItem{ Text = "Please select", Value = "" },
                new SelectListItem{ Text = "16:9 - 640 x 360", Value = "16:9 - 640 x 360" },
                new SelectListItem{ Text = "4:3 - 800 x 600", Value = "4:3 - 800 x 600" },
                new SelectListItem{ Text = "4:3 - 1024 x 768", Value = "4:3 - 1024 x 768" },
                new SelectListItem{ Text = "16:9 - 1280 x 720", Value = "16:9 - 1280 x 720" },
                new SelectListItem{ Text = "16:10 - 1280 x 800", Value = "16:10 - 1280 x 800" },
                new SelectListItem{ Text = "5:4 - 1280 x 1024", Value = "5:4 - 1280 x 1024" },
                new SelectListItem{ Text = "16:9 - 1360 x 768", Value = "16:9 - 1360 x 768" },
                new SelectListItem{ Text = "16:9 - 1366 x 768", Value = "16:9 - 1366 x 768" },
                new SelectListItem{ Text = "16:10 - 1440 x 900", Value = "16:10 - 1440 x 900" },
                new SelectListItem{ Text = "16:9 - 1536 x 864", Value = "16:9 - 1536 x 864" },
                new SelectListItem{ Text = "16:9 - 1600 x 900", Value = "16:9 - 1600 x 900" },
                new SelectListItem{ Text = "16:10 - 1680 x 1050", Value = "16:10 - 1680 x 1050" },
                new SelectListItem{ Text = "16:9 - 1920 x 1080", Value = "16:9 - 1920 x 1080" },
                new SelectListItem{ Text = "16:10 - 1920 x 1200", Value = "16:10 - 1920 x 1200" },
                new SelectListItem{ Text = "16:9 - 2048 x 1152", Value = "16:9 - 2048 x 1152" },
                new SelectListItem{ Text = "21:9 - 2560 x 1080", Value = "21:9 - 2560 x 1080" },
                new SelectListItem{ Text = "16:9 - 2560 x 1440", Value = "16:9 - 2560 x 1440" },
                new SelectListItem{ Text = "21:9 - 3440 x 1440", Value = "21:9 - 3440 x 1440" },
                new SelectListItem{ Text = "16:9 - 3840 x 2160", Value = "16:9 - 3840 x 2160" }
            };

            SelectedScreenResolution = ClientApplication.MinimumDesktopResolution;
        }

        public override bool? IsComplete
        {
            get 
            {
                return !string.IsNullOrWhiteSpace(ClientApplication.NativeDesktopMemoryAndStorage?.MinimumMemoryRequirement)
                  && !string.IsNullOrWhiteSpace(ClientApplication.NativeDesktopMemoryAndStorage?.StorageRequirementsDescription)
                  && !string.IsNullOrWhiteSpace(ClientApplication.NativeDesktopMemoryAndStorage?.MinimumCpu); 
            }
        }

        public string SelectedMemorySize { get; set; }
        public List<SelectListItem> MemorySizes { get; set; }

        public string StorageDescription { get; set; }

        public string MinimumCpu { get; set; }

        public string SelectedScreenResolution { get; set; }
        public List<SelectListItem> ScreenResolutions { get; set; }
    }
}
