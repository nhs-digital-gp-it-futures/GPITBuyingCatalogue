using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    [ExcludeFromCodeCoverage]
    public sealed class CreateOrderItemModel
    {
        public CallOffId CallOffId { get; set; }

        public DateTime? CommencementDate { get; set; }

        public int? SupplierId { get; set; }

        public DateTime? PlannedDeliveryDate { get; set; }

        public int? Quantity { get; set; }

        public string CatalogueItemName { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public CatalogueItemId? CatalogueItemId { get; set; }

        public CataloguePrice CataloguePrice { get; set; }

        public decimal? AgreedPrice { get; set; }

        public List<OrderItemRecipientModel> ServiceRecipients { get; set; }

        public TimeUnit? EstimationPeriod { get; set; }

        public bool IsNewSolution { get; set; }

        public IEnumerable<CatalogueItemId> SolutionIds { get; set; }

        public bool SkipPriceSelection { get; set; }

        public string CurrencySymbol { get; set; }

        public string TimeUnitDescription
        {
            get
            {
                if (CataloguePrice?.ProvisioningType != ProvisioningType.OnDemand)
                    return CataloguePrice?.TimeUnit?.Description();

                return (EstimationPeriod ?? CataloguePrice?.TimeUnit)?.Description();
            }
        }
    }
}
