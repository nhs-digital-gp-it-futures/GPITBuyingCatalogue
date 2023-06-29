using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.Competitions;
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
        CompetitionTaskListModel competitionTaskListModel)
    {
        var model = new CompetitionTaskListViewModel(organisation, competitionTaskListModel);

        model.OrganisationName.Should().Be(organisation.Name);
        model.InternalOrgId.Should().Be(organisation.InternalIdentifier);
        model.TaskListModel.Should().BeEquivalentTo(competitionTaskListModel);
    }
}
