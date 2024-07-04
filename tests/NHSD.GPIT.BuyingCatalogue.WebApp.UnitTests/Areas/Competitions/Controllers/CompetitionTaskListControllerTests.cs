using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Controllers;

public static class CompetitionTaskListControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionTaskListController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_ReturnsViewWithModel(
        Organisation organisation,
        CompetitionTaskListModel competitionTaskListModel,
        [Frozen] IOrganisationsService organisationsService,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionTaskListController controller)
    {
        organisationsService.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier)
            .Returns(organisation);

        competitionsService.GetCompetitionTaskList(organisation.InternalIdentifier, competitionTaskListModel.Id)
            .Returns(competitionTaskListModel);

        var expectedModel = new CompetitionTaskListViewModel(organisation, competitionTaskListModel);

        var result = (await controller.Index(organisation.InternalIdentifier, competitionTaskListModel.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, x => x.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task ShortlistedSolutions_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionTaskListController controller)
    {
        competitionsService.GetCompetitionWithSolutions(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);

        var expectedModel = new CompetitionShortlistedSolutionsModel(competition);

        var result = (await controller.ShortlistedSolutions(organisation.InternalIdentifier, competition.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, x => x.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task ContractLength_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionTaskListController controller)
    {
        competitionsService.GetCompetitionWithFramework(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);

        var expectedModel = new CompetitionContractModel(competition);

        var result = (await controller.ContractLength(organisation.InternalIdentifier, competition.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task ContractLength_InvalidModelState_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        CompetitionContractModel model,
        CompetitionTaskListController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.ContractLength(internalOrgId, competitionId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task ContractLength_Valid_Redirects(
        Organisation organisation,
        int competitionId,
        CompetitionContractModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionTaskListController controller)
    {
        var result = (await controller.ContractLength(organisation.InternalIdentifier, competitionId, model))
            .As<RedirectToActionResult>();

        await competitionsService
            .Received()
            .SetContractLength(organisation.InternalIdentifier, competitionId, model.ContractLength.GetValueOrDefault());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task AwardCriteria_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionTaskListController controller)
    {
        competitionsService.GetCompetition(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);

        var expectedModel = new CompetitionAwardCriteriaModel(competition);

        var result = (await controller.AwardCriteria(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task AwardCriteria_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        CompetitionAwardCriteriaModel model,
        CompetitionTaskListController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.AwardCriteria(internalOrgId, competitionId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task AwardCriteria_ValidModel_RedirectsToTaskList(
        Organisation organisation,
        Competition competition,
        CompetitionAwardCriteriaModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionTaskListController controller)
    {
        competition.IncludesNonPrice = false;
        competition.Organisation = organisation;

        competitionsService.GetCompetition(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);

        var result = (await controller.AwardCriteria(organisation.InternalIdentifier, competition.Id, model))
            .As<RedirectToActionResult>();

        await competitionsService
            .Received()
            .SetCompetitionCriteria(organisation.InternalIdentifier, competition.Id, model.IncludesNonPrice.GetValueOrDefault());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task AwardCriteria_NonPriceToPriceOnly_RedirectsToConfirmation(
        Organisation organisation,
        Competition competition,
        CompetitionAwardCriteriaModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionTaskListController controller)
    {
        competition.IncludesNonPrice = true;
        competition.Organisation = organisation;
        model.IncludesNonPrice = false;

        competitionsService.GetCompetition(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);

        var result = (await controller.AwardCriteria(organisation.InternalIdentifier, competition.Id, model))
            .As<RedirectToActionResult>();

        await competitionsService
            .Received(0)
            .SetCompetitionCriteria(organisation.InternalIdentifier, competition.Id, model.IncludesNonPrice.GetValueOrDefault());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ConfirmAwardCriteria));
    }

    [Theory]
    [MockAutoData]
    public static async Task Weightings_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionTaskListController controller)
    {
        competitionsService.GetCompetitionWithWeightings(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);

        var expectedModel = new CompetitionWeightingsModel(competition);

        var result = (await controller.Weightings(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Weightings_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        CompetitionWeightingsModel model,
        CompetitionTaskListController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.Weightings(internalOrgId, competitionId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task Weightings_ValidModel_Redirects(
        Organisation organisation,
        int competitionId,
        CompetitionWeightingsModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionTaskListController controller)
    {
        var result = (await controller.Weightings(organisation.InternalIdentifier, competitionId, model))
            .As<RedirectToActionResult>();

        await competitionsService
            .Received()
            .SetCompetitionWeightings(
                organisation.InternalIdentifier,
                competitionId,
                model.Price.GetValueOrDefault(),
                model.NonPrice.GetValueOrDefault());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task ReviewCriteria_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        List<Integration> integrations,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IIntegrationsService integrationsService,
        CompetitionTaskListController controller)
    {
        competition.Organisation = organisation;

        competitionsService.GetCompetitionCriteriaReview(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);

        integrationsService.GetIntegrations().Returns(integrations);

        var expectedModel = new CompetitionReviewCriteriaModel(competition, integrations);

        var result = (await controller.ReviewCriteria(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task ReviewCriteria_SetsCriteriaAsReviewed(
        string internalOrgId,
        Competition competition,
        CompetitionReviewCriteriaModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionTaskListController controller)
    {
        var result = (await controller.ReviewCriteria(internalOrgId, competition.Id, model)).As<RedirectToActionResult>();

        await competitionsService
            .Received()
            .SetCriteriaReviewed(internalOrgId, competition.Id);

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmAwardCriteria_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionTaskListController controller)
    {
        competitionsService.GetCompetition(internalOrgId, competition.Id)
            .Returns(competition);

        var expectedModel = new CompetitionAwardCriteriaModel(competition);

        var result = (await controller.ConfirmAwardCriteria(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmAwardCriteria_RemovesNonPriceElements(
        string internalOrgId,
        int competitionId,
        CompetitionAwardCriteriaModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionTaskListController controller)
    {
        var result = (await controller.ConfirmAwardCriteria(internalOrgId, competitionId, model)).As<RedirectToActionResult>();

        await competitionsService.Received().SetCompetitionCriteria(internalOrgId, competitionId, false);
        await competitionsService.Received().RemoveNonPriceElements(internalOrgId, competitionId);

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }
}
