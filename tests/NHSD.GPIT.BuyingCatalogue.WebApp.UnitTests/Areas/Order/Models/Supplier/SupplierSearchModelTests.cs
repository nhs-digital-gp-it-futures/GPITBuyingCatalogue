using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Supplier
{
    public static class SupplierSearchModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            EntityFramework.Ordering.Models.Order order)
        {
            var model = new SupplierSearchModel(odsCode, order);

            model.Title.Should().Be("Find supplier information");
            model.CallOffId.Should().Be(order.CallOffId);
            model.InternalOrgId.Should().Be(odsCode);
        }
    }
}
