using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteCatalogueSolution;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.DeleteCatalogueSolution
{
    public static class DeleteSolutionModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            string solutionName,
            string orderDescription)
        {
            var model = new DeleteSolutionModel(internalOrgId, callOffId, catalogueItemId, solutionName, orderDescription);
            model.Title.Should().Be($"Delete {solutionName} from {callOffId}?");
            model.InternalOrgId.Should().Be(internalOrgId);
            model.CallOffId.Should().Be(callOffId);
            model.SolutionId.Should().Be(catalogueItemId);
            model.SolutionName.Should().Be(solutionName);
            model.OrderDescription.Should().Be(orderDescription);
        }
    }
}
