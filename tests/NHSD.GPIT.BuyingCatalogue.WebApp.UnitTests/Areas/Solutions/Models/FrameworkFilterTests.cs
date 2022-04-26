using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class FrameworkFilterTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithDictionaryValues_HasValues_True(
            FrameworkFilter filter)
        {
            filter.DisplayText.Should().Be($"{filter.FrameworkFullName} ({filter.Count})");
        }
    }
}
