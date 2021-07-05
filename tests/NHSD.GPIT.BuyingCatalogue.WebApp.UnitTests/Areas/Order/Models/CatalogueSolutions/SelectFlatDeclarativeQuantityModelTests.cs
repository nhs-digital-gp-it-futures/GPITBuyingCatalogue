using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.CatalogueSolutions
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

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price/recipients/date");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Quantity of {solutionName} for {callOffId}");
            model.Quantity.Should().Be(quantity.ToString());
        }
    }
}
