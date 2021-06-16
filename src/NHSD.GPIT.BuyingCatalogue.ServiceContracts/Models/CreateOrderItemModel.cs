﻿using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    // MJRTODO - Change name as this is used for Create and Edit
    public sealed class CreateOrderItemModel
    {
        public DateTime? CommencementDate { get; set; }

        public string SupplierId { get; set; }

        public DateTime? PlannedDeliveryDate { get; set; }

        public int? Quantity { get; set; }

        public string CatalogueItemName { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public string CatalogueSolutionId { get; set; }

        public string CurrencyCode { get; set; }

        public TimeUnit EstimationPeriod { get; set; }

        public ItemUnitModel ItemUnit { get; set; }

        public int? PriceId { get; set; }

        public decimal? Price { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public List<OrderItemRecipientModel> ServiceRecipients { get; set; }

        public TimeUnit TimeUnit { get; set; }

        public CataloguePriceType Type { get; set; }

        public bool IsNewOrder { get; set; }
    }
}
