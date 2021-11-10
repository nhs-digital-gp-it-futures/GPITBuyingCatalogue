using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.DesktopBasedModels
{
    public sealed class MemoryAndStorageModel : ApplicationTypeBaseModel
    {
        public MemoryAndStorageModel()
        {
            MemorySizes = Framework.Constants.SelectLists.MemorySizes;
            Resolutions = Framework.Constants.SelectLists.ScreenResolutions;
        }

        public MemoryAndStorageModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/admin/catalogue-solutions/manage/{catalogueItem.Id}/client-application-type/desktop";
            MemorySizes = Framework.Constants.SelectLists.MemorySizes;
            Resolutions = Framework.Constants.SelectLists.ScreenResolutions;

            SelectedMemorySize = ClientApplication?.NativeDesktopMemoryAndStorage?.MinimumMemoryRequirement;
            StorageSpace = ClientApplication?.NativeDesktopMemoryAndStorage?.StorageRequirementsDescription;
            ProcessingPower = ClientApplication?.NativeDesktopMemoryAndStorage?.MinimumCpu;
            SelectedResolution = ClientApplication?.NativeDesktopMemoryAndStorage?.RecommendedResolution;
        }

        [Required(ErrorMessage = "Select a minimum memory size")]
        public string SelectedMemorySize { get; set; }

        public IReadOnlyList<SelectListItem> MemorySizes { get; init; }

        [Required(ErrorMessage = "Enter storage space information")]
        [StringLength(300)]
        public string StorageSpace { get; set; }

        [Required(ErrorMessage = "Enter processing power information")]
        [StringLength(300)]
        public string ProcessingPower { get; set; }

        public IReadOnlyList<SelectListItem> Resolutions { get; init; }

        public string SelectedResolution { get; set; }
    }
}
