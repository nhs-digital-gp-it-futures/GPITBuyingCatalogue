﻿using System;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.WebApp.ViewModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop
{
    public class OperatingSystemsModel : MarketingBaseModel
    {
        public OperatingSystemsModel()
            : base(null)
        {
        }

        public OperatingSystemsModel(CatalogueItem catalogueItem)
            : base(catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            BackLink = $"/marketing/supplier/solution/{CatalogueItem.CatalogueItemId}/section/native-desktop";

            OperatingSystemsDescription = ClientApplication?.NativeDesktopOperatingSystemsDescription;
        }

        public override bool? IsComplete =>
            !string.IsNullOrWhiteSpace(ClientApplication?.NativeDesktopOperatingSystemsDescription);

        [StringLength(1000)]
        public string OperatingSystemsDescription { get; set; }

        public PageTitleViewModel PageTitle() =>
            new()
            {
                Advice = "Let buyers know which operating systems will work with your Catalogue Solution.",
                Title = "Native desktop application – supported operating systems",
            };
    }
}
