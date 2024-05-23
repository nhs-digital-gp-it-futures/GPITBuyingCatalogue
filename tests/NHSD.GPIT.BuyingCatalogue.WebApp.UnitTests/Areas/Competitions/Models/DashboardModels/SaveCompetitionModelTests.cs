using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.DashboardModels;

public static class SaveCompetitionModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsProperties(
        string internalOrgId,
        string organisationName,
        string frameworkId)
    {
        var model = new SaveCompetitionModel(internalOrgId, organisationName, frameworkId);

        model.InternalOrgId.Should().Be(internalOrgId);
        model.OrganisationName.Should().Be(organisationName);
        model.FrameworkId.Should().Be(frameworkId);
    }
}
