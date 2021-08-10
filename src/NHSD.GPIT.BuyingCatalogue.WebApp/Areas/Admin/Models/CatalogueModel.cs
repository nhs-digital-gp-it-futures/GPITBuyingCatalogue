using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class CatalogueModel
    {
        public CatalogueModel(CatalogueItem catalogueItem)
        {
            catalogueItem.ValidateNotNull(nameof(catalogueItem));

            CatalogueItemId = catalogueItem.Id.ToString();
            Name = catalogueItem.Name;
            LastUpdated = catalogueItem.Solution?.LastUpdated ?? DateTime.MinValue;
            PublishedStatus = catalogueItem.PublishedStatus.Name();
            PublishedStatusId = catalogueItem.PublishedStatus;
            Supplier = catalogueItem.Supplier?.Name ?? string.Empty;
        }

        public string CatalogueItemId { get; set; }

        public string Name { get; set; }

        public DateTime LastUpdated { get; set; }

        public string PublishedStatus { get; set; }

        public PublicationStatus PublishedStatusId { get; set; }

        public string Supplier { get; set; }
    }
}
