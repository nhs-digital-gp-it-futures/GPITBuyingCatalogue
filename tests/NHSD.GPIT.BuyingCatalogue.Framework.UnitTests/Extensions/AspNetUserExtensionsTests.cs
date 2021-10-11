using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class AspNetUserExtensionsTests
    {
        [Fact]
        public static void GetDisplayName_NullUser_ThrowsExceptions()
        {
            AspNetUser user = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws<ArgumentNullException>(() => user.GetDisplayName());
        }

        [Fact]
        public static void GetDisplayName_ReturnsExpectedValue()
        {
            var user = new AspNetUser { FirstName = "Bob", LastName = "Smith" };

            user.GetDisplayName().Should().Be("Bob Smith");
        }
    }
}
