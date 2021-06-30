using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class AspNetUserExtensionsTests
    {
        [Fact]
        public static void AspNetUserExtention_FormatsDisplayName()
        {
            var user = new AspNetUser { FirstName = "Bob", LastName = "Smith" };

            user.GetDisplayName().Should().Be("Bob Smith");
        }
    }
}
