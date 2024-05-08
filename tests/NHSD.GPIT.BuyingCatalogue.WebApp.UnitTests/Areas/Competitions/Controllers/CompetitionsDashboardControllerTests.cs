using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
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
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Controllers;

public static class CompetitionsDashboardControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionsDashboardController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_ReturnsViewWithModel(
        Organisation organisation,
        PagedList<Competition> competitions,
        [Frozen] IOrganisationsService organisationsService,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionsDashboardController controller)
    {
        organisationsService.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier).Returns(organisation);

        competitionsService.GetPagedCompetitions(organisation.InternalIdentifier, Arg.Any<PageOptions>()).Returns(competitions);

        var expectedModel = new CompetitionDashboardModel(
            organisation.InternalIdentifier,
            organisation.Name,
            competitions.Items)
        {
            Options = competitions.Options,
        };

        var result = (await controller.Index(organisation.InternalIdentifier)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel);
    }

    [Theory]
    [MockAutoData]
    public static void BeforeYouStart_ReturnsViewWithModel(
        string internalOrgId,
        CompetitionsDashboardController controller)
    {
        var result = controller.BeforeYouStart(internalOrgId).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeOfType<NavBaseModel>();
    }

    [Theory]
    [MockAutoData]
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
    [MockAutoData]
    public static async Task SelectFilter_ReturnsViewWithModel(
        Organisation organisation,
        List<Filter> filters,
        [Frozen] IOrganisationsService organisationsService,
        [Frozen] IManageFiltersService filterService,
        CompetitionsDashboardController controller)
    {
        organisationsService.GetOrganisationByInternalIdentifier(Arg.Any<string>()).Returns(organisation);

        filterService.GetFilters(Arg.Any<int>()).Returns(filters);

        var expectedModel = new SelectFilterModel(organisation.Name, filters);

        var result = (await controller.SelectFilter(organisation.InternalIdentifier)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(x => x.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_SelectFilter_InvalidModel_ReturnsViewWithModel(
        Organisation organisation,
        List<Filter> filters,
        [Frozen] IOrganisationsService organisationsService,
        [Frozen] IManageFiltersService filterService,
        CompetitionsDashboardController controller)
    {
        organisationsService.GetOrganisationByInternalIdentifier(Arg.Any<string>()).Returns(organisation);

        filterService.GetFilters(Arg.Any<int>()).Returns(filters);

        var expectedModel = new SelectFilterModel(organisation.Name, filters);

        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.SelectFilter(organisation.InternalIdentifier, expectedModel)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(x => x.BackLink));
    }

    [Theory]
    [MockAutoData]
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
    [MockAutoData]
    public static async Task ReviewFilter_NullFilterDetails_Redirects(
        Organisation organisation,
        int filterId,
        [Frozen] IOrganisationsService organisationsService,
        [Frozen] IManageFiltersService filtersService,
        CompetitionsDashboardController controller)
    {
        organisationsService.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier).Returns(organisation);

        filtersService.GetFilterDetails(Arg.Any<int>(), filterId).Returns((FilterDetailsModel)null);

        var result = (await controller.ReviewFilter(organisation.InternalIdentifier, filterId))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.SelectFilter));
    }

    [Theory]
    [MockAutoData]
    public static async Task ReviewFilter_ValidFilter_ReturnsViewWithModel(
        Organisation organisation,
        int filterId,
        FilterDetailsModel filterDetailsModel,
        [Frozen] IOrganisationsService organisationsService,
        [Frozen] IManageFiltersService filtersService,
        CompetitionsDashboardController controller)
    {
        organisationsService.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier).Returns(organisation);

        filtersService.GetFilterDetails(organisation.Id, filterId).Returns(filterDetailsModel);

        var expectedModel = new ReviewFilterModel(filterDetailsModel)
        {
            Caption = organisation.Name,
            InternalOrgId = organisation.InternalIdentifier,
            OrganisationName = organisation.Name,
            InExpander = true,
            InCompetition = true,
        };

        var result = (await controller.ReviewFilter(organisation.InternalIdentifier, filterId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
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
    [MockAutoData]
    public static async Task SaveCompetition_ReturnsViewWithModel(
        Organisation organisation,
        int filterId,
        string frameworkId,
        IList<CatalogueItem> catalogueItems,
        [Frozen] IOrganisationsService organisationsService,
        [Frozen] IManageFiltersService filterService,
        [Frozen] ISolutionsFilterService solutionsFilterService,
        CompetitionsDashboardController controller)
    {
        organisationsService.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier).Returns(organisation);

        var filterIds = new FilterIdsModel() { FrameworkId = frameworkId };
        filterService.GetFilterIds(organisation.Id, filterId)
            .Returns(Task.FromResult(filterIds));

        catalogueItems.ForEach(x => x.Solution = new Solution() { FrameworkSolutions = new List<FrameworkSolution> { new FrameworkSolution() { FrameworkId = frameworkId } } });

        solutionsFilterService.GetAllSolutionsFilteredFromFilterIds(filterIds)
            .Returns((catalogueItems, null, null));

        var expectedModel = new SaveCompetitionModel(organisation.InternalIdentifier, organisation.Name, frameworkId);

        var result = (await controller.SaveCompetition(organisation.InternalIdentifier, filterId, frameworkId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task SaveCompetition_NullFrameworkId_Redirects(
        Organisation organisation,
        int filterId,
        [Frozen] IOrganisationsService organisationsService,
        [Frozen] IManageFiltersService filterService,
        CompetitionsDashboardController controller)
    {
        organisationsService.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier)
            .Returns(Task.FromResult(organisation));

        filterService.GetFilterIds(organisation.Id, filterId)
            .Returns(Task.FromResult<FilterIdsModel>(null));

        var result = (await controller.SaveCompetition(organisation.InternalIdentifier, filterId)).As<RedirectResult>();

        result.Should().NotBeNull();
        result.Should().BeOfType<RedirectResult>();
    }

    [Theory]
    [MockAutoData]
    public static async Task SaveCompetition_NoAvailableSolutions_Redirects(
        Organisation organisation,
        int filterId,
        string frameworkId,
        IList<CatalogueItem> catalogueItems,
        [Frozen] IOrganisationsService organisationsService,
        [Frozen] IManageFiltersService filterService,
        [Frozen] ISolutionsFilterService solutionsFilterService,
        CompetitionsDashboardController controller)
    {
        organisationsService.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier)
            .Returns(Task.FromResult(organisation));

        var filterIds = new FilterIdsModel() { FrameworkId = frameworkId };
        filterService.GetFilterIds(organisation.Id, filterId)
            .Returns(Task.FromResult(filterIds));

        catalogueItems.ForEach(x => x.Solution = new Solution() { FrameworkSolutions = new List<FrameworkSolution> { new FrameworkSolution() { FrameworkId = "DifferentId" } } });

        solutionsFilterService.GetAllSolutionsFilteredFromFilterIds(filterIds)
            .Returns((catalogueItems, null, null));

        var expectedModel = new SaveCompetitionModel(organisation.InternalIdentifier, organisation.Name, frameworkId);

        var result = (await controller.SaveCompetition(organisation.InternalIdentifier, filterId, frameworkId)).As<RedirectResult>();

        result.Should().NotBeNull();
        result.Should().BeOfType<RedirectResult>();
    }

    [Theory]
    [MockAutoData]
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
    [MockAutoData]
    public static async Task Post_SaveCompetition_ValidInput_SavesCompetition(
        Organisation organisation,
        Competition competition,
        int filterId,
        FilterIdsModel filterIdsModel,
        SaveCompetitionModel model,
        List<CatalogueItem> catalogueItems,
        [Frozen] IOrganisationsService organisationsService,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] ISolutionsFilterService solutionsFilterService,
        [Frozen] IManageFiltersService filtersService,
        CompetitionsDashboardController controller)
    {
        organisationsService.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier).Returns(organisation);

        competitionsService.GetCompetitionWithServices(organisation.InternalIdentifier, Arg.Any<int>(), true).Returns(competition);

        filtersService.GetFilterIds(organisation.Id, filterId).Returns(filterIdsModel);

        solutionsFilterService.GetAllSolutionsFiltered(Arg.Any<PageOptions>(), Arg.Any<Dictionary<int, string[]>>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns((catalogueItems, null, null));

        var result = (await controller.SaveCompetition(organisation.InternalIdentifier, filterId, model))
            .As<RedirectToActionResult>();

        await competitionsService.Received().AddCompetition(organisation.Id, filterId, model.Name, model.Description);
        await competitionsService.Received().AddCompetitionSolutions(
                organisation.InternalIdentifier,
                competition.Id,
                Arg.Any<IEnumerable<CompetitionSolution>>());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionSelectSolutionsController.SelectSolutions));
        result.ControllerName.Should().Be(typeof(CompetitionSelectSolutionsController).ControllerName());
    }
}
