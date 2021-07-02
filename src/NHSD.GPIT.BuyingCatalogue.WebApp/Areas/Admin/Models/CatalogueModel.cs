using System;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class CatalogueModel
    {
        public CatalogueModel(CatalogueItem catalogueItem)
        {
            catalogueItem.ValidateNotNull(nameof(catalogueItem));

            CatalogueItemId = catalogueItem.CatalogueItemId.ToString();
            Name = catalogueItem.Name;
            LastUpdated = catalogueItem.Solution?.LastUpdated ?? DateTime.MinValue;
            PublishedStatus = catalogueItem.PublishedStatus.AsString(EnumFormat.DisplayName);
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
