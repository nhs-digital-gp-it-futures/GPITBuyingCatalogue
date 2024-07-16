using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class DescriptionListViewModelTests
    {
        [Fact]
        public static void Constructor_CreatedEmptyDictionary()
        {
            var model = new DescriptionListViewModel();
            model.Items.Should().NotBeNull();
            model.Items.Should().BeEmpty();
            model.HasValues().Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
        public static void WithDictionaryValues_HasValues_True(
            DescriptionListViewModel model)
        {
            model.HasValues().Should().BeTrue();
        }
    }
}
