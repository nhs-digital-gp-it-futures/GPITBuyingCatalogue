﻿using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.DesktopBasedModels
{
    public sealed class ThirdPartyComponentsModel : ApplicationTypeBaseModel
    {
        public ThirdPartyComponentsModel()
        {
        }

        public ThirdPartyComponentsModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            ThirdPartyComponents = ApplicationTypeDetail?.NativeDesktopThirdParty?.ThirdPartyComponents;
            DeviceCapabilities = ApplicationTypeDetail?.NativeDesktopThirdParty?.DeviceCapabilities;
        }

        [StringLength(500)]
        public string ThirdPartyComponents { get; set; }

        [StringLength(500)]
        public string DeviceCapabilities { get; set; }
    }
}
