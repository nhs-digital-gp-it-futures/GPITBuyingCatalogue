using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Competitions;

public static class CompetitionRecipientTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        int competitionId,
        string odsCode)
    {
        var model = new CompetitionRecipient(competitionId, odsCode);

        model.CompetitionId.Should().Be(competitionId);
        model.OdsCode.Should().Be(odsCode);
    }
}
