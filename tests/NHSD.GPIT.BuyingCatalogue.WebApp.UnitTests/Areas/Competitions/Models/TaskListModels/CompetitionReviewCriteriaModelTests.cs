using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.TaskListModels;

public static class CompetitionReviewCriteriaModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Organisation organisation,
        Competition competition,
        NonPriceElements nonPriceElements,
        Weightings weightings)
    {
        competition.Organisation = organisation;
        competition.NonPriceElements = nonPriceElements;
        competition.Weightings = weightings;

        var model = new CompetitionReviewCriteriaModel(competition);

        model.InternalOrgId.Should().Be(organisation.InternalIdentifier);
        model.CompetitionId.Should().Be(competition.Id);
        model.CompetitionName.Should().Be(competition.Name);
        model.NonPriceElements.Should().Be(nonPriceElements);
        model.CompetitionWeights.Should().Be(weightings);
    }
}
