using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.ResultsModels;

public static class ConfirmResultsModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        List<CompetitionSolution> competitionSolutions,
        Competition competition)
    {
        competition.CompetitionSolutions = competitionSolutions;
        competition.NonPriceElements = new()
        {
            Features = new List<FeaturesCriteria> { new() },
            Implementation = new(),
            IntegrationTypes = new List<IntegrationType> { new() },
            ServiceLevel = new(),
        };

        var expectedNonPriceElements = NonPriceElementExtensions.GetAllNonPriceElements();

        var model = new ConfirmResultsModel(competition);

        model.CompetitionName.Should().Be(competition.Name);
        model.IncludesNonPriceElements.Should().Be(competition.IncludesNonPrice.GetValueOrDefault());
        model.CompetitionSolutions.Should().BeEquivalentTo(competitionSolutions);
        model.NonPriceElements.Should().BeEquivalentTo(expectedNonPriceElements);
    }
}
