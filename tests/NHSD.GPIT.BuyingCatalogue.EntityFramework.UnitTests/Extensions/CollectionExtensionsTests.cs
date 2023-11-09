using System;
using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;
using OrderRecipientCollection = NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Extensions
{
    public static class CollectionExtensionsTests
    {
        [Theory]
        [CommonAutoData]
        public static void ForCatalogueItem_Returns_Empty_Collection_When_Null(CatalogueItemId catalogueItemId)
        {
            OrderRecipientCollection.CollectionExtensions.ForCatalogueItem(null, catalogueItemId)
                .Should()
                .BeEquivalentTo(new List<OrderRecipient>());
        }

        [Theory]
        [CommonAutoData]
        public static void ForCatalogueItem_Returns_Recipients_With_Link_To_CatalogueItemId(
            OrderRecipient recipient1,
            CatalogueItemId catalogueItemId1,
            OrderRecipient recipient2,
            CatalogueItemId catalogueItemId2)
        {
            recipient1.SetDeliveryDateForItem(catalogueItemId1, DateTime.Now);
            recipient2.SetDeliveryDateForItem(catalogueItemId2, DateTime.Now);

            OrderRecipientCollection.CollectionExtensions.ForCatalogueItem(new List<OrderRecipient> { recipient1, recipient2 }, catalogueItemId1)
                .Should()
                .BeEquivalentTo(new List<OrderRecipient> { recipient1 });
        }

        [Theory]
        [CommonAutoData]
        public static void AllDeliveryDatesEntered_Returns_False_When_Null(CatalogueItemId catalogueItemId)
        {
            OrderRecipientCollection.CollectionExtensions.AllDeliveryDatesEntered(null, catalogueItemId)
                .Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void NoDeliveryDatesEntered_Returns_False_When_Null(CatalogueItemId catalogueItemId)
        {
            OrderRecipientCollection.CollectionExtensions.NoDeliveryDatesEntered(null, catalogueItemId)
                .Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void AllQuantitiesEntered_Returns_False_When_Null(OrderItem orderItem)
        {
            OrderRecipientCollection.CollectionExtensions.AllQuantitiesEntered(null, orderItem)
                .Should().BeFalse();
        }

        [Fact]
        public static void Exist_Returns_False_When_Null()
        {
            OrderRecipientCollection.CollectionExtensions.Exists(null, string.Empty)
                .Should().BeFalse();
        }
    }
}
