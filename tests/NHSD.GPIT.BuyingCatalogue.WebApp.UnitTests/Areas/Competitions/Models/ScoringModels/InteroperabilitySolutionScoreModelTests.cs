using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.ScoringModels;

public static class InteroperabilitySolutionScoreModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Solution solution,
        int? score)
    {
        var integrations = new List<Integration>
        {
            new() { IntegrationType = "IM1", Qualifier = "Bulk", },
            new() { IntegrationType = "GP Connect", Qualifier = "Structured Record" },
        };

        solution.Integrations = JsonConvert.SerializeObject(integrations);

        var model = new InteroperabilitySolutionScoreModel(solution, score);

        model.Score.Should().Be(score);
        model.Im1Integrations.Should().ContainSingle();
        model.GpConnectIntegrations.Should().ContainSingle();
    }
}
