using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItemPriceTier : IAudited
    {
        public OrderItemPriceTier()
        {
        }

        public int Id { get; set; }

        public int OrderId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public decimal Price { get; set; }

        public int LowerRange { get; set; }

        public int? UpperRange { get; set; }

        public decimal ListPrice { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public OrderItemPrice OrderItemPrice { get; set; }
    }
}
