using System;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Order
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrderDescriptionServiceTests
    {
        [Test]
        public static void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new OrderDescriptionService(null, Mock.Of<IDbRepository<EntityFramework.Models.Ordering.Order, OrderingDbContext>>()));
        }

        [Test]
        public static void Constructor_NullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new OrderDescriptionService(Mock.Of<ILogWrapper<OrderDescriptionService>>(), null));
        }   
    }
}
