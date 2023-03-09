﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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
        public static void CalculateTotalCost_FlatPrice_SingleFixed_Ignores_Quantity(IFixture fixture)
        {
            var quantity = 3587;
            var price = 3.14M;
            var calculationType = CataloguePriceCalculationType.SingleFixed;

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem orderItem = BuildOrderItem(fixture, quantity, new[] { tier }, calculationType);

            var result = orderItem.OrderItemPrice.CalculateTotalCost(orderItem.TotalQuantity);

            result.Should().Be(price);
        }

        [Theory]
        [CommonAutoData]
        public static void CalculateTotalCostPerTier_FlatPrice_SingleFixed_Ignores_Quantity(IFixture fixture)
        {
            var quantity = 3587;
            var price = 3.14M;
            var calculationType = CataloguePriceCalculationType.SingleFixed;

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem orderItem = BuildOrderItem(fixture, quantity, new[] { tier }, calculationType);

            var result = orderItem.OrderItemPrice.CalculateTotalCostPerTier(orderItem.TotalQuantity);

            var expected = new PriceCalculationModel(1, quantity, price);

            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(new List<PriceCalculationModel>() { expected });
        }

        [Theory]
        [CommonAutoData]
        public static void CalculateTotalCost_FlatPrice_Volume_Uses_Quantity(IFixture fixture)
        {
            var quantity = 3587;
            var price = 3.14M;
            var calculationType = CataloguePriceCalculationType.Volume;

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem orderItem = BuildOrderItem(fixture, quantity, new[] { tier }, calculationType);

            var result = orderItem.OrderItemPrice.CalculateTotalCost(orderItem.TotalQuantity);

            result.Should().Be(quantity * price);
        }

        [Theory]
        [CommonAutoData]
        public static void CalculateTotalCostPerTier_FlatPrice_Volume_Uses_Quantity(IFixture fixture)
        {
            var quantity = 3587;
            var price = 3.14M;
            var calculationType = CataloguePriceCalculationType.Volume;

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem orderItem = BuildOrderItem(fixture, quantity, new[] { tier }, calculationType);

            var result = orderItem.OrderItemPrice.CalculateTotalCostPerTier(orderItem.TotalQuantity);

            var expected = new PriceCalculationModel(1, quantity, quantity * price);

            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(new List<PriceCalculationModel>() { expected });
        }

        [Theory]
        [CommonInlineAutoData(500, 3.14)]
        [CommonInlineAutoData(3587, 2)]
        [CommonInlineAutoData(7210, 1.5)]
        public static void CalculateTotalCost_Tiered_SingleFixed_Ignores_Quantity(
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

            var result = orderItem.OrderItemPrice.CalculateTotalCost(orderItem.TotalQuantity);

            result.Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(500, 1, 3.14)]
        [CommonInlineAutoData(3587, 2, 2)]
        [CommonInlineAutoData(7210, 3, 1.5)]
        public static void CalculateTotalCostPerTier_Tiered_SingleFixed_Ignores_Quantity(
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

            var result = orderItem.OrderItemPrice.CalculateTotalCostPerTier(orderItem.TotalQuantity);

            var expectedTemplate = GetCostTiersTemplate(tierId, quantity, expectedCost);

            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(expectedTemplate);
        }

        [Theory]
        [CommonInlineAutoData(500, 500 * 3.14)]
        [CommonInlineAutoData(3587, 3587 * 2)]
        [CommonInlineAutoData(7210, 7210 * 1.5)]
        public static void CalculateTotalCost_Tiered_Volume_Uses_Quantity(
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

            var result = orderItem.OrderItemPrice.CalculateTotalCost(orderItem.TotalQuantity);

            result.Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(500, 1, 500 * 3.14)]
        [CommonInlineAutoData(3587, 2, 3587 * 2)]
        [CommonInlineAutoData(7210, 3, 7210 * 1.5)]
        public static void CalculateTotalCostPerTier_Tiered_Volume_Uses_Quantity(
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

            var result = orderItem.OrderItemPrice.CalculateTotalCostPerTier(orderItem.TotalQuantity);

            var expectedTemplate = GetCostTiersTemplate(tierId, quantity, expectedCost);

            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(expectedTemplate);
        }

        [Theory]
        [CommonInlineAutoData(500, 500 * 3.14)]
        [CommonInlineAutoData(1000, (999 * 3.14) + (1 * 2))]
        [CommonInlineAutoData(3587, (999 * 3.14) + (2588 * 2))]
        [CommonInlineAutoData(4999, (999 * 3.14) + (4000 * 2))]
        [CommonInlineAutoData(7210, (999 * 3.14) + (4000 * 2) + (2211 * 1.5))]
        [CommonInlineAutoData(10000, (999 * 3.14) + (4000 * 2) + (5001 * 1.5))]
        public static void CalculateTotalCost_Tiered_Cumulative_Splits_Quantity_By_Tiers(
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

            var result = orderItem.OrderItemPrice.CalculateTotalCost(orderItem.TotalQuantity);

            result.Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(500, new[] { 500 })]
        [CommonInlineAutoData(1000, new[] { 999, 1 })]
        [CommonInlineAutoData(3587, new[] { 999, 2588 })]
        [CommonInlineAutoData(4999, new[] { 999, 4000 })]
        [CommonInlineAutoData(7210, new[] { 999, 4000, 2211 })]
        [CommonInlineAutoData(10000, new[] { 999, 4000, 5001 })]
        public static void CalculateTotalCostPerTier_Tiered_Cumulative_Splits_Quantity_By_Tiers(
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

            var result = orderItem.OrderItemPrice.CalculateTotalCostPerTier(orderItem.TotalQuantity);

            var costs = new decimal[] { 3.14M, 2M, 1.5M };
            var expectedTemplate = GetCostTiersTemplate(quantitySplits, costs);

            result.Should()
                .NotBeEmpty()
                .And.BeEquivalentTo(expectedTemplate);
        }

        [Theory]
        [CommonInlineAutoData(TimeUnit.PerMonth, 0, 12, 12 * 12)]
        [CommonInlineAutoData(TimeUnit.PerYear, 0, 1, 12)]
        [CommonInlineAutoData(null, 12, 0, 0)]
        public static void Order_Totals_Using_BillingPeriod(TimeUnit? billingPeriod, decimal oneOff, decimal monthly, decimal annual, IFixture fixture)
        {
            var price = 12M;

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem orderItem = BuildOrderItem(fixture, 1, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            orderItem.OrderItemPrice.BillingPeriod = billingPeriod;

            var order = fixture.Build<Order>()
                .With(o => o.OrderItems, new HashSet<OrderItem> { orderItem })
                .Create();

            order.TotalOneOffCost().Should().Be(oneOff);
            order.TotalMonthlyCost().Should().Be(monthly);
            order.TotalAnnualCost().Should().Be(annual);
        }

        [Theory]
        [CommonInlineAutoData(TimeUnit.PerMonth, 12 * 24)]
        [CommonInlineAutoData(TimeUnit.PerYear, 1 * 24)]
        [CommonInlineAutoData(null, 12)]
        public static void Order_TotalCost_PerMonth_And_PerYear_Use_MaximumTerm_But_OneOff_Costs_Dont(TimeUnit? billingPeriod, decimal total, IFixture fixture)
        {
            var maximumTerm = 24;
            var price = 12M;

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem orderItem = BuildOrderItem(fixture, 1, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            orderItem.OrderItemPrice.BillingPeriod = billingPeriod;

            var order = fixture.Build<Order>()
                .With(o => o.OrderItems, new HashSet<OrderItem> { orderItem })
                .With(o => o.MaximumTerm, maximumTerm)
                .Create();

            order.TotalCost().Should().Be(total);
        }

        [Theory]
        [CommonAutoData]
        public static void Order_TotalCost_Is_Sum_of_OrderItem_Costs(IFixture fixture)
        {
            var maximumTerm = 24;
            var price = 12M;

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem oneOffCostOrderItem = BuildOrderItem(fixture, 1, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            oneOffCostOrderItem.OrderItemPrice.BillingPeriod = null;

            OrderItem perMonthOrderItem = BuildOrderItem(fixture, 1, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            perMonthOrderItem.OrderItemPrice.BillingPeriod = TimeUnit.PerMonth;

            var order = fixture.Build<Order>()
                .With(o => o.OrderItems, new HashSet<OrderItem> { oneOffCostOrderItem, perMonthOrderItem })
                .With(o => o.MaximumTerm, maximumTerm)
                .Create();

            order.TotalCost().Should().Be(12 + (12 * 24));
        }

        [Theory]
        [CommonAutoData]
        public static void OrderWrapper_TotalCost(IFixture fixture)
        {
            var maximumTerm = 12;
            var price = 12M;
            var commencementDate = new DateTime(2000, 1, 1);
            var amendmentPlannedDelivery = commencementDate.AddMonths(6);

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem orderItem = BuildOrderItem(fixture, 1, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            orderItem.OrderItemPrice.BillingPeriod = TimeUnit.PerMonth;

            OrderItem amendedOrderItem = BuildOrderItem(fixture, 1, new[] { tier }, CataloguePriceCalculationType.SingleFixed, amendmentPlannedDelivery);
            amendedOrderItem.OrderItemPrice.BillingPeriod = TimeUnit.PerMonth;

            var order = fixture.Build<Order>()
                .With(o => o.Revision, 1)
                .With(o => o.OrderTriageValue, OrderTriageValue.Under40K)
                .With(o => o.CommencementDate, new DateTime(2000, 1, 1))
                .With(o => o.OrderItems, new HashSet<OrderItem> { orderItem })
                .With(o => o.MaximumTerm, maximumTerm)
                .Create();

            var amendedOrder = order.BuidAmendment(2);
            amendedOrder.OrderItems = new HashSet<OrderItem> { amendedOrderItem };

            var orderWrapper = new OrderWrapper(new[] { order, amendedOrder });

            orderWrapper.TotalCost().Should().Be((12 * 12) + (12 * 6));
        }

        private static List<PriceCalculationModel> GetCostTiersTemplate(int[] quantitySplits, decimal[] costs)
        {
            var template = new List<PriceCalculationModel>()
            {
                new(1, 0, 0M),
                new(2, 0, 0M),
                new(3, 0, 0M),
            };

            quantitySplits.ForEach((q, i) =>
            {
                template[i].Quantity = q;
                template[i].Cost = costs[i] * q;
            });

            return template;
        }

        private static List<PriceCalculationModel> GetCostTiersTemplate(int id, int quantity, decimal cost)
        {
            var template = new List<PriceCalculationModel>()
            {
                new(1, 0, 0M),
                new(2, 0, 0M),
                new(3, 0, 0M),
            };

            var tier = template.First(p => p.Id == id);
            tier.Quantity = quantity;
            tier.Cost = cost;

            return template;
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
                .Create();

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
