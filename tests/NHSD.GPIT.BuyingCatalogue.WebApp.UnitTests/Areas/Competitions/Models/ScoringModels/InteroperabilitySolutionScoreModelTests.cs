using System.Collections.Generic;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.ScoringModels;

public static class InteroperabilitySolutionScoreModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Solution solution,
        int? score,
        string justification)
    {
        var integrations = new List<Integration>
        {
            new() { IntegrationType = "IM1", Qualifier = "Bulk", },
            new() { IntegrationType = "GP Connect", Qualifier = "Access Record Structured" },
        };

        solution.Integrations = JsonConvert.SerializeObject(integrations);

        var model = new InteroperabilitySolutionScoreModel(solution, score, justification);

        model.Score.Should().Be(score);
        model.Im1Integrations.Should().ContainSingle();
        model.GpConnectIntegrations.Should().ContainSingle();
        model.Justification.Should().Be(justification);
    }
}
