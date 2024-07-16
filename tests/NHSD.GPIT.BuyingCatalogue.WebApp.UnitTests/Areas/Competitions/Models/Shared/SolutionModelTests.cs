using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.Shared;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.Shared;

public static class SolutionModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsProperties(
        Solution solution,
        Supplier supplier,
        AdditionalService requiredService,
        List<SolutionService> requiredServices,
        CompetitionSolution competitionSolution)
    {
        solution.CatalogueItem.Supplier = supplier;
        requiredServices.ForEach(
            x =>
            {
                x.IsRequired = true;
                x.Service = requiredService.CatalogueItem;
            });

        competitionSolution.Solution = solution;
        competitionSolution.SolutionServices = requiredServices;

        var model = new SolutionModel(competitionSolution);

        model.SolutionId.Should().Be(solution.CatalogueItemId);
        model.SolutionName.Should().Be(solution.CatalogueItem.Name);
        model.SupplierName.Should().Be(supplier.Name);
        model.RequiredServices.Should().BeEquivalentTo(requiredServices.Select(x => x.Service.Name));
        model.Selected.Should().Be(competitionSolution.IsShortlisted);
    }

    [Theory]
    [MockAutoData]
    public static void GetAdditionalServicesList_ReturnsCommaSeparatedList(
        List<string> requiredServices,
        SolutionModel model)
    {
        var expected = string.Join(", ", requiredServices);

        model.RequiredServices = requiredServices;

        model.GetAdditionalServicesList().Should().Be(expected);
    }
}
