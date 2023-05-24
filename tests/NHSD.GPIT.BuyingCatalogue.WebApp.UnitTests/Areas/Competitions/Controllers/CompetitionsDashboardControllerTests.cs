using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Controllers;

public static class CompetitionsDashboardControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionsDashboardController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Index_ReturnsViewWithModel(
        Organisation organisation,
        List<Competition> competitions,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionsDashboardController controller)
    {
        organisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(organisation);

        competitionsService.Setup(x => x.GetCompetitions(organisation.Id))
            .ReturnsAsync(competitions);

        var expectedModel = new CompetitionDashboardModel(organisation.InternalIdentifier, organisation.Name, competitions);

        var result = (await controller.Index(organisation.InternalIdentifier)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [CommonAutoData]
    public static void BeforeYouStart_ReturnsViewWithModel(
        string internalOrgId,
        CompetitionsDashboardController controller)
    {
        var result = controller.BeforeYouStart(internalOrgId).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeOfType<NavBaseModel>();
    }

    [Theory]
    [CommonAutoData]
    public static void Post_BeforeYouStart_Redirects(
        string internalOrgId,
        NavBaseModel model,
        CompetitionsDashboardController controller)
    {
        var result = controller.BeforeYouStart(internalOrgId, model).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionsDashboardController.Index));
    }
}
