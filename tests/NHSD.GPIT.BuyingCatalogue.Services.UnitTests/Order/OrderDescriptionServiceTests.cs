using System;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Order
{
    public static class OrderDescriptionServiceTests
    {
        [Fact]
        public static void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new OrderDescriptionService(null, Mock.Of<IDbRepository<EntityFramework.Ordering.Models.Order, GPITBuyingCatalogueDbContext>>()));
        }

        [Fact]
        public static void Constructor_NullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new OrderDescriptionService(Mock.Of<ILogWrapper<OrderDescriptionService>>(), null));
        }
    }
}
