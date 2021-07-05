using System;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Order
{
    public static class OrderDescriptionServiceTests
    {
        [Fact]
        public static void Constructor_NullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new OrderDescriptionService(null));
        }
    }
}
