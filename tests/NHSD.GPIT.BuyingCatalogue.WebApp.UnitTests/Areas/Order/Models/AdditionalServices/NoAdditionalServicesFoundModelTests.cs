using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AdditionalServices
{
    public static class NoAdditionalServicesFoundModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            CallOffId callOffId)
        {
            var model = new NoAdditionalServicesFoundModel(odsCode, callOffId);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be("No Additional Services found");
        }
    }
}
