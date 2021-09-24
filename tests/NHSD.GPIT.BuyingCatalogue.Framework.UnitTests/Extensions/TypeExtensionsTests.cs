using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class TypeExtensionsTests
    {
        [Fact]
        public static void ControllerName_TrimsController()
        {
            typeof(TestController).ControllerName().Should().Be("Test");
        }

        [Fact]
        public static void AreaName_GetsAreaAttribute()
        {
            typeof(TestController).AreaName().Should().Be("Solutions");
        }

        [Fact]
        public static void AreaName_NoAreaAttribute_ReturnsNull()
        {
            typeof(TestControllerNoAttribute).AreaName().Should().BeNull();
        }

        [Area("Solutions")]
        internal static class TestController
        {
        }

        internal static class TestControllerNoAttribute
        {
        }
    }
}
