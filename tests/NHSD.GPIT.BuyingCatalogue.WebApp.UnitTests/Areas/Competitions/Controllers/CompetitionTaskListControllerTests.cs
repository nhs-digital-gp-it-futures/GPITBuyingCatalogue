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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;
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
    [CommonAutoData]
    public static async Task Index_ReturnsViewWithModel(
        Organisation organisation,
        CompetitionTaskListModel competitionTaskListModel,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionTaskListController controller)
    {
        organisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(organisation);

        competitionsService.Setup(x => x.GetCompetitionTaskList(organisation.Id, competitionTaskListModel.Id))
            .ReturnsAsync(competitionTaskListModel);

        var expectedModel = new CompetitionTaskListViewModel(organisation, competitionTaskListModel);

        var result = (await controller.Index(organisation.InternalIdentifier, competitionTaskListModel.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, x => x.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ShortlistedSolutions_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionTaskListController controller)
    {
        organisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(organisation);

        competitionsService.Setup(x => x.GetCompetitionWithServices(organisation.Id, competition.Id, false))
            .ReturnsAsync(competition);

        var expectedModel = new CompetitionShortlistedSolutionsModel(competition);

        var result = (await controller.ShortlistedSolutions(organisation.InternalIdentifier, competition.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, x => x.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ContractLength_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionTaskListController controller)
    {
        organisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(organisation);

        competitionsService.Setup(x => x.GetCompetition(organisation.InternalIdentifier, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new CompetitionContractModel(competition);

        var result = (await controller.ContractLength(organisation.InternalIdentifier, competition.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
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
    [CommonAutoData]
    public static async Task ContractLength_Valid_Redirects(
        Organisation organisation,
        int competitionId,
        CompetitionContractModel model,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionTaskListController controller)
    {
        organisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(organisation);

        var result = (await controller.ContractLength(organisation.InternalIdentifier, competitionId, model))
            .As<RedirectToActionResult>();

        competitionsService.Verify(
            x => x.SetContractLength(organisation.Id, competitionId, model.ContractLength.GetValueOrDefault()),
            Times.Once());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonAutoData]
    public static async Task AwardCriteria_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionTaskListController controller)
    {
        organisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(organisation);

        competitionsService.Setup(x => x.GetCompetition(organisation.InternalIdentifier, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new CompetitionAwardCriteriaModel(competition);

        var result = (await controller.AwardCriteria(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
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
    [CommonAutoData]
    public static async Task AwardCriteria_ValidModel_RedirectsToTaskList(
        Organisation organisation,
        Competition competition,
        CompetitionAwardCriteriaModel model,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionTaskListController controller)
    {
        competition.IncludesNonPrice = false;
        competition.Organisation = organisation;

        competitionsService.Setup(x => x.GetCompetition(organisation.InternalIdentifier, competition.Id))
            .ReturnsAsync(competition);

        var result = (await controller.AwardCriteria(organisation.InternalIdentifier, competition.Id, model))
            .As<RedirectToActionResult>();

        competitionsService.Verify(
            x => x.SetCompetitionCriteria(organisation.InternalIdentifier, competition.Id, model.IncludesNonPrice.GetValueOrDefault()),
            Times.Once());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonAutoData]
    public static async Task AwardCriteria_NonPriceToPriceOnly_RedirectsToConfirmation(
        Organisation organisation,
        Competition competition,
        CompetitionAwardCriteriaModel model,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionTaskListController controller)
    {
        competition.IncludesNonPrice = true;
        competition.Organisation = organisation;
        model.IncludesNonPrice = false;

        competitionsService.Setup(x => x.GetCompetition(organisation.InternalIdentifier, competition.Id))
            .ReturnsAsync(competition);

        var result = (await controller.AwardCriteria(organisation.InternalIdentifier, competition.Id, model))
            .As<RedirectToActionResult>();

        competitionsService.Verify(
            x => x.SetCompetitionCriteria(organisation.InternalIdentifier, competition.Id, model.IncludesNonPrice.GetValueOrDefault()),
            Times.Never());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ConfirmAwardCriteria));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Weightings_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionTaskListController controller)
    {
        organisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(organisation);

        competitionsService.Setup(x => x.GetCompetitionWithWeightings(organisation.Id, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new CompetitionWeightingsModel(competition);

        var result = (await controller.Weightings(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
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
    [CommonAutoData]
    public static async Task Weightings_ValidModel_Redirects(
        Organisation organisation,
        int competitionId,
        CompetitionWeightingsModel model,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionTaskListController controller)
    {
        organisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(organisation);

        var result = (await controller.Weightings(organisation.InternalIdentifier, competitionId, model))
            .As<RedirectToActionResult>();

        competitionsService.Verify(
            x => x.SetCompetitionWeightings(
                organisation.Id,
                competitionId,
                model.Price.GetValueOrDefault(),
                model.NonPrice.GetValueOrDefault()),
            Times.Once());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ReviewCriteria_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionTaskListController controller)
    {
        competition.Organisation = organisation;

        competitionsService.Setup(x => x.GetCompetitionCriteriaReview(organisation.InternalIdentifier, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new CompetitionReviewCriteriaModel(competition);

        var result = (await controller.ReviewCriteria(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ReviewCriteria_SetsCriteriaAsReviewed(
        string internalOrgId,
        Competition competition,
        CompetitionReviewCriteriaModel model,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionTaskListController controller)
    {
        var result = (await controller.ReviewCriteria(internalOrgId, competition.Id, model)).As<RedirectToActionResult>();

        competitionsService.Verify(x => x.SetCriteriaReviewed(internalOrgId, competition.Id), Times.Once());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ConfirmAwardCriteria_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionTaskListController controller)
    {
        competitionsService.Setup(x => x.GetCompetition(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new CompetitionAwardCriteriaModel(competition);

        var result = (await controller.ConfirmAwardCriteria(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ConfirmAwardCriteria_RemovesNonPriceElements(
        string internalOrgId,
        int competitionId,
        CompetitionAwardCriteriaModel model,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionTaskListController controller)
    {
        var result = (await controller.ConfirmAwardCriteria(internalOrgId, competitionId, model)).As<RedirectToActionResult>();

        competitionsService.Verify(x => x.SetCompetitionCriteria(internalOrgId, competitionId, false), Times.Once());
        competitionsService.Verify(x => x.RemoveNonPriceElements(internalOrgId, competitionId), Times.Once());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }
}
