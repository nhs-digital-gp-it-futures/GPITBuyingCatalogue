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

            OrderItem orderItem = BuildOrderItem(fixture, new[] { tier }, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostForBillingPeriod(quantity);

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

            OrderItem orderItem = BuildOrderItem(fixture, new[] { tier }, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostPerTierForBillingPeriod(quantity);

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

            OrderItem orderItem = BuildOrderItem(fixture, new[] { tier }, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostForBillingPeriod(quantity);

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

            OrderItem orderItem = BuildOrderItem(fixture, new[] { tier }, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostPerTierForBillingPeriod(quantity);

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

            OrderItem orderItem = BuildOrderItem(fixture, tiers, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostForBillingPeriod(quantity);

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

            OrderItem orderItem = BuildOrderItem(fixture, tiers, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostPerTierForBillingPeriod(quantity);

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

            OrderItem orderItem = BuildOrderItem(fixture, tiers, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostForBillingPeriod(quantity);

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

            OrderItem orderItem = BuildOrderItem(fixture, tiers, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostPerTierForBillingPeriod(quantity);

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

            OrderItem orderItem = BuildOrderItem(fixture, tiers, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostForBillingPeriod(quantity);

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

            OrderItem orderItem = BuildOrderItem(fixture, tiers, calculationType);

            var result = ((IPrice)orderItem.OrderItemPrice).CostPerTierForBillingPeriod(quantity);

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

            OrderItem orderItem = BuildOrderItem(fixture, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            orderItem.OrderItemPrice.BillingPeriod = billingPeriod;
            var recipient = fixture.Build<OrderRecipient>()
                 .Create();
            recipient.SetQuantityForItem(orderItem.CatalogueItemId, 1);

            var order = fixture.Build<Order>()
                .With(o => o.OrderItems, new HashSet<OrderItem> { orderItem })
                .With(o => o.OrderRecipients, new HashSet<OrderRecipient> { recipient })
                .Create();

            order.TotalOneOffCost(null).Should().Be(oneOff);
            order.TotalOneOffCost(null, true).Should().Be(Math.Round(oneOff, 2, MidpointRounding.AwayFromZero));
            order.TotalMonthlyCost(null).Should().Be(monthly);
            order.TotalMonthlyCost(null, true).Should().Be(Math.Round(monthly, 2, MidpointRounding.AwayFromZero));
            order.TotalAnnualCost(null).Should().Be(annual);
            order.TotalAnnualCost(null, true).Should().Be(Math.Round(annual, 2, MidpointRounding.AwayFromZero));
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

            OrderItem orderItem = BuildOrderItem(fixture, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            orderItem.OrderItemPrice.BillingPeriod = billingPeriod;
            var recipient = fixture.Build<OrderRecipient>()
                 .Create();
            recipient.SetQuantityForItem(orderItem.CatalogueItemId, 1);

            var order = fixture.Build<Order>()
                .With(o => o.Revision, 1)
                .With(o => o.OrderItems, new HashSet<OrderItem> { orderItem })
                .With(o => o.OrderRecipients, new HashSet<OrderRecipient> { recipient })
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

            OrderItem oneOffCostOrderItem = BuildOrderItem(fixture, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            oneOffCostOrderItem.OrderItemPrice.BillingPeriod = null;

            OrderItem perMonthOrderItem = BuildOrderItem(fixture, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            perMonthOrderItem.OrderItemPrice.BillingPeriod = TimeUnit.PerMonth;
            var recipient = fixture.Build<OrderRecipient>()
                 .Create();
            recipient.SetQuantityForItem(oneOffCostOrderItem.CatalogueItemId, 1);
            recipient.SetQuantityForItem(perMonthOrderItem.CatalogueItemId, 1);

            var order = fixture.Build<Order>()
                .With(o => o.Revision, 1)
                .With(o => o.OrderItems, new HashSet<OrderItem>(new[] { oneOffCostOrderItem, perMonthOrderItem }))
                .With(o => o.OrderRecipients, new HashSet<OrderRecipient> { recipient })
                .With(o => o.MaximumTerm, maximumTerm)
                .Create();

            var orderWrapper = new OrderWrapper(order);
            orderWrapper.TotalCost().Should().Be(12 + (12 * 24));
        }

        [Theory]
        [CommonInlineAutoData(1, 12 * 24)]
        [CommonInlineAutoData(2, 12 * 18)]
        public static void Order_TotalCostForOrderItem_Amended_Orders_Use_The_Planned_Delivery_date_Original_Orders_Dont(
            int revision,
            decimal total,
            IFixture fixture)
        {
            var maximumTerm = 24;
            var price = 12M;
            var commencementDate = new DateTime(2000, 1, 1);
            var amendmentPlannedDelivery = commencementDate.AddMonths(6);

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem perMonthOrderItemUsedForTotal = BuildOrderItem(fixture, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            perMonthOrderItemUsedForTotal.CatalogueItemId = perMonthOrderItemUsedForTotal.CatalogueItem.Id;
            perMonthOrderItemUsedForTotal.OrderItemPrice.BillingPeriod = TimeUnit.PerMonth;
            var recipient = fixture.Build<OrderRecipient>()
                 .Create();
            recipient.SetQuantityForItem(perMonthOrderItemUsedForTotal.CatalogueItemId, 1);
            recipient.SetDeliveryDateForItem(perMonthOrderItemUsedForTotal.CatalogueItemId, amendmentPlannedDelivery);

            var order = fixture.Build<Order>()
                .With(o => o.Revision, revision)
                .With(o => o.CommencementDate, commencementDate)
                .With(o => o.OrderItems, new HashSet<OrderItem>(new[] { perMonthOrderItemUsedForTotal }))
                .With(o => o.OrderRecipients, new HashSet<OrderRecipient> { recipient })
                .With(o => o.MaximumTerm, maximumTerm)
                .Create();

            var orderWrapper = new OrderWrapper(order);

            orderWrapper.TotalCostForOrderItem(perMonthOrderItemUsedForTotal.CatalogueItem.Id).Should().Be(total);
        }

        [Theory]
        [CommonInlineAutoData(1, 12 * 24)]
        [CommonInlineAutoData(2, 12 * 18)]
        public static void Order_TotalCostForOrderItem_AmendedOrder_PerServiceRecipient(
            int revision,
            decimal total,
            IFixture fixture)
        {
            var maximumTerm = 24;
            var price = 12M;
            var commencementDate = new DateTime(2000, 1, 1);
            var amendmentPlannedDelivery = commencementDate.AddMonths(6);

            (decimal Price, int LowerRange, int? UpperRange) tier = (price, 1, null);

            OrderItem perMonthOrderItemUsedForTotal = BuildOrderItem(fixture, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            perMonthOrderItemUsedForTotal.CatalogueItemId = perMonthOrderItemUsedForTotal.CatalogueItem.Id;
            perMonthOrderItemUsedForTotal.OrderItemPrice.BillingPeriod = TimeUnit.PerMonth;
            var recipient = fixture.Build<OrderRecipient>()
                .Create();
            recipient.SetQuantityForItem(perMonthOrderItemUsedForTotal.CatalogueItemId, 1);
            recipient.SetDeliveryDateForItem(perMonthOrderItemUsedForTotal.CatalogueItemId, amendmentPlannedDelivery);

            var recipient2 = fixture.Build<OrderRecipient>()
                .Create();
            recipient2.SetQuantityForItem(perMonthOrderItemUsedForTotal.CatalogueItemId, 1);
            recipient2.SetDeliveryDateForItem(perMonthOrderItemUsedForTotal.CatalogueItemId, amendmentPlannedDelivery);

            var order = fixture.Build<Order>()
                .With(o => o.Revision, revision)
                .With(o => o.CommencementDate, commencementDate)
                .With(o => o.OrderItems, new HashSet<OrderItem>(new[] { perMonthOrderItemUsedForTotal }))
                .With(o => o.OrderRecipients, new HashSet<OrderRecipient> { recipient, recipient2 })
                .With(o => o.MaximumTerm, maximumTerm)
                .Create();

            var orderWrapper = new OrderWrapper(order);

            orderWrapper.TotalCostForOrderItem(perMonthOrderItemUsedForTotal.CatalogueItem.Id).Should().Be(total);
        }

        [Theory]
        [CommonInlineAutoData(1)]
        [CommonInlineAutoData(2)]
        public static void Order_TotalCostForOrderItem_Returns_0_When_OrderItem_Not_Found(int revision, CatalogueItemId catalogueItemId, IFixture fixture)
        {
            var order = fixture.Build<Order>()
                .With(o => o.Revision, revision)
                .Create();

            var orderWrapper = new OrderWrapper(order);
            orderWrapper.TotalCostForOrderItem(catalogueItemId).Should().Be(0);
        }

        [Theory]
        [CommonInlineAutoData(1)]
        [CommonInlineAutoData(2)]
        public static void Order_TotalCostForOrderItem_Returns_0_When_OrderItem_Has_No_Recipients(int revision, Order order)
        {
            order.Revision = revision;
            var orderItem = order.OrderItems.First();
            order.OrderRecipients.ForEach(r => r.OrderItemRecipients.Clear());
            var orderWrapper = new OrderWrapper(order);
            orderWrapper.TotalCostForOrderItem(orderItem.CatalogueItem.Id).Should().Be(0);
        }

        [Theory]
        [CommonAutoData]
        public static void OrderItem_TotalCost_Returns_0_When_No_Price(OrderItem orderItem)
        {
            orderItem.OrderItemPrice = null;

            orderItem.TotalCost(null).Should().Be(0);
        }

        [Theory]
        [CommonAutoData]
        public static void OrderItem_TotalCost_PerMonth_ReturnsExpected(
            OrderRecipient recpient,
            int quantity,
            OrderItem orderItem,
            OrderItemPrice orderItemPrice)
        {
            orderItemPrice.BillingPeriod = TimeUnit.PerMonth;
            orderItem.OrderItemPrice = orderItemPrice;
            recpient.OrderItemRecipients.Clear();
            recpient.SetQuantityForItem(orderItem.CatalogueItemId, quantity);

            var expectedResult = ((IPrice)orderItemPrice).CalculateCostPerMonth(quantity);

            orderItem.TotalCost(new[] { recpient }).Should().Be(expectedResult);
        }

        [Theory]
        [CommonAutoData]
        public static void OrderItem_TotalCost_PerYear_ReturnsExpected(
            OrderRecipient recpient,
            int quantity,
            OrderItem orderItem,
            OrderItemPrice orderItemPrice)
        {
            orderItemPrice.BillingPeriod = TimeUnit.PerYear;
            orderItem.OrderItemPrice = orderItemPrice;
            recpient.OrderItemRecipients.Clear();
            recpient.SetQuantityForItem(orderItem.CatalogueItemId, quantity);

            var expectedResult = ((IPrice)orderItemPrice).CalculateCostPerYear(quantity);

            orderItem.TotalCost(new[] { recpient }).Should().Be(expectedResult);
        }

        [Theory]
        [CommonAutoData]
        public static void OrderItem_TotalCost_OneOff_ReturnsExpected(
            OrderRecipient recpient,
            int quantity,
            OrderItem orderItem,
            OrderItemPrice orderItemPrice)
        {
            orderItemPrice.BillingPeriod = null;
            orderItem.OrderItemPrice = orderItemPrice;
            recpient.OrderItemRecipients.Clear();
            recpient.SetQuantityForItem(orderItem.CatalogueItemId, quantity);

            var expectedResult = ((IPrice)orderItemPrice).CalculateOneOffCost(quantity);

            orderItem.TotalCost(new[] { recpient }).Should().Be(expectedResult);
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

            OrderItem orderItem = BuildOrderItem(fixture, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            orderItem.OrderItemPrice.BillingPeriod = TimeUnit.PerMonth;
            var recipient = fixture.Build<OrderRecipient>()
                .Without(i => i.OrderItemRecipients)
                .Create();
            recipient.SetQuantityForItem(orderItem.CatalogueItemId, 1);

            var amendedRecipient = fixture.Build<OrderRecipient>()
                .Without(i => i.OrderItemRecipients)
                .Create();
            amendedRecipient.SetQuantityForItem(orderItem.CatalogueItemId, 1);
            amendedRecipient.SetDeliveryDateForItem(orderItem.CatalogueItemId, amendmentPlannedDelivery);

            Order order = BuildOrder(fixture, maximumTerm, new[] { orderItem }, commencementDate, new[] { recipient });

            var amendedOrder = order.BuildAmendment(2);
            amendedOrder.OrderItems = new HashSet<OrderItem> { orderItem };
            amendedOrder.OrderRecipients.Add(amendedRecipient);

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

            OrderItem orderItem = BuildOrderItem(fixture, new[] { tier }, CataloguePriceCalculationType.SingleFixed);
            orderItem.OrderItemPrice.BillingPeriod = TimeUnit.PerMonth;
            var recipient = fixture.Build<OrderRecipient>()
                .Without(i => i.OrderItemRecipients)
                .Create();
            recipient.SetQuantityForItem(orderItem.CatalogueItemId, 1);

            var revision2Recipient = fixture.Build<OrderRecipient>()
                .Without(i => i.OrderItemRecipients)
                .Create();
            revision2Recipient.SetQuantityForItem(orderItem.CatalogueItemId, 1);
            revision2Recipient.SetDeliveryDateForItem(orderItem.CatalogueItemId, revision2PlannedDelivery);

            var revision3Recipient = fixture.Build<OrderRecipient>()
                .Without(i => i.OrderItemRecipients)
                .Create();
            revision3Recipient.SetQuantityForItem(orderItem.CatalogueItemId, 1);
            revision3Recipient.SetDeliveryDateForItem(orderItem.CatalogueItemId, revision3PlannedDelivery);

            Order order = BuildOrder(fixture, maximumTerm, new[] { orderItem }, commencementDate, new[] { recipient });

            var revision2 = order.BuildAmendment(2);
            revision2.OrderItems = new HashSet<OrderItem> { orderItem };
            revision2.OrderRecipients = new HashSet<OrderRecipient> { revision2Recipient };

            var revision3 = order.BuildAmendment(3);
            revision3.OrderItems = new HashSet<OrderItem> { orderItem };
            revision3.OrderRecipients = new HashSet<OrderRecipient> { revision3Recipient };

            var orderWrapper = new OrderWrapper(new[] { order, revision2, revision3 });

            orderWrapper.TotalPreviousCost().Should().Be(expectedOriginalTotal + expectedRevision2Total);
            orderWrapper.TotalCost().Should().Be(expectedOriginalTotal + expectedRevision2Total + expectedRevision3Total);
        }

        private static Order BuildOrder(IFixture fixture, int maximumTerm, OrderItem[] orderItems, DateTime commencementDate, OrderRecipient[] recipients)
        {
            return fixture.Build<Order>()
                .With(o => o.Revision, 1)
                .With(o => o.OrderTriageValue, OrderTriageValue.Under40K)
                .With(o => o.CommencementDate, commencementDate)
                .With(o => o.OrderItems, new HashSet<OrderItem>(orderItems))
                .With(o => o.OrderRecipients, new HashSet<OrderRecipient>(recipients))
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
            (decimal Price, int LowerRange, int? UpperRange)[] tiers,
            CataloguePriceCalculationType cataloguePriceCalculationType)
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

            var orderItem = fixture.Build<OrderItem>()
                .Without(i => i.OrderItemFunding)
                .With(i => i.OrderItemPrice, itemPrice)
                .Create();

            return orderItem;
        }
    }
}
