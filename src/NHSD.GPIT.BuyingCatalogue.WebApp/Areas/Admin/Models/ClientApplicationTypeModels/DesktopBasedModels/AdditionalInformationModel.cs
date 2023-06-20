﻿using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ClientApplicationTypeModels.DesktopBasedModels
{
    public sealed class AdditionalInformationModel : ApplicationTypeBaseModel
    {
        public AdditionalInformationModel()
        {
        }

        public AdditionalInformationModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            AdditionalInformation = ApplicationTypeDetail?.NativeDesktopAdditionalInformation;
        }

        [StringLength(500)]
        public string AdditionalInformation { get; set; }
    }
}
