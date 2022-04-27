using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AdditionalServices
{
    public static class SelectAdditionalServiceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            CallOffId callOffId,
            List<CatalogueItem> solutions,
            CatalogueItemId? selectedSolutionId)
        {
            var model = new SelectAdditionalServiceModel(internalOrgId, callOffId, solutions, selectedSolutionId);

            model.Title.Should().Be($"Add an Additional Service for {callOffId}");
            model.InternalOrgId.Should().Be(internalOrgId);
            model.Solutions.Should().BeEquivalentTo(solutions);
            model.SelectedAdditionalServiceId.Should().Be(selectedSolutionId);
        }
    }
}
