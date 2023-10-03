using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels.FeaturesModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Controllers;

public static class CompetitionNonPriceElementsControllerTests
{
    public static IEnumerable<object[]> GetRedirectNonPriceElementTestData => new[]
    {
        new object[]
        {
            new[]
            {
                NonPriceElement.Implementation,
                NonPriceElement.Interoperability,
                NonPriceElement.ServiceLevel,
            },
            NonPriceElement.Implementation,
        },
        new object[]
        {
            new[] { NonPriceElement.Interoperability, NonPriceElement.ServiceLevel },
            NonPriceElement.Interoperability,
        },
        new object[] { new[] { NonPriceElement.ServiceLevel }, NonPriceElement.ServiceLevel, },
    };

    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionNonPriceElementsController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Index_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.Organisation = organisation;

        competitionsService.Setup(
                x => x.GetCompetitionWithNonPriceElements(organisation.InternalIdentifier, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new NonPriceElementsModel(competition);

        var result = (await controller.Index(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task AddNonPriceElement_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.Organisation = organisation;

        competitionsService.Setup(
                x => x.GetCompetitionWithNonPriceElements(organisation.InternalIdentifier, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new AddNonPriceElementModel(competition);

        var result = (await controller.AddNonPriceElement(organisation.InternalIdentifier, competition.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task AddNonPriceElement_NoAvailableNonPriceElements_Redirects(
        Organisation organisation,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.Organisation = organisation;
        competition.NonPriceElements = new()
        {
            Implementation = new(),
            Interoperability = new List<InteroperabilityCriteria> { new(), },
            ServiceLevel = new(),
            Features = new List<FeaturesCriteria> { new() },
        };

        competitionsService.Setup(
                x => x.GetCompetitionWithNonPriceElements(organisation.InternalIdentifier, competition.Id))
            .ReturnsAsync(competition);

        var result = (await controller.AddNonPriceElement(organisation.InternalIdentifier, competition.Id))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonAutoData]
    public static void AddNonPriceElement_InvalidModel_ReturnsView(
        string internalOrgId,
        int competitionId,
        AddNonPriceElementModel model,
        CompetitionNonPriceElementsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = controller.AddNonPriceElement(internalOrgId, competitionId, model).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [CommonAutoData]
    public static void AddNonPriceElement_SelectedElements_Redirects(
        string internalOrgId,
        int competitionId,
        AddNonPriceElementModel model,
        CompetitionNonPriceElementsController controller)
    {
        model.AvailableNonPriceElements = new List<SelectOption<NonPriceElement>>
        {
            new(NonPriceElement.Implementation.EnumMemberName(), NonPriceElement.Implementation, true),
            new(NonPriceElement.Interoperability.EnumMemberName(), NonPriceElement.Interoperability, true),
            new(NonPriceElement.ServiceLevel.EnumMemberName(), NonPriceElement.ServiceLevel, true),
        };

        var result = controller.AddNonPriceElement(internalOrgId, competitionId, model).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(NonPriceElement.Implementation.ToString());
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(competitionId), competitionId },
                    {
                        "selectedNonPriceElements",
                        string.Join(',', new[] { NonPriceElement.Interoperability, NonPriceElement.ServiceLevel })
                    },
                });
    }

    [Theory]
    [CommonAutoData]
    public static async Task Interoperability_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.Organisation = organisation;

        competitionsService.Setup(
                x => x.GetCompetitionWithNonPriceElements(organisation.InternalIdentifier, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new SelectInteroperabilityCriteriaModel(competition)
        {
            InternalOrgId = organisation.InternalIdentifier, CompetitionId = competition.Id,
        };

        var result = (await controller.Interoperability(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Interoperability_InvalidModel_ReturnsView(
        string internalOrgId,
        int competitionId,
        SelectInteroperabilityCriteriaModel model,
        CompetitionNonPriceElementsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.Interoperability(internalOrgId, competitionId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Interoperability_Valid_SetsInteroperabilityCriteria(
        string internalOrgId,
        int competitionId,
        SelectInteroperabilityCriteriaModel model,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        var result =
            (await controller.Interoperability(internalOrgId, competitionId, model)).As<RedirectToActionResult>();

        competitionsService.Verify(
            x => x.SetInteroperabilityCriteria(
                internalOrgId,
                competitionId,
                It.IsAny<IEnumerable<string>>(),
                It.IsAny<IEnumerable<string>>()),
            Times.Once());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Implementation_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.Organisation = organisation;

        competitionsService.Setup(
                x => x.GetCompetitionWithNonPriceElements(organisation.InternalIdentifier, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new AddImplementationCriteriaModel(competition)
        {
            InternalOrgId = organisation.InternalIdentifier, CompetitionId = competition.Id,
        };

        var result = (await controller.Implementation(organisation.InternalIdentifier, competition.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Implementation_InvalidModel_ReturnsView(
        string internalOrgId,
        int competitionId,
        AddImplementationCriteriaModel model,
        CompetitionNonPriceElementsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.Implementation(internalOrgId, competitionId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Implementation_Valid_SetsImplementationCriteria(
        string internalOrgId,
        int competitionId,
        AddImplementationCriteriaModel model,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        var result = (await controller.Implementation(internalOrgId, competitionId, model))
            .As<RedirectToActionResult>();

        competitionsService.Verify(x => x.SetImplementationCriteria(internalOrgId, competitionId, model.Requirements));

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ServiceLevel_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.Organisation = organisation;

        competitionsService.Setup(
                x => x.GetCompetitionWithNonPriceElements(organisation.InternalIdentifier, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new AddServiceLevelCriteriaModel(competition)
        {
            InternalOrgId = organisation.InternalIdentifier, CompetitionId = competition.Id,
        };

        var result = (await controller.ServiceLevel(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task ServiceLevel_InvalidModel_ReturnsView(
        string internalOrgId,
        int competitionId,
        AddServiceLevelCriteriaModel model,
        CompetitionNonPriceElementsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.ServiceLevel(internalOrgId, competitionId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task ServiceLevel_Valid_SetsServiceLevelCriteria(
        string internalOrgId,
        int competitionId,
        AddServiceLevelCriteriaModel model,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        var result = (await controller.ServiceLevel(internalOrgId, competitionId, model)).As<RedirectToActionResult>();

        competitionsService.Verify(
            x => x.SetServiceLevelCriteria(
                internalOrgId,
                competitionId,
                model.TimeFrom.GetValueOrDefault(),
                model.TimeUntil.GetValueOrDefault(),
                It.IsAny<IEnumerable<Iso8601DayOfWeek>>(),
                model.IncludesBankHolidays!.Value),
            Times.Once());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Features_NoExistingFeaturesCriteria_RedirectsToFeatureRequirement(
        string internalOrgId,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.NonPriceElements = new() { Features = new List<FeaturesCriteria>() };

        competitionsService.Setup(x => x.GetCompetitionWithNonPriceElements(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var result = (await controller.Features(internalOrgId, competition.Id)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.FeatureRequirement));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Features_WithExistingFeaturesCriteria_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        List<FeaturesCriteria> featuresCriteria,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.NonPriceElements = new() { Features = featuresCriteria };

        competitionsService.Setup(x => x.GetCompetitionWithNonPriceElements(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new FeaturesRequirementsModel(competition) { InternalOrgId = internalOrgId };

        var result = (await controller.Features(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt.Excluding(m => m.BackLink)
                    .Excluding(m => m.SelectedNonPriceElements));
    }

    [Theory]
    [CommonAutoData]
    public static async Task FeatureRequirement_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competitionsService.Setup(x => x.GetCompetitionWithNonPriceElements(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new FeaturesRequirementModel(competition);

        var result = (await controller.FeatureRequirement(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task FeatureRequirement_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        FeaturesRequirementModel model,
        CompetitionNonPriceElementsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.FeatureRequirement(internalOrgId, competitionId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task FeatureRequirement_ValidModel_AddsFeatureRequirement(
        string internalOrgId,
        int competitionId,
        FeaturesRequirementModel model,
        [Frozen] Mock<ICompetitionNonPriceElementsService> competitionNonPriceElementsService,
        CompetitionNonPriceElementsController controller)
    {
        _ = await controller.FeatureRequirement(internalOrgId, competitionId, model);

        competitionNonPriceElementsService.Verify(
            x => x.AddFeatureRequirement(
                internalOrgId,
                competitionId,
                model.Requirements,
                model.SelectedCompliance!.Value),
            Times.Once());
    }

    [Theory]
    [CommonAutoData]
    public static async Task FeatureRequirement_WithReturnUrl_ReturnsRedirectResult(
        string internalOrgId,
        int competitionId,
        string returnUrl,
        FeaturesRequirementModel model,
        CompetitionNonPriceElementsController controller)
    {
        var result = (await controller.FeatureRequirement(internalOrgId, competitionId, model, returnUrl))
            .As<RedirectResult>();

        result.Should().NotBeNull();
        result.Url.Should().Be(returnUrl);
    }

    [Theory]
    [CommonAutoData]
    public static async Task FeatureRequirement_WithoutReturnUrl_ReturnsRedirectToActionResult(
        string internalOrgId,
        int competitionId,
        FeaturesRequirementModel model,
        CompetitionNonPriceElementsController controller)
    {
        var result = (await controller.FeatureRequirement(internalOrgId, competitionId, model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Features));
    }

    [Theory]
    [CommonAutoData]
    public static async Task EditFeatureRequirement_InvalidRequirementId_ReturnsRedirect(
        string internalOrgId,
        Competition competition,
        int requirementId,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.NonPriceElements = new();

        competitionsService.Setup(x => x.GetCompetitionWithNonPriceElements(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var result = (await controller.EditFeatureRequirement(internalOrgId, competition.Id, requirementId))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonAutoData]
    public static async Task EditFeatureRequirement_ValidRequirementId_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        List<FeaturesCriteria> featuresCriteria,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.NonPriceElements = new() { Features = featuresCriteria };

        competitionsService.Setup(x => x.GetCompetitionWithNonPriceElements(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var requirement = featuresCriteria.First();

        var expectedModel = new FeaturesRequirementModel(competition, requirement);

        var result = (await controller.EditFeatureRequirement(internalOrgId, competition.Id, requirement.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task EditFeatureRequirement_InvalidModel_AddsFeatureRequirement(
        string internalOrgId,
        int competitionId,
        int requirementId,
        FeaturesRequirementModel model,
        CompetitionNonPriceElementsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.EditFeatureRequirement(internalOrgId, competitionId, requirementId, model))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task EditFeatureRequirement_ValidModel_AddsFeatureRequirement(
        string internalOrgId,
        int competitionId,
        int requirementId,
        FeaturesRequirementModel model,
        [Frozen] Mock<ICompetitionNonPriceElementsService> competitionNonPriceElementsService,
        CompetitionNonPriceElementsController controller)
    {
        _ = await controller.EditFeatureRequirement(internalOrgId, competitionId, requirementId, model);

        competitionNonPriceElementsService.Verify(
            x => x.EditFeatureRequirement(
                internalOrgId,
                competitionId,
                requirementId,
                model.Requirements,
                model.SelectedCompliance!.Value),
            Times.Once());
    }

    [Theory]
    [CommonAutoData]
    public static async Task EditFeatureRequirement_WithReturnUrl_ReturnsRedirectResult(
        string internalOrgId,
        int competitionId,
        int requirementId,
        string returnUrl,
        FeaturesRequirementModel model,
        CompetitionNonPriceElementsController controller)
    {
        var result = (await controller.EditFeatureRequirement(internalOrgId, competitionId, requirementId, model, returnUrl))
            .As<RedirectResult>();

        result.Should().NotBeNull();
        result.Url.Should().Be(returnUrl);
    }

    [Theory]
    [CommonAutoData]
    public static async Task EditFeatureRequirement_WithoutReturnUrl_ReturnsRedirectToActionResult(
        string internalOrgId,
        int competitionId,
        int requirementId,
        FeaturesRequirementModel model,
        CompetitionNonPriceElementsController controller)
    {
        var result = (await controller.EditFeatureRequirement(internalOrgId, competitionId, requirementId, model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Features));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Weights_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competitionsService.Setup(x => x.GetCompetitionWithNonPriceElements(internalOrgId, competition.Id))
            .ReturnsAsync(competition);

        var expectedModel = new NonPriceElementWeightsModel(competition);

        var result = (await controller.Weights(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Weights_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        NonPriceElementWeightsModel model,
        CompetitionNonPriceElementsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.Weights(internalOrgId, competitionId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [CommonAutoData]
    public static async Task Weights_Valid_SetsNonPriceWeights(
        string internalOrgId,
        int competitionId,
        NonPriceElementWeightsModel model,
        [Frozen] Mock<ICompetitionsService> competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        var result = (await controller.Weights(internalOrgId, competitionId, model)).As<RedirectToActionResult>();

        competitionsService.Verify(
            x => x.SetNonPriceWeights(
                internalOrgId,
                competitionId,
                model.Implementation.GetValueOrDefault(),
                model.Interoperability.GetValueOrDefault(),
                model.ServiceLevel.GetValueOrDefault(),
                model.Features.GetValueOrDefault()),
            Times.Once());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionTaskListController.Index));
        result.ControllerName.Should().Be(typeof(CompetitionTaskListController).ControllerName());
    }

    [Theory]
    [CommonAutoData]
    public static void Delete_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        NonPriceElement nonPriceElement,
        CompetitionNonPriceElementsController controller)
    {
        var expectedModel = new DeleteNonPriceElementModel(nonPriceElement);

        var result = controller.Delete(internalOrgId, competitionId, nonPriceElement).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [CommonAutoData]
    public static async Task Delete_Redirects(
        string internalOrgId,
        int competitionId,
        NonPriceElement nonPriceElement,
        DeleteNonPriceElementModel model,
        CompetitionNonPriceElementsController controller)
    {
        var result = (await controller.Delete(internalOrgId, competitionId, nonPriceElement, model))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonAutoData]
    public static void GetRedirect_WithReturnUrl_ReturnsRedirect(
        string internalOrgId,
        int competitionId,
        string returnUrl,
        CompetitionNonPriceElementsController controller)
    {
        var result = controller.GetRedirect(internalOrgId, competitionId, returnUrl, null).As<RedirectResult>();

        result.Should().NotBeNull();
        result.Url.Should().Be(returnUrl);
    }

    [Theory]
    [CommonAutoData]
    public static void GetRedirect_WithNoSelectedNonPriceElements_RedirectsToIndex(
        string internalOrgId,
        int competitionId,
        CompetitionNonPriceElementsController controller)
    {
        const string returnUrl = null;
        const string selectedNonPriceElements = null;

        var result = controller.GetRedirect(internalOrgId, competitionId, returnUrl, selectedNonPriceElements)
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [CommonMemberAutoData(nameof(GetRedirectNonPriceElementTestData))]
    public static void GetRedirect_WithSelectedNonPriceElements_RedirectsToNonPriceElement(
        NonPriceElement[] nonPriceElements,
        NonPriceElement expectedNonPriceElementAction,
        string internalOrgId,
        int competitionId,
        CompetitionNonPriceElementsController controller)
    {
        const string returnUrl = null;

        var selectedNonPriceElements = string.Join(',', nonPriceElements);

        var result = controller.GetRedirect(internalOrgId, competitionId, returnUrl, selectedNonPriceElements).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(expectedNonPriceElementAction.ToString());
    }
}
