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
        string internalOrgId,
        string organisationName)
    {
        var model = new SaveCompetitionModel(internalOrgId, organisationName);

        model.InternalOrgId.Should().Be(internalOrgId);
        model.OrganisationName.Should().Be(organisationName);
    }
}
