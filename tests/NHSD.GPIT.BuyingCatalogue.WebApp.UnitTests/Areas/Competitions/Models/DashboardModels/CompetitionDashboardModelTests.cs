using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.DashboardModels;

public static class CompetitionDashboardModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsProperties(
        string internalOrgId,
        string organisationName,
        List<Competition> competitions)
    {
        var model = new CompetitionDashboardModel(internalOrgId, organisationName, competitions);

        model.InternalOrgId.Should().Be(internalOrgId);
        model.OrganisationName.Should().Be(organisationName);
        model.Competitions.Should()
            .BeEquivalentTo(
                competitions.Select(
                    x => new CompetitionDashboardItem(x)));
    }
}
