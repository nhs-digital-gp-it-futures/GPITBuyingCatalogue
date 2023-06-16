using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.TaskListModels;

public static class CompetitionTaskListModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsProperties(
        Organisation organisation,
        Competition competition)
    {
        var model = new CompetitionTaskListModel(organisation, competition);

        model.OrganisationName.Should().Be(organisation.Name);
        model.InternalOrgId.Should().Be(organisation.InternalIdentifier);
        model.CompetitionId.Should().Be(competition.Id);
        model.Name.Should().Be(competition.Name);
        model.Description.Should().Be(competition.Description);
    }
}
