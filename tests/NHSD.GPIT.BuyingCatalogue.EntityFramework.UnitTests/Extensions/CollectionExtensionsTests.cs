using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;
using OrderRecipientCollection = NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Extensions
{
    public static class CollectionExtensionsTests
    {
        [Theory]
        [MockAutoData]
        public static void ForCatalogueItem_Returns_Empty_Collection_When_Null(CatalogueItemId catalogueItemId)
        {
            OrderRecipientCollection.CollectionExtensions.ForCatalogueItem(null, catalogueItemId)
                .Should()
                .BeEquivalentTo(new List<OrderRecipient>());
        }

        [Theory]
        [MockAutoData]
        public static void ForCatalogueItem_Returns_Recipients_With_Link_To_CatalogueItemId(
            OrderSublocationRecipient recipient1,
            CatalogueItemId catalogueItemId1,
            OrderSublocationRecipient recipient2,
            CatalogueItemId catalogueItemId2)
        {
            recipient1.SetDeliveryDateForItem(catalogueItemId1, DateTime.Now);
            recipient2.SetDeliveryDateForItem(catalogueItemId2, DateTime.Now);

            OrderRecipientCollection.CollectionExtensions.ForCatalogueItem(
                    new List<OrderSublocationRecipient> { recipient1, recipient2 },
                    catalogueItemId1)
                .Should()
                .BeEquivalentTo(new List<OrderSublocationRecipient> { recipient1 });
        }

        [Theory]
        [MockAutoData]
        public static void AllDeliveryDatesEntered_Rejects_Null(CatalogueItemId catalogueItemId)
        {
            Exception exception = Record.Exception(() =>
            {
                OrderRecipientCollection.CollectionExtensions.AllDeliveryDatesEntered(null, catalogueItemId);
            });

            exception.Should().NotBeNull();
            exception!.GetType().Should().Be(typeof(ArgumentException));
        }

        [Theory]
        [MockAutoData]
        public static void AllDeliveryDatesEntered_Returns_False_When_Recipients_No_Linked_To_CatalogueItemId(
            OrderSublocationRecipient[] recipients,
            CatalogueItemId catalogueItemId)
        {
            OrderRecipientCollection.CollectionExtensions.AllDeliveryDatesEntered(recipients, catalogueItemId)
                .Should()
                .BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void NoDeliveryDatesEntered_Rejects_Null(CatalogueItemId catalogueItemId)
        {
            Exception exception = Record.Exception(() =>
            {
                OrderRecipientCollection.CollectionExtensions.NoDeliveryDatesEntered(null, catalogueItemId);
            });

            exception.Should().NotBeNull();
            exception!.GetType().Should().Be(typeof(ArgumentException));
        }

        [Fact]
        public static void AllQuantitiesEntered_Returns_False_When_Recipients_And_OrderItem_Null()
        {
            OrderRecipientCollection.CollectionExtensions.AllQuantitiesEntered(null, null)
                .Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void AllQuantitiesEntered_Returns_False_When_OrderItem_Null(
            OrderSublocationRecipient[] recipients)
        {
            OrderRecipientCollection.CollectionExtensions.AllQuantitiesEntered(recipients, null)
                .Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void AllQuantitiesEntered_Returns_False_When_Recipient_Null(
            OrderSublocationRecipient[] recipients,
            OrderItem orderItem)
        {
            OrderRecipientCollection.CollectionExtensions.AllQuantitiesEntered(recipients, orderItem)
                .Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void AllQuantitiesEntered_Returns_False_When_OrderItemPrice_Null(
            OrderSublocationRecipient[] recipients,
            OrderItem orderItem)
        {
            orderItem.OrderItemPrice = null;
            OrderRecipientCollection.CollectionExtensions.AllQuantitiesEntered(recipients, orderItem)
                .Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void SomeButNotAllQuantitiesEntered_Returns_False_When_Recipient_Null(OrderItem orderItem)
        {
            OrderRecipientCollection.CollectionExtensions.SomeButNotAllNewQuantitiesEntered(null, orderItem)
                .Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void SomeButNotAllQuantitiesEntered_Returns_False_When_OrderItemPrice_Null(
            OrderSublocationRecipient[] recipients,
            OrderItem orderItem)
        {
            orderItem.OrderItemPrice = null;
            OrderRecipientCollection.CollectionExtensions.SomeButNotAllNewQuantitiesEntered(recipients, orderItem)
                .Should().BeFalse();
        }
    }
}
