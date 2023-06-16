using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SharedModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.SharedModels;

public static class SolutionModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsProperties(
        Solution solution,
        Supplier supplier,
        AdditionalService requiredService,
        List<RequiredService> requiredServices,
        CompetitionSolution competitionSolution)
    {
        solution.CatalogueItem.Supplier = supplier;
        requiredServices.ForEach(x => x.Service = requiredService);

        competitionSolution.Solution = solution;
        competitionSolution.RequiredServices = requiredServices;

        var model = new SolutionModel(competitionSolution);

        model.SolutionId.Should().Be(solution.CatalogueItemId);
        model.SolutionName.Should().Be(solution.CatalogueItem.Name);
        model.SupplierName.Should().Be(supplier.Name);
        model.RequiredServices.Should().BeEquivalentTo(requiredServices.Select(x => x.Service.CatalogueItem.Name));
        model.Selected.Should().Be(competitionSolution.IsShortlisted);
    }

    [Theory]
    [CommonAutoData]
    public static void GetAdditionalServicesList_ReturnsCommaSeparatedList(
        List<string> requiredServices,
        SolutionModel model)
    {
        var expected = string.Join(", ", requiredServices);

        model.RequiredServices = requiredServices;

        model.GetAdditionalServicesList().Should().Be(expected);
    }
}
