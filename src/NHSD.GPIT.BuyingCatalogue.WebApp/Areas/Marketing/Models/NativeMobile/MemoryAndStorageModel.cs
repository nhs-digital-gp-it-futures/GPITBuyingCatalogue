﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.ViewModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile
{
    public class MemoryAndStorageModel : MarketingBaseModel
    {
        public MemoryAndStorageModel()
            : base(null)
        {
        }

        public MemoryAndStorageModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-mobile";

            SelectedMemorySize = ClientApplication?.MobileMemoryAndStorage?.MinimumMemoryRequirement;
            Description = ClientApplication?.MobileMemoryAndStorage?.Description;
        }

        public override bool? IsComplete =>
            !string.IsNullOrWhiteSpace(ClientApplication?.MobileMemoryAndStorage?.MinimumMemoryRequirement) &&
            !string.IsNullOrWhiteSpace(ClientApplication?.MobileMemoryAndStorage?.Description);

        public string SelectedMemorySize { get; set; }

        public List<SelectListItem> MemorySizes { get; set; }

        [StringLength(300)]
        public string Description { get; set; }

        public PageTitleViewModel PageTitle() =>
            new()
            {
                Advice = "Let buyers know the minimum requirements for your Catalogue Solution to function.",
                Title = "Native mobile and tablet application – memory and storage",
            };
    }
}
