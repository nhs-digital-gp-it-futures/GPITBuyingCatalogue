using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.NonPriceElementModels;

public static class AddServiceLevelCriteriaModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        ServiceLevelCriteria serviceLevelCriteria,
        Competition competition)
    {
        competition.NonPriceElements = new() { ServiceLevel = serviceLevelCriteria };

        var model = new AddServiceLevelCriteriaModel(competition);

        model.CompetitionName.Should().Be(competition.Name);
        model.TimeFrom.Should().Be(serviceLevelCriteria.TimeFrom);
        model.TimeUntil.Should().Be(serviceLevelCriteria.TimeUntil);
        model.ApplicableDays.Should().Be(serviceLevelCriteria.ApplicableDays);
    }
}
