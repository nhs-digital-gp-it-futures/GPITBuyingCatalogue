using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
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

        competitionsService.Setup(x => x.GetCompetitionsDashboard(organisation.InternalIdentifier))
            .ReturnsAsync(competitions);

        var expectedModel = new CompetitionDashboardModel(
            organisation.InternalIdentifier,
            organisation.Name,
            competitions);

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
        result.ActionName.Should().Be(nameof(CompetitionsDashboardController.SelectFilter));
    }

    [Theory]
    [CommonAutoData]
    public static async Task SelectFilter_ReturnsViewWithModel(
        Organisation organisation,
        List<Filter> filters,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] Mock<IManageFiltersService> filterService,
        CompetitionsDashboardController controller)
    {
        organisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(It.IsAny<string>()))
            .ReturnsAsync(organisation);

        filterService.Setup(x => x.GetFilters(It.IsAny<int>()))
            .ReturnsAsync(filters);

        var expectedModel = new SelectFilterModel(organisation.Name, filters);

        var result = (await controller.SelectFilter(organisation.InternalIdentifier)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(x => x.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_SelectFilter_InvalidModel_ReturnsViewWithModel(
        Organisation organisation,
        List<Filter> filters,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] Mock<IManageFiltersService> filterService,
        CompetitionsDashboardController controller)
    {
        organisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(It.IsAny<string>()))
            .ReturnsAsync(organisation);

        filterService.Setup(x => x.GetFilters(It.IsAny<int>()))
            .ReturnsAsync(filters);

        var expectedModel = new SelectFilterModel(organisation.Name, filters);

        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.SelectFilter(organisation.InternalIdentifier, expectedModel)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(x => x.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_SelectFilter_Redirects(
        string internalOrgId,
        SelectFilterModel model,
        CompetitionsDashboardController controller)
    {
        var result = (await controller.SelectFilter(internalOrgId, model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ReviewFilter));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ReviewFilter_NullFilterDetails_Redirects(
        Organisation organisation,
        int filterId,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] Mock<IManageFiltersService> filtersService,
        CompetitionsDashboardController controller)
    {
        organisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(organisation);

        filtersService.Setup(x => x.GetFilterDetails(It.IsAny<int>(), filterId))
            .ReturnsAsync((FilterDetailsModel)null);

        var result = (await controller.ReviewFilter(organisation.InternalIdentifier, filterId))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.SelectFilter));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ReviewFilter_ValidFilter_ReturnsViewWithModel(
        Organisation organisation,
        int filterId,
        FilterDetailsModel filterDetailsModel,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] Mock<IManageFiltersService> filtersService,
        CompetitionsDashboardController controller)
    {
        organisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(organisation);

        filtersService.Setup(x => x.GetFilterDetails(organisation.Id, filterId))
            .ReturnsAsync(filterDetailsModel);

        var expectedModel = new ReviewFilterModel(filterDetailsModel) { Caption = filterDetailsModel.Name };

        var result = (await controller.ReviewFilter(organisation.InternalIdentifier, filterId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static void Post_ReviewFilter_Redirects(
        string internalOrgId,
        int filterId,
        ReviewFilterModel model,
        CompetitionsDashboardController controller)
    {
        var result = controller.ReviewFilter(internalOrgId, filterId, model).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.SaveCompetition));
    }

    [Theory]
    [CommonAutoData]
    public static async Task SaveCompetition_ReturnsViewWithModel(
        Organisation organisation,
        int filterId,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        CompetitionsDashboardController controller)
    {
        organisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(organisation);

        var expectedModel = new SaveCompetitionModel(organisation.InternalIdentifier, organisation.Name);

        var result = (await controller.SaveCompetition(organisation.InternalIdentifier, filterId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_SaveCompetition_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        int filterId,
        SaveCompetitionModel model,
        CompetitionsDashboardController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.SaveCompetition(internalOrgId, filterId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Post_SaveCompetition_ValidInput_SavesCompetition(
        Organisation organisation,
        Competition competition,
        int filterId,
        FilterIdsModel filterIdsModel,
        SaveCompetitionModel model,
        List<CatalogueItem> catalogueItems,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        [Frozen] Mock<ISolutionsFilterService> solutionsFilterService,
        [Frozen] Mock<IManageFiltersService> filtersService,
        CompetitionsDashboardController controller)
    {
        organisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(organisation);

        competitionsService.Setup(x => x.GetCompetitionWithServices(organisation.InternalIdentifier, It.IsAny<int>(), true))
            .ReturnsAsync(competition);

        filtersService.Setup(x => x.GetFilterIds(organisation.Id, filterId))
            .ReturnsAsync(filterIdsModel);

        solutionsFilterService.Setup(
                x => x.GetAllSolutionsFiltered(
                    It.IsAny<PageOptions>(),
                    It.IsAny<Dictionary<int, string[]>>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
            .ReturnsAsync((catalogueItems, null, null));

        var result = (await controller.SaveCompetition(organisation.InternalIdentifier, filterId, model))
            .As<RedirectToActionResult>();

        competitionsService.Verify(x => x.AddCompetition(organisation.Id, filterId, model.Name, model.Description));
        competitionsService.Verify(
            x => x.AddCompetitionSolutions(
                organisation.InternalIdentifier,
                competition.Id,
                It.IsAny<IEnumerable<CompetitionSolution>>()));

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionSelectSolutionsController.SelectSolutions));
        result.ControllerName.Should().Be(typeof(CompetitionSelectSolutionsController).ControllerName());
    }
}
