using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

            SelectedMemorySize = ClientApplication?.NativeDesktopMemoryAndStorage?.MinimumMemoryRequirement;

            StorageDescription = ClientApplication?.NativeDesktopMemoryAndStorage?.StorageRequirementsDescription;

            MinimumCpu = ClientApplication?.NativeDesktopMemoryAndStorage?.MinimumCpu;

            SelectedScreenResolution = ClientApplication?.NativeDesktopMemoryAndStorage?.RecommendedResolution;
        }

        public override bool? IsComplete =>
            !string.IsNullOrWhiteSpace(ClientApplication?.NativeDesktopMemoryAndStorage?.MinimumMemoryRequirement)
            && !string.IsNullOrWhiteSpace(ClientApplication?.NativeDesktopMemoryAndStorage?.StorageRequirementsDescription)
            && !string.IsNullOrWhiteSpace(ClientApplication?.NativeDesktopMemoryAndStorage?.MinimumCpu);

        public string SelectedMemorySize { get; set; }
        public List<SelectListItem> MemorySizes { get; set; }

        [StringLength(300)]
        public string StorageDescription { get; set; }

        [StringLength(300)]
        public string MinimumCpu { get; set; }

        public string SelectedScreenResolution { get; set; }
        public List<SelectListItem> ScreenResolutions { get; set; }
    }
}
