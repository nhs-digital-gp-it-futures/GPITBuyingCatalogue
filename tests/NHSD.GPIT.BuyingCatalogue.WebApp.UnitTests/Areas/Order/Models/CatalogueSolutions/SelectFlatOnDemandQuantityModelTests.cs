using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.CatalogueSolutions
{
    public static class SelectFlatOnDemandQuantityModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            CallOffId callOffId,
            string solutionName,
            int? quantity,
            TimeUnit? estimationPeriod)
        {
            var model = new SelectFlatOnDemandQuantityModel(odsCode, callOffId, solutionName, quantity, estimationPeriod);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/select/solution/price/recipients/date");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Quantity of {solutionName} for {callOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.CallOffId.Should().Be(callOffId);
            model.SolutionName.Should().Be(solutionName);
            model.Quantity.Should().Be(quantity.ToString());
            model.EstimationPeriod.Should().Be(estimationPeriod);
        }
    }
}
