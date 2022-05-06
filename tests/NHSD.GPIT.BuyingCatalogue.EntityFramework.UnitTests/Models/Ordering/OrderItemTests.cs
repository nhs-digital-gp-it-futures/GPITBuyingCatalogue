using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Ordering
{
    public static class OrderItemTests
    {
        [Theory]
        [CommonAutoData]
        public static void CalculateTotalCost_FlatPrice_SingleFixed(
            CataloguePrice price,
            CataloguePriceTier tier,
            OrderItem orderItem,
            OrderItemRecipient orderItemRecipient)
        {
            tier.Price = 3.14M;
            tier.LowerRange = 1;
            tier.UpperRange = null;

            price.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;
            price.CataloguePriceTiers = new HashSet<CataloguePriceTier> { tier };

            orderItemRecipient.Quantity = 3587;

            orderItem.OrderItemRecipients = new HashSet<OrderItemRecipient> { orderItemRecipient };
            orderItem.OrderItemPrice = new(price);

            var result = orderItem.CalculateTotalCost();

            result.Should().Be(3.14M);
        }

        [Theory]
        [CommonAutoData]
        public static void CalculateTotalCost_FlatPrice_Volume(
            CataloguePrice price,
            CataloguePriceTier tier,
            OrderItem orderItem,
            OrderItemRecipient orderItemRecipient)
        {
            tier.Price = 3.14M;
            tier.LowerRange = 1;
            tier.UpperRange = null;

            price.CataloguePriceCalculationType = CataloguePriceCalculationType.Volume;
            price.CataloguePriceTiers = new HashSet<CataloguePriceTier> { tier };

            orderItemRecipient.Quantity = 3587;

            orderItem.OrderItemRecipients = new HashSet<OrderItemRecipient> { orderItemRecipient };
            orderItem.OrderItemPrice = new(price);

            var result = orderItem.CalculateTotalCost();

            result.Should().Be(11263.18M);
        }

        [Theory]
        [CommonInlineAutoData(500, 3.14)]
        [CommonInlineAutoData(3587, 2)]
        [CommonInlineAutoData(7210, 1.5)]
        public static void CalculateTotalCost_Tiered_SingleFixed(
            int quantity,
            decimal expected,
            CataloguePrice price,
            OrderItem orderItem,
            OrderItemRecipient orderItemRecipient)
        {
            price.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;
            price.CataloguePriceTiers = new HashSet<CataloguePriceTier>
            {
                new()
                {
                    Price = 3.14M,
                    LowerRange = 1,
                    UpperRange = 999,
                },
                new()
                {
                    Price = 2M,
                    LowerRange = 1000,
                    UpperRange = 4999,
                },
                new()
                {
                    Price = 1.5M,
                    LowerRange = 5000,
                    UpperRange = null,
                },
            };

            orderItemRecipient.Quantity = quantity;

            orderItem.OrderItemRecipients = new HashSet<OrderItemRecipient> { orderItemRecipient };
            orderItem.OrderItemPrice = new(price);

            var result = orderItem.CalculateTotalCost();

            result.Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(500, 1570)]
        [CommonInlineAutoData(1000, 3138.86)]
        [CommonInlineAutoData(3587, 8312.86)]
        [CommonInlineAutoData(4999, 11136.86)]
        [CommonInlineAutoData(7210, 14453.36)]
        [CommonInlineAutoData(10000, 18638.36)]
        public static void CalculateTotalCost_Tiered_Cumulative(
            int quantity,
            decimal expected,
            CataloguePrice price,
            OrderItem orderItem,
            OrderItemRecipient orderItemRecipient)
        {
            price.CataloguePriceCalculationType = CataloguePriceCalculationType.Cumulative;
            price.CataloguePriceTiers = new HashSet<CataloguePriceTier>
            {
                new()
                {
                    Price = 3.14M,
                    LowerRange = 1,
                    UpperRange = 999,
                },
                new()
                {
                    Price = 2M,
                    LowerRange = 1000,
                    UpperRange = 4999,
                },
                new()
                {
                    Price = 1.5M,
                    LowerRange = 5000,
                    UpperRange = null,
                },
            };

            orderItemRecipient.Quantity = quantity;

            orderItem.OrderItemRecipients = new HashSet<OrderItemRecipient> { orderItemRecipient };
            orderItem.OrderItemPrice = new(price);

            var result = orderItem.CalculateTotalCost();

            result.Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(500, 1570)]
        [CommonInlineAutoData(3587, 7174)]
        [CommonInlineAutoData(7210, 10815)]
        public static void CalculateTotalCost_Tiered_Volume(
            int quantity,
            decimal expected,
            CataloguePrice price,
            OrderItem orderItem,
            OrderItemRecipient orderItemRecipient)
        {
            price.CataloguePriceCalculationType = CataloguePriceCalculationType.Volume;
            price.CataloguePriceTiers = new HashSet<CataloguePriceTier>
            {
                new()
                {
                    Price = 3.14M,
                    LowerRange = 1,
                    UpperRange = 999,
                },
                new()
                {
                    Price = 2M,
                    LowerRange = 1000,
                    UpperRange = 4999,
                },
                new()
                {
                    Price = 1.5M,
                    LowerRange = 5000,
                    UpperRange = null,
                },
            };

            orderItemRecipient.Quantity = quantity;

            orderItem.OrderItemRecipients = new HashSet<OrderItemRecipient> { orderItemRecipient };
            orderItem.OrderItemPrice = new(price);

            var result = orderItem.CalculateTotalCost();

            result.Should().Be(expected);
        }
    }
}
