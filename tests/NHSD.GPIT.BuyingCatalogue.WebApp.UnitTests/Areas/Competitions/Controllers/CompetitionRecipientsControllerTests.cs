using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
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
                    new()
                    {
                        CompetitionId = CommonCompetitionId,
                        SublocationOdsCode = "XXXX",
                        OwnerOdsCode = CommonOrganisationExternalIdentifier,
                        SublocationRecipients = new List<CompetitionSublocationRecipient>
                        {
                            new()
                            {
                                CompetitionId = CommonCompetitionId,
                                RecipientOdsCode = "AAAA",
                                ParentSublocationOdsCode = "XXXX",
                            },
                            new()
                            {
                                CompetitionId = CommonCompetitionId,
                                RecipientOdsCode = "AAAB",
                                ParentSublocationOdsCode = "XXXX",
                            },
                        },
                    },
                    new()
                    {
                        CompetitionId = CommonCompetitionId,
                        SublocationOdsCode = "XXXA",
                        OwnerOdsCode = CommonOrganisationExternalIdentifier,
                        SublocationRecipients = new List<CompetitionSublocationRecipient>
                        {
                            new()
                            {
                                CompetitionId = CommonCompetitionId,
                                RecipientOdsCode = "AAAC",
                                ParentSublocationOdsCode = "XXXA",
                            },
                        },
                    },
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
                    new()
                    {
                        CompetitionId = CommonCompetitionId,
                        SublocationOdsCode = "XXXX",
                        OwnerOdsCode = CommonOrganisationExternalIdentifier,
                        SublocationRecipients = new List<CompetitionSublocationRecipient>
                        {
                            new()
                            {
                                CompetitionId = CommonCompetitionId,
                                RecipientOdsCode = "AAAA",
                                ParentSublocationOdsCode = "XXXX",
                            },
                            new()
                            {
                                CompetitionId = CommonCompetitionId,
                                RecipientOdsCode = "AAAB",
                                ParentSublocationOdsCode = "XXXX",
                            },
                        },
                    },
                    new()
                    {
                        CompetitionId = CommonCompetitionId,
                        SublocationOdsCode = "XXXA",
                        OwnerOdsCode = CommonOrganisationExternalIdentifier,
                        SublocationRecipients = new List<CompetitionSublocationRecipient>
                        {
                            new()
                            {
                                CompetitionId = CommonCompetitionId,
                                RecipientOdsCode = "AAAC",
                                ParentSublocationOdsCode = "XXXA",
                            },
                        },
                    },
                    new()
                    {
                        CompetitionId = CommonCompetitionId,
                        SublocationOdsCode = "XXXE",
                        OwnerOdsCode = CommonOrganisationExternalIdentifier,
                        SublocationRecipients = new List<CompetitionSublocationRecipient>(),
                    },
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
                    new()
                    {
                        CompetitionId = CommonCompetitionId,
                        SublocationOdsCode = "XXXX",
                        OwnerOdsCode = CommonOrganisationExternalIdentifier,
                    },
                    new()
                    {
                        CompetitionId = CommonCompetitionId,
                        SublocationOdsCode = "XXXA",
                        OwnerOdsCode = CommonOrganisationExternalIdentifier,
                    },
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
                new List<CompetitionSublocation>
                {
                    new()
                    {
                        CompetitionId = CommonCompetitionId,
                        SublocationOdsCode = "XXXX",
                        OwnerOdsCode = CommonOrganisationExternalIdentifier,
                    },
                },
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
                    new()
                    {
                        CompetitionId = CommonCompetitionId,
                        SublocationOdsCode = "XXXX",
                        OwnerOdsCode = CommonOrganisationExternalIdentifier,
                    },
                    new()
                    {
                        CompetitionId = CommonCompetitionId,
                        SublocationOdsCode = "XXXA",
                        OwnerOdsCode = CommonOrganisationExternalIdentifier,
                    },
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
                    new()
                    {
                        CompetitionId = CommonCompetitionId,
                        SublocationOdsCode = "XXXX",
                        OwnerOdsCode = CommonOrganisationExternalIdentifier,
                    },
                    new()
                    {
                        CompetitionId = CommonCompetitionId,
                        SublocationOdsCode = "XXXA",
                        OwnerOdsCode = CommonOrganisationExternalIdentifier,
                    },
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
                    new()
                    {
                        CompetitionId = CommonCompetitionId,
                        SublocationOdsCode = "XXXX",
                        OwnerOdsCode = CommonOrganisationExternalIdentifier,
                    },
                    new()
                    {
                        CompetitionId = CommonCompetitionId,
                        SublocationOdsCode = "XXXA",
                        OwnerOdsCode = CommonOrganisationExternalIdentifier,
                    },
                    new()
                    {
                        CompetitionId = CommonCompetitionId,
                        SublocationOdsCode = "XXXE",
                        OwnerOdsCode = CommonOrganisationExternalIdentifier,
                    },
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
