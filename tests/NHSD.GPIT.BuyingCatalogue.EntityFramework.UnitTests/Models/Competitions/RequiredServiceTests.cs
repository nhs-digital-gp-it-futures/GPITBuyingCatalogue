using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Competitions;

public static class RequiredServiceTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsProperties(
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId)
    {
        var model = new RequiredService(competitionId, solutionId, serviceId);

        model.CompetitionId.Should().Be(competitionId);
        model.SolutionId.Should().Be(solutionId);
        model.ServiceId.Should().Be(serviceId);
    }
}
