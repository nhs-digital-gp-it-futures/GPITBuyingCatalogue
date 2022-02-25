using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AssociatedServices
{
    public static class SelectAssociatedServiceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            CallOffId callOffId,
            List<CatalogueItem> solutions,
            CatalogueItemId? selectedSolutionId)
        {
            var model = new SelectAssociatedServiceModel(internalOrgId, callOffId, solutions, selectedSolutionId);

            model.Title.Should().Be($"Add an Associated Service for {callOffId}");
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Solutions.Should().BeEquivalentTo(solutions);
            model.SelectedSolutionId.Should().Be(selectedSolutionId);
        }
    }
}
