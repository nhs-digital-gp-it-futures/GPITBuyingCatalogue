﻿using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class CatalogueModel
    {
        public CatalogueModel(CatalogueItem catalogueItem)
        {
            CatalogueItemId = catalogueItem.CatalogueItemId.ToString();
            Name = catalogueItem.Name;
            LastUpdated = catalogueItem.Supplier?.LastUpdated ?? DateTime.MinValue;
            PublishedStatus = catalogueItem.PublishedStatus.GetDisplayName();
            PublishedStatusId = (int)catalogueItem.PublishedStatus;
            Supplier = catalogueItem.Supplier?.Name ?? string.Empty;
        }

        public string CatalogueItemId { get; set; }

        public string Name { get; set; }

        public DateTime LastUpdated { get; set; }

        public string PublishedStatus { get; set; }

        public int PublishedStatusId { get; set; }

        public string Supplier { get; set; }
    }
}
