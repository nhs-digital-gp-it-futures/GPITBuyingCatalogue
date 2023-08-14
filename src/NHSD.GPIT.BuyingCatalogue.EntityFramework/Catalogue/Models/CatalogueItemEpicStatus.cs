using System;
using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class CatalogueItemEpicStatus : IAudited
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsMet { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }
    }
}
