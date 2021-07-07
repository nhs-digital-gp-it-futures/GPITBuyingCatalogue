using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Supplier
{
    public static class NoSupplierFoundModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            CallOffId callOffId)
        {
            var model = new NoSupplierFoundModel(odsCode, callOffId);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}/supplier/search");
            model.BackLinkText.Should().Be("Go back to search");
        }
    }
}
