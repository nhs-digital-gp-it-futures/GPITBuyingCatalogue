using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AdditionalServices
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

            model.Title.Should().Be($"Quantity of {solutionName} for {callOffId}");
            model.InternalOrgId.Should().Be(odsCode);
            model.CallOffId.Should().Be(callOffId);
            model.SolutionName.Should().Be(solutionName);
            model.Quantity.Should().Be(quantity.ToString());
            model.EstimationPeriod.Should().Be(estimationPeriod);
        }
    }
}
