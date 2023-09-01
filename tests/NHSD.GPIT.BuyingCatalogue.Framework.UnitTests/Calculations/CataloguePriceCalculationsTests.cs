using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Calculations
{
    public static class CataloguePriceCalculationsTests
    {
        [Theory]
        [CommonAutoData]
        public static void CostForBillingPeriod_FlatPrice_SingleFixed_Ignores_Quantity(IFixture fixture)
        {
            var quantity = 3587;
            var price = 3.14M;
            var calculationType = CataloguePriceCalculationType.SingleFixed;

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem orderItem = BuildOrderItem(fixture, quantity, new[] { tier }, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostForBillingPeriod(orderItem.TotalQuantity);

            result.Should().Be(price);
        }

        [Theory]
        [CommonAutoData]
        public static void CostPerTierForBillingPeriod_FlatPrice_SingleFixed_Ignores_Quantity(IFixture fixture)
        {
            var quantity = 3587;
            var cost = 3.14M;
            var price = 3.14M;
            var calculationType = CataloguePriceCalculationType.SingleFixed;

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem orderItem = BuildOrderItem(fixture, quantity, new[] { tier }, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostPerTierForBillingPeriod(orderItem.TotalQuantity);

            var expected = new PriceCalculationModel(1, quantity, price, cost);

            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(new List<PriceCalculationModel>() { expected });
        }

        [Theory]
        [CommonAutoData]
        public static void CostForBillingPeriod_FlatPrice_Volume_Uses_Quantity(IFixture fixture)
        {
            var quantity = 3587;
            var price = 3.14M;
            var calculationType = CataloguePriceCalculationType.Volume;

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem orderItem = BuildOrderItem(fixture, quantity, new[] { tier }, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostForBillingPeriod(orderItem.TotalQuantity);

            result.Should().Be(quantity * price);
        }

        [Theory]
        [CommonAutoData]
        public static void CostPerTierForBillingPeriod_FlatPrice_Volume_Uses_Quantity(IFixture fixture)
        {
            var quantity = 3587;
            var price = 3.14M;
            var cost = quantity * price;
            var calculationType = CataloguePriceCalculationType.Volume;

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem orderItem = BuildOrderItem(fixture, quantity, new[] { tier }, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostPerTierForBillingPeriod(orderItem.TotalQuantity);

            var expected = new PriceCalculationModel(1, quantity, price, cost);

            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(new List<PriceCalculationModel>() { expected });
        }

        [Theory]
        [CommonInlineAutoData(500, 3.14)]
        [CommonInlineAutoData(3587, 2)]
        [CommonInlineAutoData(7210, 1.5)]
        public static void CostForBillingPeriod_Tiered_SingleFixed_Ignores_Quantity(
            int quantity,
            decimal expected,
            IFixture fixture)
        {
            (decimal Price, int LowerRange, int? UpperRange)[] tiers =
            {
                (3.14M, 1, 999),
                (2M, 1000, 4999),
                (1.5M, 5000, null),
            };

            var calculationType = CataloguePriceCalculationType.SingleFixed;

            OrderItem orderItem = BuildOrderItem(fixture, quantity, tiers, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostForBillingPeriod(orderItem.TotalQuantity);

            result.Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(500, 1, 3.14)]
        [CommonInlineAutoData(3587, 2, 2)]
        [CommonInlineAutoData(7210, 3, 1.5)]
        public static void CostPerTierForBillingPeriod_Tiered_SingleFixed_Ignores_Quantity(
            int quantity,
            int tierId,
            decimal expectedCost,
            IFixture fixture)
        {
            (decimal Price, int LowerRange, int? UpperRange)[] tiers =
            {
                (3.14M, 1, 999),
                (2M, 1000, 4999),
                (1.5M, 5000, null),
            };

            var calculationType = CataloguePriceCalculationType.SingleFixed;

            OrderItem orderItem = BuildOrderItem(fixture, quantity, tiers, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostPerTierForBillingPeriod(orderItem.TotalQuantity);

            var template = new List<PriceCalculationModel>()
            {
                new(1, 0, 3.14M, 0M),
                new(2, 0, 2M, 0M),
                new(3, 0, 1.5M, 0M),
            };

            UpdateTier(template.First(p => p.Id == tierId), quantity, expectedCost);

            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(template);
        }

        [Theory]
        [CommonInlineAutoData(500, 500 * 3.14)]
        [CommonInlineAutoData(3587, 3587 * 2)]
        [CommonInlineAutoData(7210, 7210 * 1.5)]
        public static void CostForBillingPeriod_Tiered_Volume_Uses_Quantity(
            int quantity,
            decimal expected,
            IFixture fixture)
        {
            (decimal Price, int LowerRange, int? UpperRange)[] tiers =
            {
                (3.14M, 1, 999),
                (2M, 1000, 4999),
                (1.5M, 5000, null),
            };

            var calculationType = CataloguePriceCalculationType.Volume;

            OrderItem orderItem = BuildOrderItem(fixture, quantity, tiers, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostForBillingPeriod(orderItem.TotalQuantity);

            result.Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(500, 1, 500 * 3.14)]
        [CommonInlineAutoData(3587, 2, 3587 * 2)]
        [CommonInlineAutoData(7210, 3, 7210 * 1.5)]
        public static void CostPerTierForBillingPeriod_Tiered_Volume_Uses_Quantity(
            int quantity,
            int tierId,
            decimal expectedCost,
            IFixture fixture)
        {
            (decimal Price, int LowerRange, int? UpperRange)[] tiers =
            {
                (3.14M, 1, 999),
                (2M, 1000, 4999),
                (1.5M, 5000, null),
            };

            var calculationType = CataloguePriceCalculationType.Volume;

            OrderItem orderItem = BuildOrderItem(fixture, quantity, tiers, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostPerTierForBillingPeriod(orderItem.TotalQuantity);

            var template = new List<PriceCalculationModel>()
            {
                new(1, 0, 3.14M, 0M),
                new(2, 0, 2M, 0M),
                new(3, 0, 1.5M, 0M),
            };

            UpdateTier(template.First(p => p.Id == tierId), quantity, expectedCost);

            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(template);
        }

        [Theory]
        [CommonInlineAutoData(500, 500 * 3.14)]
        [CommonInlineAutoData(1000, (999 * 3.14) + (1 * 2))]
        [CommonInlineAutoData(3587, (999 * 3.14) + (2588 * 2))]
        [CommonInlineAutoData(4999, (999 * 3.14) + (4000 * 2))]
        [CommonInlineAutoData(7210, (999 * 3.14) + (4000 * 2) + (2211 * 1.5))]
        [CommonInlineAutoData(10000, (999 * 3.14) + (4000 * 2) + (5001 * 1.5))]
        public static void CostForBillingPeriod_Tiered_Cumulative_Splits_Quantity_By_Tiers(
            int quantity,
            decimal expected,
            IFixture fixture)
        {
            (decimal Price, int LowerRange, int? UpperRange)[] tiers =
            {
                (3.14M, 1, 999),
                (2M, 1000, 4999),
                (1.5M, 5000, null),
            };

            var calculationType = CataloguePriceCalculationType.Cumulative;

            OrderItem orderItem = BuildOrderItem(fixture, quantity, tiers, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostForBillingPeriod(orderItem.TotalQuantity);

            result.Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(500, new[] { 500 })]
        [CommonInlineAutoData(1000, new[] { 999, 1 })]
        [CommonInlineAutoData(3587, new[] { 999, 2588 })]
        [CommonInlineAutoData(4999, new[] { 999, 4000 })]
        [CommonInlineAutoData(7210, new[] { 999, 4000, 2211 })]
        [CommonInlineAutoData(10000, new[] { 999, 4000, 5001 })]
        public static void CostPerTierForBillingPeriod_Tiered_Cumulative_Splits_Quantity_By_Tiers(
            int quantity,
            int[] quantitySplits,
            IFixture fixture)
        {
            (decimal Price, int LowerRange, int? UpperRange)[] tiers =
            {
                (3.14M, 1, 999),
                (2M, 1000, 4999),
                (1.5M, 5000, null),
            };

            var calculationType = CataloguePriceCalculationType.Cumulative;

            OrderItem orderItem = BuildOrderItem(fixture, quantity, tiers, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostPerTierForBillingPeriod(orderItem.TotalQuantity);

            var template = new List<PriceCalculationModel>()
            {
                new(1, 0, 3.14M, 0M),
                new(2, 0, 2M, 0M),
                new(3, 0, 1.5M, 0M),
            };

            quantitySplits.ForEach((q, i) =>
            {
                template[i].Quantity = q;
                template[i].Cost = template[i].Price * q;
            });

            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(template);
        }

        [Theory]
        [CommonInlineAutoData(TimeUnit.PerMonth, 12, 0, 12, 12 * 12)]
        [CommonInlineAutoData(TimeUnit.PerYear, 12, 0, 1, 12)]
        [CommonInlineAutoData(TimeUnit.PerYear, 12.666, 0, 1.0555, 12.666)]
        [CommonInlineAutoData(null, 12, 12, 0, 0)]
        [CommonInlineAutoData(null, 12.666, 12.666, 0, 0)]
        public static void Order_Totals_Using_BillingPeriod(TimeUnit? billingPeriod, decimal price, decimal oneOff, decimal monthly, decimal annual, IFixture fixture)
        {
            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem orderItem = BuildOrderItem(fixture, 1, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            orderItem.OrderItemPrice.BillingPeriod = billingPeriod;

            var order = fixture.Build<Order>()
                .With(o => o.OrderItems, new HashSet<OrderItem> { orderItem })
                .Create();

            order.TotalOneOffCost().Should().Be(oneOff);
            order.TotalOneOffCost(true).Should().Be(Math.Round(oneOff, 2, MidpointRounding.AwayFromZero));
            order.TotalMonthlyCost().Should().Be(monthly);
            order.TotalMonthlyCost(true).Should().Be(Math.Round(monthly, 2, MidpointRounding.AwayFromZero));
            order.TotalAnnualCost().Should().Be(annual);
            order.TotalAnnualCost(true).Should().Be(Math.Round(annual, 2, MidpointRounding.AwayFromZero));
        }

        [Theory]
        [CommonInlineAutoData(TimeUnit.PerMonth, 12, 12 * 24)]
        [CommonInlineAutoData(TimeUnit.PerYear, 12, 1 * 24)]
        [CommonInlineAutoData(TimeUnit.PerYear, 12.666, 25.332)]
        [CommonInlineAutoData(null, 12, 12)]
        [CommonInlineAutoData(null, 12.666, 12.666)]
        public static void OrderWrapper_TotalCost_PerMonth_And_PerYear_Use_MaximumTerm_But_OneOff_Costs_Dont(TimeUnit? billingPeriod, decimal price, decimal total, IFixture fixture)
        {
            var maximumTerm = 24;

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem orderItem = BuildOrderItem(fixture, 1, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            orderItem.OrderItemPrice.BillingPeriod = billingPeriod;

            var order = fixture.Build<Order>()
                .With(o => o.Revision, 1)
                .With(o => o.OrderItems, new HashSet<OrderItem> { orderItem })
                .With(o => o.MaximumTerm, maximumTerm)
                .Create();

            var orderWrapper = new OrderWrapper(order);

            orderWrapper.TotalCost().Should().Be(total);
            orderWrapper.TotalCost(true).Should().Be(Math.Round(total, 2, MidpointRounding.AwayFromZero));
        }

        [Theory]
        [CommonAutoData]
        public static void OrderWrapper_TotalCost_Is_Sum_of_OrderItem_Costs(IFixture fixture)
        {
            var maximumTerm = 24;
            var price = 12M;

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem oneOffCostOrderItem = BuildOrderItem(fixture, 1, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            oneOffCostOrderItem.OrderItemPrice.BillingPeriod = null;

            OrderItem perMonthOrderItem = BuildOrderItem(fixture, 1, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            perMonthOrderItem.OrderItemPrice.BillingPeriod = TimeUnit.PerMonth;

            var order = fixture.Build<Order>()
                .With(o => o.Revision, 1)
                .With(o => o.OrderItems, new HashSet<OrderItem>(new[] { oneOffCostOrderItem, perMonthOrderItem }))
                .With(o => o.MaximumTerm, maximumTerm)
                .Create();

            var orderWrapper = new OrderWrapper(order);
            orderWrapper.TotalCost().Should().Be(12 + (12 * 24));
        }

        [Theory]
        [CommonInlineAutoData(1, 12 * 24)]
        [CommonInlineAutoData(2, 12 * 18)]
        public static void Order_TotalCostForOrderItem_Amended_Orders_Use_The_Planned_Delivery_date_Original_Orders_Dont(int revision, decimal total, IFixture fixture)
        {
            var maximumTerm = 24;
            var price = 12M;
            var commencementDate = new DateTime(2000, 1, 1);
            var amendmentPlannedDelivery = commencementDate.AddMonths(6);

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem perMonthOrderItemUsedForTotal = BuildOrderItem(fixture, 1, new[] { tier }, CataloguePriceCalculationType.SingleFixed, amendmentPlannedDelivery);
            perMonthOrderItemUsedForTotal.OrderItemPrice.BillingPeriod = TimeUnit.PerMonth;

            var order = fixture.Build<Order>()
                .With(o => o.Revision, revision)
                .With(o => o.CommencementDate, commencementDate)
                .With(o => o.OrderItems, new HashSet<OrderItem>(new[] { perMonthOrderItemUsedForTotal }))
                .With(o => o.MaximumTerm, maximumTerm)
                .Create();

            order.TotalCostForOrderItem(perMonthOrderItemUsedForTotal.CatalogueItem.Id).Should().Be(total);
        }

        [Theory]
        [CommonInlineAutoData(1)]
        [CommonInlineAutoData(2)]
        public static void Order_TotalCostForOrderItem_Returns_0_When_OrderItem_Not_Found(int revision, CatalogueItemId catalogueItemId, IFixture fixture)
        {
            var order = fixture.Build<Order>()
                .With(o => o.Revision, revision)
                .Create();

            order.TotalCostForOrderItem(catalogueItemId).Should().Be(0);
        }

        [Theory]
        [CommonInlineAutoData(1)]
        [CommonInlineAutoData(2)]
        public static void Order_TotalCostForOrderItem_Returns_0_When_OrderItem_Has_No_Recipients(int revision, OrderItem orderItem, IFixture fixture)
        {
            orderItem.OrderItemRecipients.Clear();

            var order = fixture.Build<Order>()
                .With(o => o.Revision, revision)
                .With(o => o.OrderItems, new HashSet<OrderItem>(new[] { orderItem }))
                .Create();

            order.TotalCostForOrderItem(orderItem.CatalogueItem.Id).Should().Be(0);
        }

        [Theory]
        [CommonAutoData]
        public static void OrderItem_TotalCost_Returns_0_When_No_Price(OrderItem orderItem)
        {
            orderItem.OrderItemPrice = null;

            orderItem.TotalCost().Should().Be(0);
        }

        [Fact]
        public static void OrderWrapper_Null_TotalPreviousCost_Returns_0()
        {
            ((OrderWrapper)null).TotalPreviousCost().Should().Be(0);
        }

        [Fact]
        public static void OrderWrapper_Null_TotalCost_Returns_0()
        {
            ((OrderWrapper)null).TotalCost().Should().Be(0);
        }

        [Theory]
        [CommonAutoData]
        public static void OrderWrapper_TotalCost_And_TotalPreviousCost_One_Amendment(IFixture fixture)
        {
            var maximumTerm = 12;
            var price = 12M;
            var commencementDate = new DateTime(2000, 1, 1);
            var amendmentPlannedDelivery = commencementDate.AddMonths(6);
            var expectedOriginalTotal = 12 * 12;
            var expectedAmendmentTotal = 12 * 6;

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem orderItem = BuildOrderItem(fixture, 1, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            orderItem.OrderItemPrice.BillingPeriod = TimeUnit.PerMonth;

            OrderItem amendedOrderItem = BuildOrderItem(fixture, 1, new[] { tier }, CataloguePriceCalculationType.SingleFixed, amendmentPlannedDelivery);
            amendedOrderItem.OrderItemPrice.BillingPeriod = TimeUnit.PerMonth;

            Order order = BuildOrder(fixture, maximumTerm, new[] { orderItem }, commencementDate);

            var amendedOrder = order.BuildAmendment(2);
            amendedOrder.OrderItems = new HashSet<OrderItem> { amendedOrderItem };

            var orderWrapper = new OrderWrapper(new[] { order, amendedOrder });

            orderWrapper.TotalPreviousCost().Should().Be(expectedOriginalTotal);
            orderWrapper.TotalCost().Should().Be(expectedOriginalTotal + expectedAmendmentTotal);
        }

        [Theory]
        [CommonAutoData]
        public static void OrderWrapper_TotalCost_And_TotalPreviousCosts_Multiple_Amendments(IFixture fixture)
        {
            var maximumTerm = 12;
            var price = 12M;
            var commencementDate = new DateTime(2000, 1, 1);
            var revision2PlannedDelivery = commencementDate.AddMonths(6);
            var revision3PlannedDelivery = commencementDate.AddMonths(9);
            var expectedOriginalTotal = 12 * 12;
            var expectedRevision2Total = 12 * 6;
            var expectedRevision3Total = 12 * 3;

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem orderItem = BuildOrderItem(fixture, 1, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            orderItem.OrderItemPrice.BillingPeriod = TimeUnit.PerMonth;

            OrderItem revision2OrderItem = BuildOrderItem(fixture, 1, new[] { tier }, CataloguePriceCalculationType.SingleFixed, revision2PlannedDelivery);
            revision2OrderItem.OrderItemPrice.BillingPeriod = TimeUnit.PerMonth;

            OrderItem revision3OrderItem = BuildOrderItem(fixture, 1, new[] { tier }, CataloguePriceCalculationType.SingleFixed, revision3PlannedDelivery);
            revision3OrderItem.OrderItemPrice.BillingPeriod = TimeUnit.PerMonth;

            Order order = BuildOrder(fixture, maximumTerm, new[] { orderItem }, commencementDate);

            var revision2 = order.BuildAmendment(2);
            revision2.OrderItems = new HashSet<OrderItem> { revision2OrderItem };

            var revision3 = order.BuildAmendment(3);
            revision3.OrderItems = new HashSet<OrderItem> { revision3OrderItem };

            var orderWrapper = new OrderWrapper(new[] { order, revision2, revision3 });

            orderWrapper.TotalPreviousCost().Should().Be(expectedOriginalTotal + expectedRevision2Total);
            orderWrapper.TotalCost().Should().Be(expectedOriginalTotal + expectedRevision2Total + expectedRevision3Total);
        }

        private static Order BuildOrder(IFixture fixture, int maximumTerm, OrderItem[] orderItems, DateTime commencementDate)
        {
            return fixture.Build<Order>()
                .With(o => o.Revision, 1)
                .With(o => o.OrderTriageValue, OrderTriageValue.Under40K)
                .With(o => o.CommencementDate, commencementDate)
                .With(o => o.OrderItems, new HashSet<OrderItem>(orderItems))
                .With(o => o.MaximumTerm, maximumTerm)
                .Create();
        }

        private static void UpdateTier(PriceCalculationModel tier, int quantity, decimal cost)
        {
            tier.Quantity = quantity;
            tier.Cost = cost;
        }

        private static OrderItem BuildOrderItem(
            IFixture fixture,
            int quantity,
            (decimal Price, int LowerRange, int? UpperRange)[] tiers,
            CataloguePriceCalculationType cataloguePriceCalculationType,
            DateTime? plannedDeliveryDate = null)
        {
            var priceTiers = tiers.Select(tier => fixture.Build<OrderItemPriceTier>()
                .Without(t => t.OrderItemPrice)
                .With(t => t.Price, tier.Price)
                .With(t => t.LowerRange, tier.LowerRange)
                .With(t => t.UpperRange, tier.UpperRange)
                .Create());

            var itemPrice = fixture.Build<OrderItemPrice>()
                .Without(p => p.OrderItem)
                .With(p => p.CataloguePriceCalculationType, cataloguePriceCalculationType)
                .With(p => p.ProvisioningType, ProvisioningType.Patient)
                .With(p => p.OrderItemPriceTiers, new HashSet<OrderItemPriceTier>(priceTiers))
                .Create() as IPrice;

            var recipientBuilder = fixture.Build<OrderItemRecipient>()
                .With(r => r.Quantity, itemPrice.IsPerServiceRecipient() ? quantity : null);

            if (plannedDeliveryDate.HasValue)
            {
                recipientBuilder = recipientBuilder.With(r => r.DeliveryDate, plannedDeliveryDate.Value);
            }

            var recipient = recipientBuilder.Create();

            var orderItem = fixture.Build<OrderItem>()
                .Without(i => i.OrderItemFunding)
                .With(i => i.OrderItemPrice, itemPrice)
                .With(i => i.Quantity, itemPrice.IsPerServiceRecipient() ? null : quantity)
                .With(i => i.OrderItemRecipients, new HashSet<OrderItemRecipient> { recipient })
                .Create();

            return orderItem;
        }
    }
}
