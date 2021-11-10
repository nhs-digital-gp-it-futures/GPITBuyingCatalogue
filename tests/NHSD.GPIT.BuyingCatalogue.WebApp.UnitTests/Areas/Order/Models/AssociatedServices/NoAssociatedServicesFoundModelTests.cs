using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AssociatedServices
{
    public static class NoAssociatedServicesFoundModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            CallOffId callOffId)
        {
            var model = new NoAssociatedServicesFoundModel(odsCode, callOffId);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}");
            model.Title.Should().Be("No Associated Services found");
        }
    }
}
