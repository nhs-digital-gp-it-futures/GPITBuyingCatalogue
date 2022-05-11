using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Calculations
{
    public static class CataloguePriceCalculationsTests
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

            var result = orderItem.OrderItemPrice.CalculateTotalCost(orderItem.GetTotalRecipientQuantity());

            result.Should().Be(3.14M);
        }

        [Theory]
        [CommonAutoData]
        public static void CalculateTotalCostPerTier_FlatPrice_SingleFixed(
            CataloguePrice price,
            CataloguePriceTier tier,
            OrderItem orderItem,
            OrderItemRecipient orderItemRecipient)
        {
            var expectedQuantity = 3587;
            var expectedPrice = 3.14M;

            tier.Price = expectedPrice;
            tier.LowerRange = 1;
            tier.UpperRange = null;

            price.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;
            price.CataloguePriceTiers = new HashSet<CataloguePriceTier> { tier };

            orderItemRecipient.Quantity = expectedQuantity;

            orderItem.OrderItemRecipients = new HashSet<OrderItemRecipient> { orderItemRecipient };
            orderItem.OrderItemPrice = new(price);

            var result = orderItem.OrderItemPrice.CalculateTotalCostPerTier(orderItem.GetTotalRecipientQuantity());

            var expected = new PriceCalculationModel(1, expectedQuantity, expectedPrice);

            result.Should()
                .NotBeEmpty()
                .And.HaveCount(1)
                .And.ContainEquivalentOf(expected);
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

            var result = orderItem.OrderItemPrice.CalculateTotalCost(orderItem.GetTotalRecipientQuantity());

            result.Should().Be(11263.18M);
        }

        [Theory]
        [CommonAutoData]
        public static void CalculateTotalCostPerTier_FlatPrice_Volume(
            CataloguePrice price,
            CataloguePriceTier tier,
            OrderItem orderItem,
            OrderItemRecipient orderItemRecipient)
        {
            var expectedQuantity = 3587;
            var expectedCost = 11263.18M;

            tier.Price = 3.14M;
            tier.LowerRange = 1;
            tier.UpperRange = null;

            price.CataloguePriceCalculationType = CataloguePriceCalculationType.Volume;
            price.CataloguePriceTiers = new HashSet<CataloguePriceTier> { tier };

            orderItemRecipient.Quantity = expectedQuantity;

            orderItem.OrderItemRecipients = new HashSet<OrderItemRecipient> { orderItemRecipient };
            orderItem.OrderItemPrice = new(price);

            var result = orderItem.OrderItemPrice.CalculateTotalCostPerTier(orderItem.GetTotalRecipientQuantity());

            var expected = new PriceCalculationModel(1, expectedQuantity, expectedCost);

            result.Should()
                .NotBeEmpty()
                .And.HaveCount(1)
                .And.ContainEquivalentOf(expected);
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

            var result = orderItem.OrderItemPrice.CalculateTotalCost(orderItem.GetTotalRecipientQuantity());

            result.Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(500, 0)]
        [CommonInlineAutoData(3587, 1)]
        [CommonInlineAutoData(7210, 2)]
        public static void CalculateTotalCostPerTier_Tiered_SingleFixed(
            int quantity,
            int index,
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

            var result = orderItem.OrderItemPrice.CalculateTotalCostPerTier(orderItem.GetTotalRecipientQuantity());

            var expected = TieredSingleFixedExpected(index, quantity);

            result.Should()
                .NotBeEmpty()
                .And.HaveCount(3)
                .And.ContainEquivalentOf(expected[0])
                .And.ContainEquivalentOf(expected[1])
                .And.ContainEquivalentOf(expected[2]);
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

            var result = orderItem.OrderItemPrice.CalculateTotalCost(orderItem.GetTotalRecipientQuantity());

            result.Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(500, 0)]
        [CommonInlineAutoData(1000, 1)]
        [CommonInlineAutoData(3587, 2)]
        [CommonInlineAutoData(4999, 3)]
        [CommonInlineAutoData(7210, 4)]
        [CommonInlineAutoData(10000, 5)]
        public static void CalculateTotalCostPerTier_Tiered_Cumulative(
            int quantity,
            int index,
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

            var result = orderItem.OrderItemPrice.CalculateTotalCostPerTier(orderItem.GetTotalRecipientQuantity());

            var expected = TieredCumulativeExpected(index);

            result.Should()
                .NotBeEmpty()
                .And.HaveCount(3)
                .And.ContainEquivalentOf(expected[0])
                .And.ContainEquivalentOf(expected[1])
                .And.ContainEquivalentOf(expected[2]);
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

            var result = orderItem.OrderItemPrice.CalculateTotalCost(orderItem.GetTotalRecipientQuantity());

            result.Should().Be(expected);
        }

        [Theory]
        [CommonInlineAutoData(500, 0)]
        [CommonInlineAutoData(3587, 1)]
        [CommonInlineAutoData(7210, 2)]
        public static void CalculateTotalCostPerTier_Tiered_Volume(
            int quantity,
            int index,
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

            var result = orderItem.OrderItemPrice.CalculateTotalCostPerTier(orderItem.GetTotalRecipientQuantity());

            var expected = TieredVolumeExpected(index);

            result.Should()
                .NotBeEmpty()
                .And.HaveCount(3)
                .And.ContainEquivalentOf(expected[0])
                .And.ContainEquivalentOf(expected[1])
                .And.ContainEquivalentOf(expected[2]);
        }

        private static List<PriceCalculationModel> TieredSingleFixedExpected(int index, int quantity)
        {
            var expected = new List<List<PriceCalculationModel>>()
            {
                new List<PriceCalculationModel>()
                {
                    new PriceCalculationModel(1, quantity, 3.14M),
                    new PriceCalculationModel(2, 0, 0M),
                    new PriceCalculationModel(3, 0, 0M),
                },
                new List<PriceCalculationModel>()
                {
                    new PriceCalculationModel(1, 0, 0),
                    new PriceCalculationModel(2, quantity, 2M),
                    new PriceCalculationModel(3, 0, 0M),
                },
                new List<PriceCalculationModel>()
                {
                    new PriceCalculationModel(1, 0, 0M),
                    new PriceCalculationModel(2, 0, 0M),
                    new PriceCalculationModel(3, quantity, 1.5M),
                },
            };

            return expected[index];
        }

        private static List<PriceCalculationModel> TieredCumulativeExpected(int index)
        {
            var expected = new List<List<PriceCalculationModel>>()
            {
                new List<PriceCalculationModel>()
                {
                    new PriceCalculationModel(1, 500, 1570M),
                    new PriceCalculationModel(2, 0, 0M),
                    new PriceCalculationModel(3, 0, 0M),
                },
                new List<PriceCalculationModel>()
                {
                    new PriceCalculationModel(1, 999, 3136.86M),
                    new PriceCalculationModel(2, 1, 2M),
                    new PriceCalculationModel(3, 0, 0M),
                },
                new List<PriceCalculationModel>()
                {
                    new PriceCalculationModel(1, 999, 3136.86M),
                    new PriceCalculationModel(2, 2588, 5176M),
                    new PriceCalculationModel(3, 0, 0M),
                },
                new List<PriceCalculationModel>()
                {
                    new PriceCalculationModel(1, 999, 3136.86M),
                    new PriceCalculationModel(2, 4000, 8000M),
                    new PriceCalculationModel(3, 0, 0M),
                },
                new List<PriceCalculationModel>()
                {
                    new PriceCalculationModel(1, 999, 3136.86M),
                    new PriceCalculationModel(2, 4000, 8000M),
                    new PriceCalculationModel(3, 2211, 3316.5M),
                },
                new List<PriceCalculationModel>()
                {
                    new PriceCalculationModel(1, 999, 3136.86M),
                    new PriceCalculationModel(2, 4000, 8000M),
                    new PriceCalculationModel(3, 5001, 7501.5M),
                },
            };

            return expected[index];
        }

        private static List<PriceCalculationModel> TieredVolumeExpected(int index)
        {
            var expected = new List<List<PriceCalculationModel>>()
            {
                new List<PriceCalculationModel>()
                {
                    new PriceCalculationModel(1, 500, 1570M),
                    new PriceCalculationModel(2, 0, 0M),
                    new PriceCalculationModel(3, 0, 0M),
                },
                new List<PriceCalculationModel>()
                {
                    new PriceCalculationModel(1, 0, 0M),
                    new PriceCalculationModel(2, 3587, 7174M),
                    new PriceCalculationModel(3, 0, 0M),
                },
                new List<PriceCalculationModel>()
                {
                    new PriceCalculationModel(1, 0, 0M),
                    new PriceCalculationModel(2, 0, 0M),
                    new PriceCalculationModel(3, 7210, 10815M),
                },
            };

            return expected[index];
        }
    }
}
