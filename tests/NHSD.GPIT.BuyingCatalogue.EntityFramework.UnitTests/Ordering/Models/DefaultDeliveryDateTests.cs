using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Ordering.Models
{
    public static class DefaultDeliveryDateTests
    {
        [Theory]
        [CommonAutoData]
        public static void Equals_DefaultDeliveryDate_ReturnsExpectedResult(
           DefaultDeliveryDate defaultDeliveryDate)
        {
            var otherDefaultDeliveryDate = new DefaultDeliveryDate
            {
                CatalogueItemId = defaultDeliveryDate.CatalogueItemId,
                OrderId = defaultDeliveryDate.OrderId,
            };

            defaultDeliveryDate.Should().Be(otherDefaultDeliveryDate);
        }

        [Theory]
        [CommonAutoData]
        public static void Equals_DefaultDeliveryDate_DifferentCatalogueItemId_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate)
        {
            var otherDefaultDeliveryDate = new DefaultDeliveryDate
            {
                CatalogueItemId = default,
                OrderId = defaultDeliveryDate.OrderId,
            };

            defaultDeliveryDate.Should().NotBe(otherDefaultDeliveryDate);
        }

        [Theory]
        [CommonAutoData]
        public static void Equals_DefaultDeliveryDate_DifferentOrderId_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate)
        {
            var otherDefaultDeliveryDate = new DefaultDeliveryDate
            {
                CatalogueItemId = defaultDeliveryDate.CatalogueItemId,
                OrderId = defaultDeliveryDate.OrderId + 1,
            };

            defaultDeliveryDate.Should().NotBe(otherDefaultDeliveryDate);
        }

        [Theory]
        [CommonAutoData]
        public static void Equals_DefaultDeliveryDate_DifferentType_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate,
            string somethingElse)
        {
            defaultDeliveryDate.Should().NotBe(somethingElse);
        }

        [Theory]
        [CommonAutoData]
        public static void Equals_DefaultDeliveryDate_NullObject_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate)
        {
            defaultDeliveryDate.Should().NotBe(null);
        }

        [Theory]
        [CommonAutoData]
        public static void Equals_DefaultDeliveryDate_SameObject_ReturnsExpectedResult(
            DefaultDeliveryDate defaultDeliveryDate)
        {
            defaultDeliveryDate.Should().Be(defaultDeliveryDate);
        }

        [Theory]
        [CommonAutoData]
        public static void GetHashCode_Equal_ReturnsExpectedValue(int orderId, CatalogueItemId itemId)
        {
            var date1 = new DefaultDeliveryDate { OrderId = orderId, CatalogueItemId = itemId };
            var date2 = new DefaultDeliveryDate { OrderId = orderId, CatalogueItemId = itemId };

            var hash1 = date1.GetHashCode();
            var hash2 = date2.GetHashCode();

            hash1.Should().Be(hash2);
        }

        [Theory]
        [CommonAutoData]
        public static void GetHashCode_DifferentOrderId_ReturnsExpectedValue(
            int orderId1,
            int orderId2,
            CatalogueItemId itemId)
        {
            var date1 = new DefaultDeliveryDate { OrderId = orderId1, CatalogueItemId = itemId };
            var date2 = new DefaultDeliveryDate { OrderId = orderId2, CatalogueItemId = itemId };

            var hash1 = date1.GetHashCode();
            var hash2 = date2.GetHashCode();

            hash1.Should().NotBe(hash2);
        }

        [Theory]
        [CommonAutoData]
        public static void GetHashCode_DifferentCatalogueItemId_ReturnsExpectedValue(
            int orderId,
            CatalogueItemId itemId1,
            CatalogueItemId itemId2)
        {
            var date1 = new DefaultDeliveryDate { OrderId = orderId, CatalogueItemId = itemId1 };
            var date2 = new DefaultDeliveryDate { OrderId = orderId, CatalogueItemId = itemId2 };

            var hash1 = date1.GetHashCode();
            var hash2 = date2.GetHashCode();

            hash1.Should().NotBe(hash2);
        }
    }
}
