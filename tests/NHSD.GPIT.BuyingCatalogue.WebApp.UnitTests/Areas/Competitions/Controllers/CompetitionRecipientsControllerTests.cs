using System.Collections.Generic;
using System.Linq;
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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using Xunit;
using OdsOrganisation = NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models.OdsOrganisation;
using SelectRecipientsModel = NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.SelectRecipientsModel;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Controllers;

public static class CompetitionRecipientsControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionRecipientsController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Index_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        List<OdsOrganisation> odsOrganisations,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        [Frozen] Mock<IOdsService> odsService,
        CompetitionRecipientsController controller)
    {
        organisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(organisation);

        competitionsService.Setup(x => x.GetCompetitionWithRecipients(organisation.InternalIdentifier, competition.Id))
            .ReturnsAsync(competition);

        odsService.Setup(x => x.GetServiceRecipientsByParentInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(odsOrganisations.Select(x => new ServiceRecipient { Name = x.Name, OrgId = x.Id, }));

        var result = (await controller.Index(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();
    }

    [Theory]
    [CommonAutoData]
    public static void Index_InvalidModelState_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        SelectRecipientsModel model,
        CompetitionRecipientsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = controller.Index(internalOrgId, competitionId, model).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.ShouldExpand));
    }

    [Theory]
    [CommonAutoData]
    public static void Index_Valid_Redirects(
        string internalOrgId,
        int competitionId,
        SelectRecipientsModel model,
        CompetitionRecipientsController controller)
    {
        var result = controller.Index(internalOrgId, competitionId, model).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().BeEquivalentTo(nameof(controller.ConfirmRecipients));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ConfirmRecipients_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        List<ServiceRecipient> serviceRecipients,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        [Frozen] Mock<IOdsService> odsService,
        CompetitionRecipientsController controller)
    {
        organisationsService.Setup(x => x.GetOrganisationByInternalIdentifier(organisation.InternalIdentifier))
            .ReturnsAsync(organisation);

        competitionsService.Setup(x => x.GetCompetition(organisation.InternalIdentifier, competition.Id))
            .ReturnsAsync(competition);

        odsService.Setup(
                x => x.GetServiceRecipientsById(organisation.InternalIdentifier, It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(serviceRecipients);

        var expectedModel = new ConfirmChangesModel(organisation)
        {
            Caption = competition.Name,
            Selected = serviceRecipients.Select(
                    x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OrgId, Location = x.Location })
                .ToList(),
            Advice = CompetitionRecipientsController.ConfirmRecipientsAdvice,
        };

        var result = (await controller.ConfirmRecipients(
            organisation.InternalIdentifier,
            competition.Id,
            string.Join(',', serviceRecipients.Select(x => x.OrgId)))).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt
            .Excluding(m => m.BackLink)
            .Excluding(m => m.AddRemoveRecipientsLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ConfirmRecipients_Valid_Redirects(
        string internalOrgId,
        int competitionId,
        ConfirmChangesModel model,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionRecipientsController controller)
    {
        var result = (await controller.ConfirmRecipients(internalOrgId, competitionId, model))
            .As<RedirectToActionResult>();

        competitionsService.Verify(x => x.SetCompetitionRecipients(competitionId, It.IsAny<IEnumerable<string>>()), Times.Once());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionTaskListController.Index));
        result.ControllerName.Should().Be(typeof(CompetitionTaskListController).ControllerName());
    }

    [Theory]
    [CommonAutoData]
    public static async Task UploadOrSelectServiceRecipients_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionRecipientsController controller)
    {
        competitionsService.Setup(x => x.GetCompetition(internalOrgId, competitionId))
            .ReturnsAsync(competition);

        var result = await controller.UploadOrSelectServiceRecipients(internalOrgId, competitionId);

        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<UploadOrSelectServiceRecipientModel>().Subject;

        model.Should().NotBeNull();
        model.Caption.Should().Be(competition.Name);
        model.BackLink.Should().NotBeNull();
    }

    [Theory]
    [CommonAutoData]
    public static void UploadOrSelectServiceRecipients_InvalidModel_ReturnsViewWithModel(
        UploadOrSelectServiceRecipientModel model,
        string internalOrgId,
        int competitionId,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionRecipientsController controller)
    {
        controller.ModelState.AddModelError("SomeError", "Error message");
        competitionsService.Setup(x => x.GetCompetition(internalOrgId, competitionId))
            .ReturnsAsync(competition);

        var result = controller.UploadOrSelectServiceRecipients(model, internalOrgId, competitionId);

        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var returnedModel = viewResult.Model.Should().BeOfType<UploadOrSelectServiceRecipientModel>().Subject;

        returnedModel.Should().BeEquivalentTo(model);
    }

    [Theory]
    [CommonAutoData]
    public static void UploadOrSelectServiceRecipients_UploadRecipients_RedirectsToImportController(
        UploadOrSelectServiceRecipientModel model,
        string internalOrgId,
        int competitionId,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionRecipientsController controller)
    {
        model.ShouldUploadRecipients = true;
        competitionsService.Setup(x => x.GetCompetition(internalOrgId, competitionId))
            .ReturnsAsync(new Competition());

        var result = controller.UploadOrSelectServiceRecipients(model, internalOrgId, competitionId);

        var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectToActionResult.ActionName.Should().Be(nameof(CompetitionImportServiceRecipientsController.Index));
        redirectToActionResult.ControllerName.Should().Be(typeof(CompetitionImportServiceRecipientsController).ControllerName());
    }

    [Theory]
    [CommonAutoData]
    public static void UploadOrSelectServiceRecipients_DoNotUploadRecipients_RedirectsToIndexAction(
        UploadOrSelectServiceRecipientModel model,
        string internalOrgId,
        int competitionId,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionRecipientsController controller)
    {
        model.ShouldUploadRecipients = false;
        competitionsService.Setup(x => x.GetCompetition(internalOrgId, competitionId))
            .ReturnsAsync(new Competition());

        var result = controller.UploadOrSelectServiceRecipients(model, internalOrgId, competitionId);

        var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectToActionResult.ActionName.Should().Be(nameof(CompetitionRecipientsController.Index));
        redirectToActionResult.ControllerName.Should().Be(typeof(CompetitionRecipientsController).ControllerName());
    }
}
