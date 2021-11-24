﻿using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.MobileTabletBasedModels
{
    public sealed class HardwareRequirementsModel : ApplicationTypeBaseModel
    {
        public HardwareRequirementsModel()
        {
        }

        public HardwareRequirementsModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            Description = ClientApplication?.NativeMobileHardwareRequirements;
        }

        [StringLength(500)]
        public string Description { get; set; }
    }
}
