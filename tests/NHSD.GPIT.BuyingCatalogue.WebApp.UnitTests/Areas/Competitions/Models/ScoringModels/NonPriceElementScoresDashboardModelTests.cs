using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.ScoringModels;

public static class NonPriceElementScoresDashboardModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Organisation organisation,
        Competition competition)
    {
        competition.Organisation = organisation;
        competition.NonPriceElements = new() { Implementation = new(), ServiceLevel = new(), };

        var expectedNonPriceElements = new Dictionary<NonPriceElement, TaskProgress>
        {
            { NonPriceElement.Implementation, TaskProgress.NotStarted },
            { NonPriceElement.ServiceLevel, TaskProgress.NotStarted },
        };

        var model = new NonPriceElementScoresDashboardModel(competition);

        model.CompetitionName.Should().Be(competition.Name);
        model.NonPriceElements.Should().BeEquivalentTo(expectedNonPriceElements);
    }
}
