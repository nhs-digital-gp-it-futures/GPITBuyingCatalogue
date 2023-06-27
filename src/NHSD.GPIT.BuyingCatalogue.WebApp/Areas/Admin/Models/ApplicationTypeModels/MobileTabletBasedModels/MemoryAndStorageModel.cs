using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ApplicationTypeModels.MobileTabletBasedModels
{
    public sealed class MemoryAndStorageModel : ApplicationTypeBaseModel
    {
        public MemoryAndStorageModel()
        {
            MemorySizes = Framework.Constants.SelectLists.MemorySizes;
        }

        public MemoryAndStorageModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            MemorySizes = Framework.Constants.SelectLists.MemorySizes;

            SelectedMemorySize = ApplicationTypeDetail?.MobileMemoryAndStorage?.MinimumMemoryRequirement;
            Description = ApplicationTypeDetail?.MobileMemoryAndStorage?.Description;
        }

        public string SelectedMemorySize { get; set; }

        public IReadOnlyList<SelectOption<string>> MemorySizes { get; init; }

        [StringLength(300)]
        public string Description { get; set; }
    }
}
