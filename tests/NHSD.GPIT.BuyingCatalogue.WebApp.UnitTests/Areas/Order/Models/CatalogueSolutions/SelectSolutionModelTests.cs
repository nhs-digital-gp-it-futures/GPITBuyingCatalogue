using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.CatalogueSolutions
{
    public static class SelectSolutionModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            CallOffId callOffId,
            List<CatalogueItem> solutions,
            CatalogueItemId? selectedSolutionId)
        {
            var model = new SelectSolutionModel(odsCode, callOffId, solutions, selectedSolutionId);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions");
            model.Title.Should().Be($"Add Catalogue Solution for {callOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.Solutions.Should().BeEquivalentTo(solutions);
            model.SelectedSolutionId.Should().Be(selectedSolutionId);
        }
    }
}
