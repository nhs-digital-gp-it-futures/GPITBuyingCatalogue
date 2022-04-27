using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class ListPriceModelTests
    {
        [Fact]
        public static void Class_Inherits_SolutionDisplayBaseModel()
        {
            typeof(ListPriceModel)
                .Should()
                .BeAssignableTo<SolutionDisplayBaseModel>();
        }

        [Theory]
        [CommonAutoData]
        public static void HasFlatListPrices_ValidPrices_ReturnsTrue(ListPriceModel model)
        {
            model.HasFlatListPrices().Should().BeTrue();
        }

        [Fact]
        public static void HasFlatListPrices_NoPrices_ReturnsFalse()
        {
            var model = new ListPriceModel { FlatListPrices = new List<PriceViewModel>(), };

            model.HasFlatListPrices().Should().BeFalse();
        }

        [Fact]
        public static void HasFlatListPrices_NullPrices_ReturnsFalse()
        {
            var model = new ListPriceModel { FlatListPrices = null, };

            model.HasFlatListPrices().Should().BeFalse();
        }
    }
}
