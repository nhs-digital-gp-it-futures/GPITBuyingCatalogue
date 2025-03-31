using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using FluentAssertions.Equivalency;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Controllers;

public static class CompetitionRecipientsControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionRecipientsController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task UploadOrSelectServiceRecipients_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        competitionsService.GetCompetition(internalOrgId, competitionId)
            .Returns(competition);

        var result = await controller.UploadOrSelectServiceRecipients(internalOrgId, competitionId);

        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeOfType<UploadOrSelectServiceRecipientModel>().Subject;

        model.Should().NotBeNull();
        model.Caption.Should().Be(competition.Name);
        model.BackLink.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task UploadOrSelectServiceRecipients_InvalidModel_ReturnsViewWithModel(
        UploadOrSelectServiceRecipientModel model,
        string internalOrgId,
        int competitionId,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        controller.ModelState.AddModelError("SomeError", "Error message");
        competitionsService.GetCompetition(internalOrgId, competitionId)
            .Returns(competition);

        IActionResult result = await controller.UploadOrSelectServiceRecipients(model, internalOrgId, competitionId);

        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var returnedModel = viewResult.Model.Should().BeOfType<UploadOrSelectServiceRecipientModel>().Subject;

        returnedModel.Should().BeEquivalentTo(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task UploadOrSelectServiceRecipients_UploadRecipients_RedirectsToImportController(
        UploadOrSelectServiceRecipientModel model,
        string internalOrgId,
        int competitionId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        model.ShouldUploadRecipients = true;
        competitionsService.GetCompetition(internalOrgId, competitionId)
            .Returns(new Competition());

        IActionResult result = await controller.UploadOrSelectServiceRecipients(model, internalOrgId, competitionId);

        var redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectToActionResult.ActionName.Should().Be(nameof(CompetitionImportServiceRecipientsController.Index));
        redirectToActionResult.ControllerName.Should().Be(typeof(CompetitionImportServiceRecipientsController).ControllerName());
    }

    [Theory]
    [MockAutoData]
    public static async Task UploadOrSelectServiceRecipients_DoNotUploadRecipients_RedirectsToIndexAction(
        UploadOrSelectServiceRecipientModel model,
        string internalOrgId,
        int competitionId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        model.ShouldUploadRecipients = false;
        competitionsService.GetCompetition(internalOrgId, competitionId)
            .Returns(new Competition());

        IActionResult result = await controller.UploadOrSelectServiceRecipients(model, internalOrgId, competitionId);

        RedirectToActionResult redirectToActionResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectToActionResult.ActionName.Should().Be(nameof(CompetitionRecipientsController.SelectSublocations));
        redirectToActionResult.ControllerName.Should().Be(typeof(CompetitionRecipientsController).ControllerName());
    }

    [Theory]
    [MockMemberAutoData(nameof(ExistingAndNewSublocationsToRenderedSublocations))]
    public static async Task SelectSublocations_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> existingSublocations,
        List<OdsOrganisation> possibleSublocations,
        List<CheckboxNameAndValueModel> renderedSelections,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IOdsService odsService,
        CompetitionRecipientsController controller)
    {
        competition.CompetitionSublocations = existingSublocations;
        competition.Organisation = organisation;

        competitionsService.GetCompetitionWithSublocations(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);

        odsService.GetSublocationsByParentOdsCode(organisation.ExternalIdentifier).Returns(possibleSublocations);

        var expectedModel = new SelectSublocationsModel { RenderedSublocations = renderedSelections };

        var result = (await controller.SelectSublocations(organisation.InternalIdentifier, competition.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt.Excluding(m => m.BackLink)
                    .Excluding(m => m.Title)
                    .Excluding(m => m.Caption)
                    .Excluding(m => m.Advice));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectSublocations_Post_ErrorStateRejectsRequest(
        string internalOrgId,
        int competitionId,
        SelectSublocationsModel selectSublocationsModel,
        CompetitionRecipientsController controller)
    {
        controller.ModelState.AddModelError("some-property", "some-error");

        var result =
            (await controller.SelectSublocations(selectSublocationsModel, internalOrgId, competitionId))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(selectSublocationsModel);
    }

    [Theory]
    [MockMemberAutoData(nameof(ExpectedAddsFromSublocationSelection))]
    public static async Task SelectSublocations_Post_AddsOnly_PerformsAddServiceCall(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> existingSublocations,
        List<CheckboxNameAndValueModel> checkboxSelections,
        HashSet<string> expectedAdds,
        string expectedControllerName,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        competition.CompetitionSublocations = existingSublocations;
        competition.Organisation = organisation;

        competitionsService.GetCompetitionWithSublocations(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);

        var callingModel = new SelectSublocationsModel { RenderedSublocations = checkboxSelections };

        var result =
            (await controller.SelectSublocations(callingModel, organisation.InternalIdentifier, competition.Id))
            .As<RedirectToActionResult>();

        await competitionsService.Received()
            .AddSublocations(
                organisation.InternalIdentifier,
                competition.Id,
                Arg.Is<HashSet<string>>(hs => AreStringHashSetsEquivalent(hs, expectedAdds)));

        result.Should().NotBeNull();
        result.ActionName.Should().Be(expectedControllerName);
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", organisation.InternalIdentifier }, { "competitionId", competition.Id },
                });
    }

    [Theory]
    [MockMemberAutoData(nameof(ExpectedRemovesOrAddsAndRemoves))]
    public static async Task SelectSublocations_Post_RemovesOrAddsAndRemoves_RedirectsToRemovePage(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> existingSublocations,
        List<CheckboxNameAndValueModel> checkboxSelections,
        string expectedAddsAsConcatString,
        string expectedRemovesAsConcatString,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        competition.CompetitionSublocations = existingSublocations;
        competition.Organisation = organisation;

        competitionsService.GetCompetitionWithSublocations(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);

        var callingModel = new SelectSublocationsModel { RenderedSublocations = checkboxSelections };

        var result =
            (await controller.SelectSublocations(callingModel, organisation.InternalIdentifier, competition.Id))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.RemoveSublocations));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", organisation.InternalIdentifier },
                    { "competitionId", competition.Id },
                    { "sublocationsToRemove", expectedRemovesAsConcatString },
                    { "sublocationsToAdd", expectedAddsAsConcatString },
                });
    }

    [Theory]
    [MockAutoData]
    public static async Task AddSublocations_ReturnsSublocationsView(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        competition.OrganisationId = organisation.Id;
        competition.Organisation = organisation;
        competition.CompetitionSublocations = competitionSublocations;

        competitionsService.GetCompetitionWithSublocations(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);
        competitionsService.GetCountForCompetitionSublocationRecipients(
                organisation.InternalIdentifier,
                competition.Id,
                Arg.Any<string>())
            .Returns(
                call => competitionSublocations.First(x => x.SublocationOdsCode == call.ArgAt<string>(2))
                    .SublocationRecipients.Count);

        var expectedModel = new SelectSublocationsOverviewModel
        {
            Title = "Add sublocations",
            Caption = competition.Name,
            Advice = "Select a sublocation to add organisations to this competition",
            ProcessType = "competition",
            Sublocations = competitionSublocations.Select(
                    x => new SublocationModel
                    {
                        Name = x.SublocationOrganisation.Name,
                        ServiceRecipientCount = x.SublocationRecipients.Count,
                        OdsCode = x.SublocationOdsCode,
                    })
                .ToList(),
            ParentName = organisation.Name,
        };

        var result =
            (await controller.AddSublocations(organisation.InternalIdentifier, competition.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt.Excluding(model => model.BackLink)
                    .Excluding(model => model.AddOrChangeSublocationsHref)
                    .Excluding(model => model.Sublocations));

        IReadOnlyList<SublocationModel> sublocations = result.Model.As<SelectSublocationsOverviewModel>().Sublocations;

        sublocations.Should()
            .BeEquivalentTo(
                expectedModel.Sublocations,
                opt => opt.Excluding(slModel => slModel.RecipientHref).Excluding(slModel => slModel.TaskProgress));
    }

    [Theory]
    [MockAutoData]
    public static void AddSublocations_Post_ConditionalRedirect_RedirectToTasklistIfIncomplete(
        string internalOrganisationId,
        int competitionId,
        CompetitionRecipientsController controller)
    {
        var callingModel =
            new SelectSublocationsOverviewModel
            {
                Sublocations = [new SublocationModel { ServiceRecipientCount = 0 }],
            };

        var result =
            controller.AddSublocations(callingModel, internalOrganisationId, competitionId)
                .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionTaskListController.Index));
        result.ControllerName.Should().Be(typeof(CompetitionTaskListController).ControllerName());
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", internalOrganisationId }, { "competitionId", competitionId },
                });
    }

    [Theory]
    [MockAutoData]
    public static void AddSublocations_Post_ConditionalRedirect_RedirectToConfirmScreenIfComplete(
        string internalOrganisationId,
        int competitionId,
        CompetitionRecipientsController controller)
    {
        var callingModel =
            new SelectSublocationsOverviewModel
            {
                Sublocations = [new SublocationModel { ServiceRecipientCount = 1 }],
            };

        var result =
            controller.AddSublocations(callingModel, internalOrganisationId, competitionId)
                .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ConfirmSublocationRecipients));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", internalOrganisationId }, { "competitionId", competitionId },
                });
    }

    [Theory]
    [MockMemberAutoData(nameof(RemoveSublocationsUrlParamsToModel))]
    public static async Task RemoveSublocations_ReturnsView(
        string sublocationsToRemoveAsConcatString,
        string sublocationsToAddAsConcatString,
        RemoveSublocationsModel expectedModel,
        Organisation organisation,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        competition.Organisation = organisation;

        competitionsService.GetCompetition(organisation.InternalIdentifier, competition.Id).Returns(competition);

        var result =
            (await controller.RemoveSublocations(
                organisation.InternalIdentifier,
                competition.Id,
                sublocationsToRemoveAsConcatString,
                sublocationsToAddAsConcatString))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt.Excluding(m => m.ListHeaderText)
                    .Excluding(m => m.BackLink)
                    .Excluding(m => m.Title)
                    .Excluding(m => m.Caption)
                    .Excluding(m => m.Advice));

        var modelForFurtherComparison = result.Model.As<RemoveSublocationsModel>();

        modelForFurtherComparison.ListHeaderText.Should()
            .Be($"{competition.Organisation.Name} {expectedModel.Pluralisation} to be removed:");
        modelForFurtherComparison.Title.Should().Be($"Remove {expectedModel.Pluralisation}");
        modelForFurtherComparison.Caption.Should().Be(competition.Name);
        modelForFurtherComparison.Advice.Should().Be("Confirm you want to remove sublocations from this competition");
    }

    [Theory]
    [MockAutoData]
    public static async Task RemoveSublocations_Post_NoServiceCallsIfNo(
        string internalOrganisationId,
        int competitionId,
        List<string> sublocationIdsToRemove,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        var callingModel =
            new RemoveSublocationsModel { ConfirmRemove = false, SublocationIdsToRemove = sublocationIdsToRemove };

        var result =
            (await controller.RemoveSublocations(callingModel, internalOrganisationId, competitionId))
            .As<RedirectToActionResult>();

        await competitionsService.DidNotReceiveWithAnyArgs().AddSublocations(null, 0, null);
        await competitionsService.DidNotReceiveWithAnyArgs().RemoveSublocations(null, 0, null);

        result.ActionName.Should().Be(nameof(controller.ConfirmSublocations));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", internalOrganisationId }, { "competitionId", competitionId },
                });
    }

    [Theory]
    [MockAutoData]
    public static async Task RemoveSublocations_Post_NoServiceCallsIfNotPopulated(
        string internalOrganisationId,
        int competitionId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        var callingModel =
            new RemoveSublocationsModel { ConfirmRemove = true, SublocationIdsToRemove = [], SublocationIdsToAdd = [] };

        var result =
            (await controller.RemoveSublocations(callingModel, internalOrganisationId, competitionId))
            .As<RedirectToActionResult>();

        await competitionsService.DidNotReceiveWithAnyArgs().AddSublocations(null, 0, null);
        await competitionsService.DidNotReceiveWithAnyArgs().RemoveSublocations(null, 0, null);

        result.ActionName.Should().Be(nameof(controller.ConfirmSublocations));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", internalOrganisationId }, { "competitionId", competitionId },
                });
    }

    [Theory]
    [MockAutoData]
    public static async Task RemoveSublocations_Post_PerformsServiceCallsAndRedirects(
        string internalOrganisationId,
        int competitionId,
        List<string> sublocationIdsToRemove,
        List<string> sublocationIdsToAdd,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        var callingModel =
            new RemoveSublocationsModel
            {
                ConfirmRemove = true,
                SublocationIdsToRemove = sublocationIdsToRemove,
                SublocationIdsToAdd = sublocationIdsToAdd,
            };

        var result =
            (await controller.RemoveSublocations(callingModel, internalOrganisationId, competitionId))
            .As<RedirectToActionResult>();

        await competitionsService.Received()
            .RemoveSublocations(
                internalOrganisationId,
                competitionId,
                Arg.Is<HashSet<string>>(hs => AreStringHashSetsEquivalent(hs, sublocationIdsToRemove.ToHashSet())));

        await competitionsService.Received()
            .AddSublocations(
                internalOrganisationId,
                competitionId,
                Arg.Is<HashSet<string>>(hs => AreStringHashSetsEquivalent(hs, sublocationIdsToAdd.ToHashSet())));

        result.ActionName.Should().Be(nameof(controller.ConfirmSublocations));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { "internalOrgId", internalOrganisationId }, { "competitionId", competitionId },
                });
    }

    [Theory]
    [MockMemberAutoData(nameof(PreviousSelectionsAndPotentialRecipientsToExpected))]
    public static async Task
        SelectSublocationRecipients_PreviousSelectionsScenarios_ReturnsViewWithSelectionsAsExpected(
            Organisation organisation,
            Competition competition,
            List<ServiceRecipient> possibleRecipients,
            CompetitionSublocation workingSublocation,
            SublocationModel expectedSublocationModel,
            List<ServiceRecipientModel> expectedRendered,
            [Frozen] ICompetitionSublocationService competitionSublocationService,
            [Frozen] IOrganisationsService organisationsService,
            [Frozen] IOdsService odsOrganisationsService,
            CompetitionRecipientsController controller)
    {
        competition.OrganisationId = organisation.Id;
        competition.Organisation = organisation;

        workingSublocation.Competition = competition;

        organisationsService.GetOrganisationExternalIdentifierByInternalIdentifier(organisation.InternalIdentifier)
            .Returns(organisation.ExternalIdentifier);
        odsOrganisationsService.GetServiceRecipientsBySublocation(workingSublocation.SublocationOdsCode)
            .Returns(possibleRecipients);
        competitionSublocationService.GetCompetitionSublocationWithRecipients(
                organisation.ExternalIdentifier,
                competition.Id,
                workingSublocation.SublocationOdsCode)
            .Returns(workingSublocation);

        var expectedModel = new SelectSublocationRecipientsModel
        {
            Sublocation = expectedSublocationModel,
            IsAmendment = false,
            RenderedServiceRecipients = expectedRendered,
        };

        var result =
            (await controller.SelectSublocationRecipients(
                organisation.InternalIdentifier,
                competition.Id,
                workingSublocation.SublocationOdsCode))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt.Excluding(m => m.Title)
                    .Excluding(m => m.Caption)
                    .Excluding(m => m.Advice)
                    .Excluding(m => m.BackLink)
                    .Excluding(m => m.Sublocation)
                    .Excluding(m => m.PreviouslySelected)
                    .Excluding(m => m.RenderedServiceRecipients));

        SublocationModel sublocationForFurtherEvaluation =
            result.Model.As<SelectSublocationRecipientsModel>().Sublocation;

        sublocationForFurtherEvaluation.Should()
            .BeEquivalentTo(
                expectedSublocationModel,
                opt => opt.Excluding(m => m.TaskProgress)
                    .Excluding(m => m.ServiceRecipientCount)
                    .Excluding(m => m.ServiceRecipients));

        Func<EquivalencyAssertionOptions<ServiceRecipientModel>, EquivalencyAssertionOptions<ServiceRecipientModel>>
            commonNameDescriptionExclusionConfig = opt =>
                opt.Excluding(m => m.Name)
                    .Excluding(m => m.Description)
                    .WithoutStrictOrdering(); // Ordering provided by DB not controller

        sublocationForFurtherEvaluation.ServiceRecipients.Should()
            .BeEquivalentTo(
                expectedSublocationModel.ServiceRecipients,
                commonNameDescriptionExclusionConfig);

        IEnumerable<ServiceRecipientModel> previouslySelectedServiceRecipientsForFurtherEvaluation =
            result.Model.As<SelectSublocationRecipientsModel>().PreviouslySelected;

        previouslySelectedServiceRecipientsForFurtherEvaluation.Should()
            .BeEquivalentTo(
                expectedSublocationModel.ServiceRecipients,
                commonNameDescriptionExclusionConfig);

        IReadOnlyList<ServiceRecipientModel> renderedRecipientsForFurtherEvaluation =
            result.Model.As<SelectSublocationRecipientsModel>().RenderedServiceRecipients;

        renderedRecipientsForFurtherEvaluation.Should()
            .BeEquivalentTo(
                expectedModel.RenderedServiceRecipients,
                commonNameDescriptionExclusionConfig);
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectSublocationRecipients_SelectionMode_All_ReturnsViewAllItemsSelected(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        [Frozen] ICompetitionSublocationService competitionSublocationService,
        [Frozen] IOrganisationsService organisationsService,
        CompetitionRecipientsController controller)
    {
        Assert.Fail("not implemented");
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectSublocationRecipients_SelectionMode_None_ReturnsViewAllItemsDeselected(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        [Frozen] ICompetitionSublocationService competitionSublocationService,
        [Frozen] IOrganisationsService organisationsService,
        CompetitionRecipientsController controller)
    {
        Assert.Fail("not implemented");
    }

    [Theory]
    [MockAutoData]
    public static async Task SublocationOdsCode_Post_ReturnsViewOnError(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        Assert.Fail("not implemented");
    }

    [Theory]
    [MockAutoData]
    public static async Task SublocationOdsCode_Post_PerformsServiceCallsAndRedirects(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        Assert.Fail("not implemented");
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmSublocations_ReturnsView(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        Assert.Fail("not implemented");
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmSublocations_Post_Redirects(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        Assert.Fail("not implemented");
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmSublocationsRecipients_ReturnsView(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        Assert.Fail("not implemented");
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmSublocationsRecipients_Post_Redirects(
        Organisation organisation,
        Competition competition,
        List<CompetitionSublocation> competitionSublocations,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionRecipientsController controller)
    {
        Assert.Fail("not implemented");
    }

    private static IEnumerable<object[]> ExistingAndNewSublocationsToRenderedSublocations()
    {
        return
        [
            // 2 Existing sublocations that should be ticked, and a new one that shouldn't
            [
                CommonOrganisationFactory(),
                CommonCompetitionFactory(),
                new List<CompetitionSublocation>
                {
                    CommonCompetitionSublocationFactory(
                        "XXXX",
                        [
                            CommonCompetitionSublocationRecipientFactory("AAAA", "XXXX"),
                            CommonCompetitionSublocationRecipientFactory("AAAB", "XXXX"),
                        ]),
                    CommonCompetitionSublocationFactory(
                        "XXXA",
                        [
                            CommonCompetitionSublocationRecipientFactory("AAAC", "XXXA"),
                        ]),
                },
                new List<OdsOrganisation>
                {
                    new() { OdsCode = "XXXX", OrganisationName = "An existing sublocation - XXXX" },
                    new() { OdsCode = "XXXA", OrganisationName = "An existing sublocation - XXXA" },
                    new() { OdsCode = "XXXE", OrganisationName = "A new sublocation - XXXE" },
                },
                new List<CheckboxNameAndValueModel>
                {
                    new() { Name = "XXXX", Value = true },
                    new() { Name = "XXXA", Value = true },
                    new() { Name = "XXXE", Value = false },
                },
            ],

            // 3 Existing sublocations with no new - should all be ticked
            [
                CommonOrganisationFactory(),
                CommonCompetitionFactory(),
                new List<CompetitionSublocation>
                {
                    CommonCompetitionSublocationFactory(
                        "XXXX",
                        [
                            CommonCompetitionSublocationRecipientFactory("AAAA", "XXXX"),
                            CommonCompetitionSublocationRecipientFactory("AAAB", "XXXX"),
                        ]),
                    CommonCompetitionSublocationFactory(
                        "XXXA",
                        [
                            CommonCompetitionSublocationRecipientFactory("AAAC", "XXXA"),
                        ]),
                    CommonCompetitionSublocationFactory(
                        "XXXE"),
                },
                new List<OdsOrganisation>
                {
                    new() { OdsCode = "XXXX", OrganisationName = "An existing sublocation - XXXX" },
                    new() { OdsCode = "XXXA", OrganisationName = "An existing sublocation - XXXA" },
                    new() { OdsCode = "XXXE", OrganisationName = "A new sublocation - XXXE" },
                },
                new List<CheckboxNameAndValueModel>
                {
                    new() { Name = "XXXX", Value = true },
                    new() { Name = "XXXA", Value = true },
                    new() { Name = "XXXE", Value = true },
                },
            ],

            // 3 all new sublocations - none should be ticked
            [
                CommonOrganisationFactory(),
                CommonCompetitionFactory(),
                new List<CompetitionSublocation>(),
                new List<OdsOrganisation>
                {
                    new() { OdsCode = "XXXX", OrganisationName = "An existing sublocation - XXXX" },
                    new() { OdsCode = "XXXA", OrganisationName = "An existing sublocation - XXXA" },
                    new() { OdsCode = "XXXE", OrganisationName = "A new sublocation - XXXE" },
                },
                new List<CheckboxNameAndValueModel>
                {
                    new() { Name = "XXXX", Value = false },
                    new() { Name = "XXXA", Value = false },
                    new() { Name = "XXXE", Value = false },
                },
            ],
        ];
    }

    private static IEnumerable<object[]> ExpectedAddsFromSublocationSelection()
    {
        return
        [
            // 2 existing and 3 ticked resulting in 1 add
            [
                CommonOrganisationFactory(),
                CommonCompetitionFactory(),
                new List<CompetitionSublocation>
                {
                    CommonCompetitionSublocationFactory("XXXX"), CommonCompetitionSublocationFactory("XXXA"),
                },
                new List<CheckboxNameAndValueModel>
                {
                    new() { Name = "XXXX", Value = true },
                    new() { Name = "XXXA", Value = true },
                    new() { Name = "XXXE", Value = true },
                },
                new HashSet<string> { "XXXE" },
                nameof(CompetitionRecipientsController.ConfirmSublocations),
            ],

            // 1 existing and 3 ticked resulting in 2 adds
            [
                CommonOrganisationFactory(),
                CommonCompetitionFactory(),
                new List<CompetitionSublocation> { CommonCompetitionSublocationFactory("XXXX") },
                new List<CheckboxNameAndValueModel>
                {
                    new() { Name = "XXXX", Value = true },
                    new() { Name = "XXXA", Value = true },
                    new() { Name = "XXXE", Value = true },
                },
                new HashSet<string> { "XXXA", "XXXE" },
                nameof(CompetitionRecipientsController.ConfirmSublocations),
            ],

            // 0 existing and 3 ticked resulting in 3 adds - also redirects to 'Add sublocation' page instead of 'confirm'
            [
                CommonOrganisationFactory(),
                CommonCompetitionFactory(),
                new List<CompetitionSublocation>(),
                new List<CheckboxNameAndValueModel>
                {
                    new() { Name = "XXXX", Value = true },
                    new() { Name = "XXXA", Value = true },
                    new() { Name = "XXXE", Value = true },
                },
                new HashSet<string> { "XXXX", "XXXA", "XXXE" },
                nameof(CompetitionRecipientsController.AddSublocations),
            ],
        ];
    }

    private static IEnumerable<object[]> ExpectedRemovesOrAddsAndRemoves()
    {
        return
        [
            // 2 existing, 1 unticked and 2 ticked resulting in 1 add and 1 remove
            [
                CommonOrganisationFactory(),
                CommonCompetitionFactory(),
                new List<CompetitionSublocation>
                {
                    CommonCompetitionSublocationFactory("XXXX"), CommonCompetitionSublocationFactory("XXXA"),
                },
                new List<CheckboxNameAndValueModel>
                {
                    new() { Name = "XXXX", Value = false },
                    new() { Name = "XXXA", Value = true },
                    new() { Name = "XXXE", Value = true },
                },
                "XXXE",
                "XXXX",
            ],

            // 2 existing 1 unticked and 1 ticked resulting in 1 remove no adds
            [
                CommonOrganisationFactory(),
                CommonCompetitionFactory(),
                new List<CompetitionSublocation>
                {
                    CommonCompetitionSublocationFactory("XXXX"), CommonCompetitionSublocationFactory("XXXA"),
                },
                new List<CheckboxNameAndValueModel>
                {
                    new() { Name = "XXXX", Value = false }, new() { Name = "XXXA", Value = true },
                },
                string.Empty,
                "XXXX",
            ],

            // 3 existing and 1 ticked resulting in 2 removes
            [
                CommonOrganisationFactory(),
                CommonCompetitionFactory(),
                new List<CompetitionSublocation>
                {
                    CommonCompetitionSublocationFactory("XXXX"),
                    CommonCompetitionSublocationFactory("XXXA"),
                    CommonCompetitionSublocationFactory("XXXE"),
                },
                new List<CheckboxNameAndValueModel>
                {
                    new() { Name = "XXXX", Value = false },
                    new() { Name = "XXXA", Value = false },
                    new() { Name = "XXXE", Value = true },
                },
                string.Empty,
                "XXXX,XXXA",
            ],
        ];
    }

    private static IEnumerable<object[]> RemoveSublocationsUrlParamsToModel()
    {
        return
        [
            // 3 to remove, 2 to add
            [
                "AAAA,AAAB,AAAC", "AAAE,AAAF",
                new RemoveSublocationsModel
                {
                    SublocationIdsToRemove =
                        ["AAAA", "AAAB", "AAAC"],
                    SublocationIdsToAdd = ["AAAE", "AAAF"],
                    Pluralisation = "sublocations",
                },
            ],

            // 1 to remove, 0 to add
            [
                "AAAA", string.Empty,
                new RemoveSublocationsModel
                {
                    SublocationIdsToRemove = ["AAAA"], SublocationIdsToAdd = [], Pluralisation = "sublocation",
                },
            ],

            // 1 to remove, 3 to add
            [
                "AAAA", "AAAE,AAAF,AAAC",
                new RemoveSublocationsModel
                {
                    SublocationIdsToRemove =
                        ["AAAA"],
                    SublocationIdsToAdd = ["AAAE", "AAAF", "AAAC"],
                    Pluralisation = "sublocation",
                },
            ],
        ];
    }

    private static IEnumerable<object[]> PreviousSelectionsAndPotentialRecipientsToExpected()
    {
        List<ServiceRecipient> possibleRecipientRepo =
        [
            CommonServiceRecipientFactory("AAAA", "XXXX"),
            CommonServiceRecipientFactory("AAAB", "XXXX"),
            CommonServiceRecipientFactory("AAAC", "XXXX"),
        ];

        return
        [
            // 1 existing + repo = 3 rendered with 1 existing selected
            [
                CommonOrganisationFactory(), CommonCompetitionFactory(), possibleRecipientRepo,
                CommonCompetitionSublocationFactory(
                    "XXXX",
                    [CommonCompetitionSublocationRecipientFactory("AAAA", "XXXX")]),

                commonSublocationModelFactory("XXXX", [CommonServiceRecipientModelFactory("AAAA", "XXXX", true)]),
                new List<ServiceRecipientModel>
                {
                    CommonServiceRecipientModelFactory("AAAA", "XXXX", true),
                    CommonServiceRecipientModelFactory("AAAB", "XXXX", false),
                    CommonServiceRecipientModelFactory("AAAC", "XXXX", false),
                },
            ],

            // 0 existing + repo = 3 rendered with 0 existing selected
            [
                CommonOrganisationFactory(), CommonCompetitionFactory(), possibleRecipientRepo,
                CommonCompetitionSublocationFactory(
                    "XXXX",
                    []),

                commonSublocationModelFactory(
                    "XXXX",
                    [
                    ]),
                new List<ServiceRecipientModel>
                {
                    CommonServiceRecipientModelFactory("AAAA", "XXXX", false),
                    CommonServiceRecipientModelFactory("AAAB", "XXXX", false),
                    CommonServiceRecipientModelFactory("AAAC", "XXXX", false),
                },
            ],

            // 3 existing + repo = 3 rendered with 3 existing selected
            [
                CommonOrganisationFactory(), CommonCompetitionFactory(), possibleRecipientRepo,
                CommonCompetitionSublocationFactory(
                    "XXXX",
                    [
                        CommonCompetitionSublocationRecipientFactory("AAAA", "XXXX"),
                        CommonCompetitionSublocationRecipientFactory("AAAB", "XXXX"),
                        CommonCompetitionSublocationRecipientFactory("AAAC", "XXXX"),
                    ]),

                commonSublocationModelFactory(
                    "XXXX",
                    [
                        CommonServiceRecipientModelFactory("AAAA", "XXXX", true),
                        CommonServiceRecipientModelFactory("AAAB", "XXXX", true),
                        CommonServiceRecipientModelFactory("AAAC", "XXXX", true),
                    ]),
                new List<ServiceRecipientModel>
                {
                    CommonServiceRecipientModelFactory("AAAA", "XXXX", true),
                    CommonServiceRecipientModelFactory("AAAB", "XXXX", true),
                    CommonServiceRecipientModelFactory("AAAC", "XXXX", true),
                },
            ],

            // 3 existing + empty repo = 3 rendered with 3 existing selected
            [
                CommonOrganisationFactory(), CommonCompetitionFactory(), new List<ServiceRecipient>(),
                CommonCompetitionSublocationFactory(
                    "XXXX",
                    [
                        CommonCompetitionSublocationRecipientFactory("AAAA", "XXXX"),
                        CommonCompetitionSublocationRecipientFactory("AAAB", "XXXX"),
                        CommonCompetitionSublocationRecipientFactory("AAAC", "XXXX"),
                    ]),

                commonSublocationModelFactory(
                    "XXXX",
                    [
                        CommonServiceRecipientModelFactory("AAAA", "XXXX", true),
                        CommonServiceRecipientModelFactory("AAAB", "XXXX", true),
                        CommonServiceRecipientModelFactory("AAAC", "XXXX", true),
                    ]),
                new List<ServiceRecipientModel>
                {
                    CommonServiceRecipientModelFactory("AAAA", "XXXX", true),
                    CommonServiceRecipientModelFactory("AAAB", "XXXX", true),
                    CommonServiceRecipientModelFactory("AAAC", "XXXX", true),
                },
            ],

            // 3 existing (not in repo) + repo = 6 rendered with 3 existing selected
            [
                CommonOrganisationFactory(), CommonCompetitionFactory(), possibleRecipientRepo,
                CommonCompetitionSublocationFactory(
                    "XXXX",
                    [
                        CommonCompetitionSublocationRecipientFactory("BAAA", "XXXX"),
                        CommonCompetitionSublocationRecipientFactory("BAAB", "XXXX"),
                        CommonCompetitionSublocationRecipientFactory("BAAC", "XXXX"),
                    ]),

                commonSublocationModelFactory(
                    "XXXX",
                    [
                        CommonServiceRecipientModelFactory("BAAA", "XXXX", true),
                        CommonServiceRecipientModelFactory("BAAB", "XXXX", true),
                        CommonServiceRecipientModelFactory("BAAC", "XXXX", true),
                    ]),
                new List<ServiceRecipientModel>
                {
                    CommonServiceRecipientModelFactory("AAAA", "XXXX", false),
                    CommonServiceRecipientModelFactory("AAAB", "XXXX", false),
                    CommonServiceRecipientModelFactory("AAAC", "XXXX", false),
                    CommonServiceRecipientModelFactory("BAAA", "XXXX", true),
                    CommonServiceRecipientModelFactory("BAAB", "XXXX", true),
                    CommonServiceRecipientModelFactory("BAAC", "XXXX", true),
                },
            ],
        ];
    }

    private const int CommonCompetitionId = 34;

    private const int CommonOrganisationId = 21;
    private const string CommonOrganisationInternalIdentifier = "BB-FFGG";
    private const string CommonOrganisationExternalIdentifier = "FFGG";

    private static Organisation CommonOrganisationFactory()
    {
        return new Organisation
        {
            Id = CommonCompetitionId,
            InternalIdentifier = CommonOrganisationInternalIdentifier,
            ExternalIdentifier = CommonOrganisationExternalIdentifier,
            Name = "A Local ICB",
        };
    }

    private static Competition CommonCompetitionFactory()
    {
        return new Competition
        {
            Id = CommonCompetitionId,
            OrganisationId = CommonOrganisationId,
            Name = "My Competition",
            Description = "Competition for competitiony things",
        };
    }

    private static CompetitionSublocation CommonCompetitionSublocationFactory(
        string sublocationOdsCode,
        List<CompetitionSublocationRecipient> sublocationRecipients = null)
    {
        return new CompetitionSublocation
        {
            CompetitionId = CommonCompetitionId,
            SublocationOdsCode = sublocationOdsCode,
            OwnerOdsCode = CommonOrganisationExternalIdentifier,
            SublocationRecipients = sublocationRecipients,
        };
    }

    private static CompetitionSublocationRecipient CommonCompetitionSublocationRecipientFactory(
        string recipientOdsCode,
        string parentSublocationOdsCode)
    {
        return new CompetitionSublocationRecipient
        {
            CompetitionId = CommonCompetitionId,
            RecipientOdsCode = recipientOdsCode,
            ParentSublocationOdsCode = parentSublocationOdsCode,
        };
    }

    private static ServiceRecipient CommonServiceRecipientFactory(string orgId, string locationOrgId)
    {
        return new ServiceRecipient { OrgId = orgId, LocationOrgId = locationOrgId };
    }

    private static SublocationModel commonSublocationModelFactory(
        string odsCode,
        IReadOnlyList<ServiceRecipientModel> serviceRecipients)
    {
        return new SublocationModel { OdsCode = odsCode, ServiceRecipients = serviceRecipients };
    }

    private static ServiceRecipientModel CommonServiceRecipientModelFactory(
        string odsCode,
        string locationOrgId,
        bool selected)
    {
        return new ServiceRecipientModel { OdsCode = odsCode, LocationOrgId = locationOrgId, Selected = selected };
    }

    private static bool AreStringHashSetsEquivalent(
        HashSet<string> actual,
        HashSet<string> expected)
    {
        try
        {
            actual.Should().BeEquivalentTo(expected);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
