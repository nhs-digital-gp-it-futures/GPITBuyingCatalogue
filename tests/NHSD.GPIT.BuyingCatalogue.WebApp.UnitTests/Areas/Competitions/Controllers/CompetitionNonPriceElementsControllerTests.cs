using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels.FeaturesModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Controllers;

public static class CompetitionNonPriceElementsControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionNonPriceElementsController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        List<Integration> integrations,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IIntegrationsService integrationsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.Organisation = organisation;

        competitionsService.GetCompetitionWithNonPriceElements(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);

        integrationsService.GetIntegrations().Returns(integrations);

        var expectedModel = new NonPriceElementsModel(competition, integrations);

        var result = (await controller.Index(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Interoperability_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        List<Integration> integrations,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IIntegrationsService integrationsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.Organisation = organisation;

        competitionsService.GetCompetitionWithNonPriceElements(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);

        integrationsService.GetIntegrationsWithTypes().Returns(integrations);

        var expectedModel = new SelectInteroperabilityCriteriaModel(competition, integrations)
        {
            InternalOrgId = organisation.InternalIdentifier, CompetitionId = competition.Id,
        };

        var result =
            (await controller.Interoperability(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
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
    [MockAutoData]
    public static async Task Interoperability_Valid_SetsInteroperabilityCriteria(
        string internalOrgId,
        int competitionId,
        SelectInteroperabilityCriteriaModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        var result =
            (await controller.Interoperability(internalOrgId, competitionId, model)).As<RedirectToActionResult>();

        await competitionsService.Received()
            .SetInteroperabilityCriteria(
                internalOrgId,
                competitionId,
                Arg.Any<IEnumerable<int>>());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task Implementation_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.Organisation = organisation;

        competitionsService.GetCompetitionWithNonPriceElements(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);

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
    [MockAutoData]
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
    [MockAutoData]
    public static async Task Implementation_Valid_SetsImplementationCriteria(
        string internalOrgId,
        int competitionId,
        AddImplementationCriteriaModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        var result = (await controller.Implementation(internalOrgId, competitionId, model))
            .As<RedirectToActionResult>();

        await competitionsService.Received()
            .SetImplementationCriteria(internalOrgId, competitionId, model.Requirements);

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task ServiceLevel_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.Organisation = organisation;

        competitionsService.GetCompetitionWithNonPriceElements(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);

        var expectedModel = new AddServiceLevelCriteriaModel(competition)
        {
            InternalOrgId = organisation.InternalIdentifier, CompetitionId = competition.Id,
        };

        var result = (await controller.ServiceLevel(organisation.InternalIdentifier, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
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
    [MockAutoData]
    public static async Task ServiceLevel_Valid_SetsServiceLevelCriteria(
        string internalOrgId,
        int competitionId,
        AddServiceLevelCriteriaModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        var result = (await controller.ServiceLevel(internalOrgId, competitionId, model)).As<RedirectToActionResult>();

        await competitionsService.Received()
            .SetServiceLevelCriteria(
                internalOrgId,
                competitionId,
                model.TimeFrom.GetValueOrDefault(),
                model.TimeUntil.GetValueOrDefault(),
                Arg.Any<IEnumerable<Iso8601DayOfWeek>>(),
                model.IncludesBankHolidays!.Value);

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task Feature_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new FeatureModel(competition);

        var result = (await controller.Feature(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task Feature_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        FeatureModel model,
        CompetitionNonPriceElementsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.Feature(internalOrgId, competitionId, model)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task Feature_ValidModel_AddsFeatureRequirement(
        string internalOrgId,
        int competitionId,
        FeatureModel model,
        [Frozen] ICompetitionNonPriceElementsService competitionNonPriceElementsService,
        CompetitionNonPriceElementsController controller)
    {
        _ = await controller.Feature(internalOrgId, competitionId, model);

        await competitionNonPriceElementsService.Received()
            .AddFeatureRequirement(
                internalOrgId,
                competitionId,
                model.Requirements,
                model.SelectedCompliance!.Value);
    }

    [Theory]
    [MockAutoData]
    public static async Task FeatureRequirement_WithoutReturnUrl_ReturnsRedirectToActionResult(
        string internalOrgId,
        int competitionId,
        FeatureModel model,
        CompetitionNonPriceElementsController controller)
    {
        var result = (await controller.Feature(internalOrgId, competitionId, model))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task GetEditFeature_InvalidRequirementId_ReturnsRedirect(
        string internalOrgId,
        Competition competition,
        int requirementId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.NonPriceElements = new();

        competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competition.Id).Returns(competition);

        var result = (await controller.EditFeature(internalOrgId, competition.Id, requirementId))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task GetEditFeature_ValidRequirementId_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        List<FeaturesCriteria> featuresCriteria,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.NonPriceElements = new() { Features = featuresCriteria };

        competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competition.Id).Returns(competition);

        var requirement = featuresCriteria.First();

        var expectedModel = new FeatureModel(competition, requirement)
        {
            InternalOrgId = internalOrgId,
        };

        var result = (await controller.EditFeature(internalOrgId, competition.Id, requirement.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.ViewName.Should().Be("Feature");
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task EditFeature_InvalidModel_AddsFeatureRequirement(
        string internalOrgId,
        int competitionId,
        int requirementId,
        FeatureModel model,
        CompetitionNonPriceElementsController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.EditFeature(internalOrgId, competitionId, requirementId, model))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task EditFeatureRequirement_ValidModel_AddsFeatureRequirement(
        string internalOrgId,
        int competitionId,
        int requirementId,
        FeatureModel model,
        [Frozen] ICompetitionNonPriceElementsService competitionNonPriceElementsService,
        CompetitionNonPriceElementsController controller)
    {
        _ = await controller.EditFeature(internalOrgId, competitionId, requirementId, model);

        await competitionNonPriceElementsService.Received()
            .EditFeatureRequirement(
                internalOrgId,
                competitionId,
                requirementId,
                model.Requirements,
                model.SelectedCompliance!.Value);
    }

    [Theory]
    [MockAutoData]
    public static async Task EditFeature_WithoutReturnUrl_ReturnsRedirectToActionResult(
        string internalOrgId,
        int competitionId,
        int requirementId,
        FeatureModel model,
        CompetitionNonPriceElementsController controller)
    {
        var result = (await controller.EditFeature(internalOrgId, competitionId, requirementId, model))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));
    }

    [Theory]
    [MockAutoData]
    public static async Task Get_DeleteFeature_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Organisation organisation,
        FeaturesCriteria firstCriteria,
        FeaturesCriteria secondCriteria,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.Organisation = organisation;
        competition.NonPriceElements = new()
        {
            Features = new List<FeaturesCriteria> { firstCriteria, secondCriteria, },
        };

        competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new DeleteNonPriceElementModel(
            NonPriceElement.Features,
            competition,
            featureId: firstCriteria.Id);

        var result = (await controller.DeleteFeature(internalOrgId, competition.Id, firstCriteria.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.ViewName.Should().Be("Delete");
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(x => x.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task PostDeleteFeature_MoreThanOneFeature_DeletesSingleFeature(
        FeaturesCriteria firstCriteria,
        FeaturesCriteria secondCriteria,
        string internalOrgId,
        Competition competition,
        DeleteNonPriceElementModel model,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] ICompetitionNonPriceElementsService competitionNonPriceElementsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.NonPriceElements = new()
        {
            Features = new List<FeaturesCriteria> { firstCriteria, secondCriteria, },
        };

        competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competition.Id).Returns(competition);

        var result = (await controller.DeleteFeature(
                internalOrgId,
                competition.Id,
                firstCriteria.Id,
                model))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));

        await competitionNonPriceElementsService.Received()
            .DeleteFeatureRequirement(
                internalOrgId,
                competition.Id,
                firstCriteria.Id);
    }

    [Theory]
    [MockAutoData]
    public static async Task PostDeleteFeature_WithReturn_MoreThanOneFeature_DeletesSingleFeature(
        string returnUrl,
        FeaturesCriteria firstCriteria,
        FeaturesCriteria secondCriteria,
        string internalOrgId,
        Competition competition,
        DeleteNonPriceElementModel model,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] ICompetitionNonPriceElementsService competitionNonPriceElementsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.NonPriceElements = new()
        {
            Features = new List<FeaturesCriteria> { firstCriteria, secondCriteria, },
        };

        competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competition.Id).Returns(competition);

        var result = (await controller.DeleteFeature(
                internalOrgId,
                competition.Id,
                firstCriteria.Id,
                model,
                returnUrl))
            .As<RedirectResult>();

        result.Should().NotBeNull();
        result.Url.Should().Be(returnUrl);

        await competitionNonPriceElementsService.Received()
            .DeleteFeatureRequirement(
                internalOrgId,
                competition.Id,
                firstCriteria.Id);
    }

    [Theory]
    [MockAutoData]
    public static async Task PostDeleteFeature_OneFeature_DeletesFeatureNonPriceElement(
        FeaturesCriteria firstCriteria,
        string internalOrgId,
        Competition competition,
        DeleteNonPriceElementModel model,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] ICompetitionNonPriceElementsService competitionNonPriceElementsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.NonPriceElements = new()
        {
            Features = new List<FeaturesCriteria> { firstCriteria },
        };

        competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competition.Id).Returns(competition);

        var result = (await controller.DeleteFeature(
                internalOrgId,
                competition.Id,
                firstCriteria.Id,
                model))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Index));

        await competitionNonPriceElementsService.Received()
            .DeleteNonPriceElement(
                internalOrgId,
                competition.Id,
                NonPriceElement.Features);
    }

    [Theory]
    [MockAutoData]
    public static async Task PostDeleteFeature_WithReturn_OneFeature_DeletesFeatureNonPriceElement(
        string returnUrl,
        FeaturesCriteria firstCriteria,
        string internalOrgId,
        Competition competition,
        DeleteNonPriceElementModel model,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] ICompetitionNonPriceElementsService competitionNonPriceElementsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.NonPriceElements = new()
        {
            Features = new List<FeaturesCriteria> { firstCriteria },
        };

        competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competition.Id).Returns(competition);

        var result = (await controller.DeleteFeature(
                internalOrgId,
                competition.Id,
                firstCriteria.Id,
                model,
                returnUrl))
            .As<RedirectResult>();

        result.Should().NotBeNull();
        result.Url.Should().Be(returnUrl);

        await competitionNonPriceElementsService.Received()
            .DeleteNonPriceElement(
                internalOrgId,
                competition.Id,
                NonPriceElement.Features);
    }

    [Theory]
    [MockAutoData]
    public static async Task Weights_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new NonPriceElementWeightsModel(competition);

        var result = (await controller.Weights(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
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
    [MockAutoData]
    public static async Task Weights_Valid_SetsNonPriceWeights(
        string internalOrgId,
        int competitionId,
        NonPriceElementWeightsModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        var result = (await controller.Weights(internalOrgId, competitionId, model)).As<RedirectToActionResult>();

        await competitionsService.Received()
            .SetNonPriceWeights(
                internalOrgId,
                competitionId,
                model.Implementation.GetValueOrDefault(),
                model.Interoperability.GetValueOrDefault(),
                model.ServiceLevel.GetValueOrDefault(),
                model.Features.GetValueOrDefault());

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(CompetitionTaskListController.Index));
        result.ControllerName.Should().Be(typeof(CompetitionTaskListController).ControllerName());
    }

    [Theory]
    [MockInlineAutoData(NonPriceElement.Implementation)]
    [MockInlineAutoData(NonPriceElement.Features)]
    [MockInlineAutoData(NonPriceElement.ServiceLevel)]
    public static async Task GetDelete_ReturnsViewWithModel(
        NonPriceElement nonPriceElement,
        Organisation organisation,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.Organisation = organisation;
        competitionsService.GetCompetitionWithNonPriceElements(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);

        var expectedModel = new DeleteNonPriceElementModel(nonPriceElement, competition, new List<Integration>());

        var result = (await controller.Delete(organisation.InternalIdentifier, competition.Id, nonPriceElement)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task GetDelete_Interoperability_ReturnsViewWithModel(
        Organisation organisation,
        Competition competition,
        IEnumerable<Integration> integrations,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IIntegrationsService integrationsService,
        CompetitionNonPriceElementsController controller)
    {
        competition.Organisation = organisation;
        competitionsService.GetCompetitionWithNonPriceElements(organisation.InternalIdentifier, competition.Id)
            .Returns(competition);

        integrationsService.GetIntegrationsWithTypes().Returns(integrations);

        var expectedModel = new DeleteNonPriceElementModel(NonPriceElement.Interoperability, competition, integrations);

        var result = (await controller.Delete(organisation.InternalIdentifier, competition.Id, NonPriceElement.Interoperability)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task PostDelete_Redirects(
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
}
