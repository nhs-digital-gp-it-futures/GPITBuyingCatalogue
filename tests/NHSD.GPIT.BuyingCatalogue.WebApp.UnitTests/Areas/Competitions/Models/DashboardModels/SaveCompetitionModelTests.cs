using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.DashboardModels;

public static class SaveCompetitionModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsProperties(
        int organisationId,
        string organisationName)
    {
        var model = new SaveCompetitionModel(organisationId, organisationName);

        model.OrganisationId.Should().Be(organisationId);
        model.OrganisationName.Should().Be(organisationName);
    }
}
