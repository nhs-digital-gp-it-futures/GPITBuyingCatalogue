using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteAdditionalService;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.DeleteAdditionalService
{
    public static class DeleteAdditionalServiceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            string solutionName,
            string orderDescription)
        {
            var model = new DeleteAdditionalServiceModel(odsCode, callOffId, catalogueItemId, solutionName, orderDescription);

            model.Title.Should().Be($"Delete {solutionName} from {callOffId}?");
            model.OdsCode.Should().Be(odsCode);
            model.CallOffId.Should().Be(callOffId);
            model.AdditionalServiceId.Should().Be(catalogueItemId);
            model.SolutionName.Should().Be(solutionName);
            model.OrderDescription.Should().Be(orderDescription);
        }
    }
}
