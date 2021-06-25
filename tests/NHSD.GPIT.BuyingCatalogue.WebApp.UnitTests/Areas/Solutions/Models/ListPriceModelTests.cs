using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ListPriceModelTests
    {
        [Test]
        public static void Class_Inherits_SolutionDisplayBaseModel()
        {
            typeof(ListPriceModel)
                .Should()
                .BeAssignableTo<SolutionDisplayBaseModel>();
        }

        [Test]
        [CommonAutoData]
        public static void HasFlatListPrices_ValidPrices_ReturnsTrue(ListPriceModel model)
        {
            model.HasFlatListPrices().Should().BeTrue();
        }

        [Test]
        public static void HasFlatListPrices_NoPrices_ReturnsFalse()
        {
            var model = new ListPriceModel { FlatListPrices = new List<PriceViewModel>(), };

            model.HasFlatListPrices().Should().BeFalse();
        }

        [Test]
        public static void HasFlatListPrices_NullPrices_ReturnsFalse()
        {
            var model = new ListPriceModel { FlatListPrices = null, };

            model.HasFlatListPrices().Should().BeFalse();
        }
    }
}
