using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteAssociatedService;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.DeleteAssociatedService
{
    public static class DeleteAssociatedServiceModelTests
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
            var model = new DeleteAssociatedServiceModel(odsCode, callOffId, catalogueItemId, solutionName, orderDescription);

            model.Title.Should().Be($"Delete {solutionName} from {callOffId}?");
            model.InternalOrgId.Should().Be(odsCode);
            model.CallOffId.Should().Be(callOffId);
            model.CatalogueItemId.Should().Be(catalogueItemId);
            model.SolutionName.Should().Be(solutionName);
            model.OrderDescription.Should().Be(orderDescription);
        }
    }
}
