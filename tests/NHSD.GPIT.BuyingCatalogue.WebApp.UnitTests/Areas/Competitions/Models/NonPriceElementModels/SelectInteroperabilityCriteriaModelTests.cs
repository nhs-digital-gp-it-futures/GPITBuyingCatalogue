using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.NonPriceElementModels;

public static class SelectInteroperabilityCriteriaModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Competition competition)
    {
        competition.NonPriceElements = new() { Interoperability = Enumerable.Empty<InteroperabilityCriteria>().ToList() };

        var model = new SelectInteroperabilityCriteriaModel(competition);

        model.CompetitionName.Should().Be(competition.Name);
        model.Im1Integrations.Should()
            .BeEquivalentTo(Interoperability.Im1Integrations.Select(x => new SelectOption<string>(x.Value, x.Key)));
        model.GpConnectIntegrations.Should()
            .BeEquivalentTo(Interoperability.GpConnectIntegrations.Select(x => new SelectOption<string>(x.Value, x.Key)));
    }
}
