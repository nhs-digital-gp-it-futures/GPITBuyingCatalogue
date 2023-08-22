using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources
{
    public sealed class FundingSource : OrderingBaseModel
    {
        public FundingSource()
        {
        }

        public FundingSource(string internalOrgId, CallOffId callOffId, Order order, OrderItem orderItem)
        {
            Title = "Funding source";
            Caption = orderItem.CatalogueItem.Name;
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            CatalogueItemName = orderItem.CatalogueItem.Name;
            SelectedFundingType = orderItem.FundingType;
            TotalCost = order.TotalCostForOrderItem(orderItem.CatalogueItem.Id);

            AvailableFundingTypes =
                order.SelectedFramework.FundingTypes.Select(
                        x =>
                        {
                            var fundingType = x.AsOrderItemFundingType();

                            return new SelectOption<OrderItemFundingType>(
                                fundingType.Description(),
                                GetFundingTypeHintText(fundingType),
                                fundingType);
                        })
                    .ToList();
        }

        public string CatalogueItemName { get; set; }

        public CallOffId CallOffId { get; set; }

        public OrderItemFundingType SelectedFundingType { get; set; } = OrderItemFundingType.None;

        public List<SelectOption<OrderItemFundingType>> AvailableFundingTypes { get; set; }

        public decimal TotalCost { get; set; }

        private static string GetFundingTypeHintText(OrderItemFundingType fundingType) => fundingType switch
        {
            OrderItemFundingType.Gpit => "Use central funding from your GPIT allocation.",
            OrderItemFundingType.Pcarp => "Use central funding from your PCARP allocation.",
            OrderItemFundingType.LocalFunding => "Use local funding to pay for this item.",
            _ => throw new ArgumentOutOfRangeException(nameof(fundingType)),
        };
    }
}
