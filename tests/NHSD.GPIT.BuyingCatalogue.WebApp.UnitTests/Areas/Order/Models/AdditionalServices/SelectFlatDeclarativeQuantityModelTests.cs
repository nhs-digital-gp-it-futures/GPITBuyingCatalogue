using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AdditionalServices
{
    public static class SelectFlatDeclarativeQuantityModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            CallOffId callOffId,
            string solutionName,
            int? quantity)
        {
            var model = new SelectFlatDeclarativeQuantityModel(odsCode, callOffId, solutionName, quantity);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}/additional-services/select/additional-service/price/recipients/date");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Quantity of {solutionName} for {callOffId}");
            model.Quantity.Should().Be(quantity.ToString());
        }
    }
}
