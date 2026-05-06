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
                .BeEquivalentTo(new List<OrderSublocationRecipient>());
        }

        [Theory]
        [MockAutoData]
        public static void ForCatalogueItem_Returns_Recipients_With_Link_To_CatalogueItemId(
            OrderSublocationRecipient recipient1,
            OrderItem orderItem1,
            OrderSublocationRecipient recipient2,
            OrderItem orderItem2)
        {
            recipient1.SetDeliveryDateForItem(orderItem1, DateTime.Now);
            recipient2.SetDeliveryDateForItem(orderItem2, DateTime.Now);

            OrderRecipientCollection.CollectionExtensions.ForCatalogueItem(
                    new List<OrderSublocationRecipient> { recipient1, recipient2 },
                    orderItem1.CatalogueItemId)
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
            exception!.GetType().Should().Be(typeof(ArgumentNullException));
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
            OrderRecipientCollection.CollectionExtensions.SomeNewQuantitiesEntered(null, orderItem)
                .Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void SomeButNotAllQuantitiesEntered_Returns_False_When_OrderItemPrice_Null(
            OrderSublocationRecipient[] recipients,
            OrderItem orderItem)
        {
            orderItem.OrderItemPrice = null;
            OrderRecipientCollection.CollectionExtensions.SomeNewQuantitiesEntered(recipients, orderItem)
                .Should().BeFalse();
        }
    }
}
