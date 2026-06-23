using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MoreLinq.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Controllers;

public static class CompetitionHubControllerTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CompetitionScoringController).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task Index_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competition.CompetitionSolutions = [];

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new PricingDashboardModel(competition);

        var result = (await controller.Index(internalOrgId, competition.Id)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.InternalOrgId));
    }

    [Theory]
    [MockAutoData]
    public static async Task Hub_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        Solution solution,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;

        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competition.Id).Returns(competition);
        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, solution.CatalogueItemId).Returns(competitionSolution);

        var expectedModel = new CompetitionSolutionHubModel(internalOrgId, competitionSolution, competition.FlattenedRecipients, competition.ContractLength);

        var result = (await controller.Hub(internalOrgId, competition.Id, solution.CatalogueItemId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt.Excluding(m => m.BackLink).Excluding(m => m.AssociatedServicesUrl));
    }

    [Theory]
    [MockAutoData]
    public static async Task Hub_InvalidSolutionId_ReturnsViewWithModel(
        string internalOrgId,
        CatalogueItemId solutionId,
        Competition competition,
        CompetitionSolution competitionSolution,
        Solution solution,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competition.Id).Returns(competition);
        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, solution.CatalogueItemId).Returns(competitionSolution);

        var result = (await controller.Hub(internalOrgId, competition.Id, solutionId)).As<BadRequestResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task Hub_NoAssociatedServicesRemaining_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        Solution solution,
        List<AssociatedService> associatedServices,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competitionSolution.Services = associatedServices
            .Select(x =>
                new CompetitionAssociatedService(competition.Id, x.CatalogueItemId)
                {
                    CatalogueItem = x.CatalogueItem,
                })
            .Cast<CompetitionCatalogueItem>()
            .ToList();
        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competition.Id).Returns(competition);
        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, solution.CatalogueItemId).Returns(competitionSolution);

        var expectedModel = new CompetitionSolutionHubModel(internalOrgId, competitionSolution, competition.FlattenedRecipients, competition.ContractLength);

        var result = (await controller.Hub(internalOrgId, competition.Id, solution.CatalogueItemId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt.Excluding(m => m.BackLink).Excluding(m => m.AssociatedServicesUrl));
    }

    [Theory]
    [MockAutoData]
    public static async Task Hub_AdditionalServiceWithAssociatedServicesRemaining_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        Solution solution,
        CompetitionCatalogueItemPrice servicePrice,
        AdditionalService service,
        CompetitionAdditionalService additionalService,
        List<AssociatedService> associatedServices,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;

        additionalService.CatalogueItem = service.CatalogueItem;
        additionalService.CatalogueItemId = service.CatalogueItemId;
        additionalService.Price = servicePrice;
        additionalService.AssociatedServices = associatedServices
            .Select(x => new CompetitionAssociatedService(competition.Id, x.CatalogueItemId)
                {
                    CatalogueItem = x.CatalogueItem,
                });

        competitionSolution.Services = [additionalService];

        competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competition.Id).Returns(competition);
        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, solution.CatalogueItemId).Returns(competitionSolution);

        var expectedModel = new CompetitionSolutionHubModel(internalOrgId, competitionSolution, competition.FlattenedRecipients, competition.ContractLength);
        var result = (await controller.Hub(internalOrgId, competition.Id, solution.CatalogueItemId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt.Excluding(m => m.BackLink).Excluding(m => m.AssociatedServicesUrl));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectPrice_NullServiceId_ReturnsExpectedPriceId(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice competitionPrice,
        CatalogueItem catalogueItem,
        CataloguePrice cataloguePrice,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IListPriceService listPriceService,
        CompetitionHubController controller)
    {
        competitionSolution.CatalogueItemId = catalogueItem.Id;
        competitionSolution.Price = competitionPrice;

        catalogueItem.CataloguePrices = [cataloguePrice];
        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, competitionSolution.CatalogueItemId).Returns(competitionSolution);

        listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItem.Id).Returns(catalogueItem);

        var result = (await controller.SelectPrice(internalOrgId, competition.Id, catalogueItem.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();

        var model = result.Model.As<SelectPriceModel>();

        model.SelectedPriceId.Should().Be(competitionPrice.CataloguePriceId);
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectPrice_WithServiceId_ReturnsExpectedPriceId(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CompetitionAdditionalService solutionService,
        CompetitionCatalogueItemPrice competitionPrice,
        CompetitionCatalogueItemPrice servicePrice,
        Solution solution,
        AssociatedService service,
        CataloguePrice cataloguePrice,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IListPriceService listPriceService,
        CompetitionHubController controller)
    {
        solutionService.CatalogueItemId = service.CatalogueItemId;
        solutionService.Price = servicePrice;

        competitionSolution.Price = competitionPrice;
        competitionSolution.Services = [solutionService];

        competitionSolution.CatalogueItemId = solution.CatalogueItemId;

        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItem.CataloguePrices = [cataloguePrice];
        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, competitionSolution.CatalogueItemId).Returns(competitionSolution);

        listPriceService.GetCatalogueItemWithPublishedListPrices(service.CatalogueItemId).Returns(service.CatalogueItem);

        var result = (await controller.SelectPrice(internalOrgId, competition.Id, competitionSolution.CatalogueItemId, null, service.CatalogueItemId, null))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();

        var model = result.Model.As<SelectPriceModel>();

        model.SelectedPriceId.Should().Be(servicePrice.CataloguePriceId);
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectPrice_WithAssociatedServiceId_ReturnsExpectedPriceId(
        string internalOrgId,
        Competition competition,
        Solution solution,
        CompetitionSolution competitionSolution,
        CompetitionAdditionalService competitionAdditionalService,
        AdditionalService additionalService,
        CompetitionCatalogueItemPrice competitionPrice,
        CompetitionCatalogueItemPrice servicePrice,
        AssociatedService associatedService,
        CataloguePrice cataloguePrice,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IListPriceService listPriceService,
        CompetitionHubController controller)
    {
        competitionAdditionalService.CatalogueItemId = additionalService.CatalogueItemId;
        competitionAdditionalService.CatalogueItem = additionalService.CatalogueItem;
        competitionAdditionalService.AssociatedServices = [
            new CompetitionAssociatedService(competition.Id, associatedService.CatalogueItemId)
            {
                CatalogueItem = associatedService.CatalogueItem,
                Price = servicePrice,
            }
        ];

        competitionSolution.Services = [competitionAdditionalService];

        competitionSolution.Price = competitionPrice;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItem.CataloguePrices = [cataloguePrice];

        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, competitionSolution.CatalogueItemId).Returns(competitionSolution);
        listPriceService.GetCatalogueItemWithPublishedListPrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

        var result = (await controller.SelectPrice(
            internalOrgId,
            competition.Id,
            competitionSolution.CatalogueItemId,
            additionalService.CatalogueItemId,
            associatedService.CatalogueItemId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();

        var model = result.Model.As<SelectPriceModel>();
        model.SelectedPriceId.Should().Be(servicePrice.CataloguePriceId);
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectPrice_WithSelectedPriceId_ReturnsExpectedPriceId(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice competitionPrice,
        CatalogueItem catalogueItem,
        CataloguePrice cataloguePrice,
        int selectedPriceId,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IListPriceService listPriceService,
        CompetitionHubController controller)
    {
        competitionSolution.CatalogueItemId = catalogueItem.Id;
        competitionSolution.Price = competitionPrice;

        catalogueItem.CataloguePrices = [cataloguePrice];
        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, competitionSolution.CatalogueItemId).Returns(competitionSolution);

        listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItem.Id).Returns(catalogueItem);

        var result = (await controller.SelectPrice(internalOrgId, competition.Id, catalogueItem.Id, selectedPriceId: selectedPriceId))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().NotBeNull();

        var model = result.Model.As<SelectPriceModel>();

        model.SelectedPriceId.Should().Be(selectedPriceId);
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectPrice_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice competitionPrice,
        CatalogueItem catalogueItem,
        CataloguePrice cataloguePrice,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IListPriceService listPriceService,
        CompetitionHubController controller)
    {
        competitionSolution.CatalogueItemId = catalogueItem.Id;
        competitionSolution.Price = competitionPrice;

        catalogueItem.CataloguePrices = [cataloguePrice];
        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, competitionSolution.CatalogueItemId).Returns(competitionSolution);

        listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItem.Id).Returns(catalogueItem);

        var expectedModel = new SelectPriceModel(catalogueItem);

        var result = (await controller.SelectPrice(internalOrgId, competition.Id, catalogueItem.Id))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.SelectedPriceId));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectPrice_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        SelectPriceModel model,
        Solution solution,
        List<CataloguePrice> prices,
        [Frozen] IListPriceService listPriceService,
        CompetitionHubController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        solution.CatalogueItem.CataloguePrices = prices;

        listPriceService.GetCatalogueItemWithPublishedListPrices(solution.CatalogueItemId).Returns(solution.CatalogueItem);

        var result = (await controller.SelectPrice(internalOrgId, competitionId, solution.CatalogueItemId, model))
            .As<ViewResult>();

        result.Should().NotBeNull();

        var responseModel = result.Model.As<SelectPriceModel>();
        responseModel.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.Prices));
        responseModel.Prices.Should().BeEquivalentTo(prices);
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectPrice_InvalidModelWithServiceId_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        SelectPriceModel model,
        Solution solution,
        AdditionalService additionalService,
        List<CataloguePrice> prices,
        [Frozen] IListPriceService listPriceService,
        CompetitionHubController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        additionalService.CatalogueItem.CataloguePrices = prices;

        listPriceService.GetCatalogueItemWithPublishedListPrices(additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

        var result = (await controller.SelectPrice(internalOrgId, competitionId, solution.CatalogueItemId, model, additionalService.CatalogueItemId))
            .As<ViewResult>();

        result.Should().NotBeNull();

        var responseModel = result.Model.As<SelectPriceModel>();
        responseModel.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.Prices));
        responseModel.Prices.Should().BeEquivalentTo(prices);
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectPrice_Valid_Redirects(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        SelectPriceModel model,
        CompetitionHubController controller)
    {
        var result = (await controller.SelectPrice(internalOrgId, competitionId, solutionId, model))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(competitionId), competitionId },
                    { nameof(solutionId), solutionId },
                    { "priceId", model.SelectedPriceId.GetValueOrDefault() },
                    { "serviceId", null },
                });
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectPrice_ValidWithServiceId_Redirects(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        SelectPriceModel model,
        CompetitionHubController controller)
    {
        var result = (await controller.SelectPrice(internalOrgId, competitionId, solutionId, model, serviceId))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { nameof(competitionId), competitionId },
                    { nameof(solutionId), solutionId },
                    { "priceId", model.SelectedPriceId.GetValueOrDefault() },
                    { nameof(serviceId), serviceId },
                });
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmPrice_NullServiceId_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice competitionPrice,
        CatalogueItem catalogueItem,
        CataloguePrice cataloguePrice,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IListPriceService listPriceService,
        CompetitionHubController controller)
    {
        competitionSolution.CatalogueItemId = catalogueItem.Id;
        competitionSolution.Price = competitionPrice;

        catalogueItem.CataloguePrices = [cataloguePrice];
        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, competitionSolution.CatalogueItemId).Returns(competitionSolution);

        listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItem.Id).Returns(catalogueItem);

        var expectedModel = new ConfirmPriceModel(catalogueItem, cataloguePrice, competitionPrice);

        var result = (await controller.ConfirmPrice(
            internalOrgId,
            competition.Id,
            catalogueItem.Id,
            cataloguePrice.CataloguePriceId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmPrice_WithServiceId_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CompetitionAdditionalService solutionService,
        CompetitionCatalogueItemPrice competitionPrice,
        CompetitionCatalogueItemPrice servicePrice,
        Solution solution,
        AssociatedService service,
        CataloguePrice cataloguePrice,
        CataloguePrice additionalServicePrice,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IListPriceService listPriceService,
        CompetitionHubController controller)
    {
        solutionService.CatalogueItemId = service.CatalogueItemId;
        solutionService.Price = servicePrice;

        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.Price = competitionPrice;
        competitionSolution.Services = [solutionService];

        service.CatalogueItem.CataloguePrices = [additionalServicePrice];
        solution.CatalogueItem.CataloguePrices = [cataloguePrice];
        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, competitionSolution.CatalogueItemId).Returns(competitionSolution);

        listPriceService.GetCatalogueItemWithPublishedListPrices(service.CatalogueItemId).Returns(service.CatalogueItem);

        var expectedModel = new ConfirmPriceModel(service.CatalogueItem, additionalServicePrice, competitionPrice);

        var result = (await controller.ConfirmPrice(
            internalOrgId,
            competition.Id,
            competitionSolution.CatalogueItemId,
            additionalServicePrice.CataloguePriceId,
            (CatalogueItemId?)null,
            service.CatalogueItemId,
            (RoutingSource?)null)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmPrice_WithAdditionalServiceId_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Solution solution,
        CompetitionSolution competitionSolution,
        CompetitionAdditionalService competitionAdditionalService,
        AdditionalService additionalService,
        CompetitionCatalogueItemPrice competitionPrice,
        CompetitionCatalogueItemPrice servicePrice,
        AssociatedService associatedService,
        CataloguePrice cataloguePrice,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IListPriceService listPriceService,
        CompetitionHubController controller)
    {
        associatedService.CatalogueItem.CataloguePrices = [cataloguePrice];
        competitionAdditionalService.CatalogueItemId = additionalService.CatalogueItemId;
        competitionAdditionalService.CatalogueItem = additionalService.CatalogueItem;
        competitionAdditionalService.AssociatedServices = [
            new CompetitionAssociatedService(competition.Id, associatedService.CatalogueItemId)
            {
                CatalogueItem = associatedService.CatalogueItem,
                Price = servicePrice,
            }
        ];

        competitionSolution.Services = [competitionAdditionalService];

        competitionSolution.Price = competitionPrice;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItem.CataloguePrices = [cataloguePrice];

        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, competitionSolution.CatalogueItemId)
            .Returns(competitionSolution);
        listPriceService.GetCatalogueItemWithPublishedListPrices(associatedService.CatalogueItemId)
            .Returns(competitionAdditionalService.AssociatedServices.FirstOrDefault().CatalogueItem);

        var expectedModel = new ConfirmPriceModel(
            associatedService.CatalogueItem,
            associatedService.CatalogueItem.CataloguePrices.FirstOrDefault(),
            servicePrice);

        var result = (await controller.ConfirmPrice(
            internalOrgId,
            competition.Id,
            competitionSolution.CatalogueItemId,
            cataloguePrice.CataloguePriceId,
            competitionAdditionalService.CatalogueItemId,
            associatedService.CatalogueItemId,
            (RoutingSource?)null)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmPrice_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        int priceId,
        ConfirmPriceModel model,
        CompetitionHubController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.ConfirmPrice(internalOrgId, competitionId, solutionId, priceId, model))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmPrice_NullServiceId_SetsSolutionPrice(
        string internalOrgId,
        int competitionId,
        CatalogueItem catalogueItem,
        List<CataloguePrice> prices,
        ConfirmPriceModel model,
        [Frozen] IListPriceService listPriceService,
        [Frozen] ICompetitionsPriceService competitionsPriceService,
        CompetitionHubController controller)
    {
        var price = prices.First();

        catalogueItem.CataloguePrices = prices;

        listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItem.Id).Returns(catalogueItem);

        _ = await controller.ConfirmPrice(
            internalOrgId,
            competitionId,
            catalogueItem.Id,
            price.CataloguePriceId,
            model);

        await competitionsPriceService.Received().SetSolutionPrice(internalOrgId, competitionId, catalogueItem.Id, price, Arg.Any<List<PricingTierDto>>());
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmPrice_WithServiceId_SetsSolutionPrice(
        string internalOrgId,
        int competitionId,
        CatalogueItem catalogueItem,
        CatalogueItem additionalService,
        List<CataloguePrice> prices,
        ConfirmPriceModel model,
        [Frozen] IListPriceService listPriceService,
        [Frozen] ICompetitionsPriceService competitionsPriceService,
        CompetitionHubController controller)
    {
        var price = prices.First();

        additionalService.CataloguePrices = prices;

        listPriceService.GetCatalogueItemWithPublishedListPrices(additionalService.Id).Returns(additionalService);

        _ = await controller.ConfirmPrice(
            internalOrgId,
            competitionId,
            catalogueItem.Id,
            price.CataloguePriceId,
            model,
            null,
            additionalService.Id);

        await competitionsPriceService.Received().SetServicePrice(internalOrgId, competitionId, catalogueItem.Id, additionalService.Id, price, Arg.Any<List<PricingTierDto>>());
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmPrice_WithAdditionalServiceId_SetsSolutionPrice(
        string internalOrgId,
        int competitionId,
        CatalogueItem solutionItem,
        CatalogueItem additionalService,
        CatalogueItem associatedService,
        List<CataloguePrice> prices,
        ConfirmPriceModel model,
        [Frozen] IListPriceService listPriceService,
        [Frozen] ICompetitionsPriceService competitionsPriceService,
        CompetitionHubController controller)
    {
        var price = prices.First();

        associatedService.CataloguePrices = prices;

        listPriceService.GetCatalogueItemWithPublishedListPrices(associatedService.Id).Returns(associatedService);

        _ = await controller.ConfirmPrice(
            internalOrgId,
            competitionId,
            solutionItem.Id,
            price.CataloguePriceId,
            model,
            additionalService.Id,
            associatedService.Id);

        await competitionsPriceService.Received()
            .SetAdditionalServiceAssociatedServicePrice(internalOrgId, competitionId, solutionItem.Id, additionalService.Id, associatedService.Id, price, Arg.Any<List<PricingTierDto>>());
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmPrice_Redirects(
        string internalOrgId,
        int competitionId,
        CatalogueItem catalogueItem,
        List<CataloguePrice> prices,
        ConfirmPriceModel model,
        [Frozen] IListPriceService listPriceService,
        CompetitionHubController controller)
    {
        var price = prices.First();

        catalogueItem.CataloguePrices = prices;

        listPriceService.GetCatalogueItemWithPublishedListPrices(catalogueItem.Id).Returns(catalogueItem);

        var result = (await controller.ConfirmPrice(
            internalOrgId,
            competitionId,
            catalogueItem.Id,
            price.CataloguePriceId,
            model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Hub));
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmPrice_WithAdditionalService_Redirects(
        string internalOrgId,
        int competitionId,
        CatalogueItem solutionItem,
        CatalogueItem additionalService,
        CatalogueItem associatedService,
        List<CataloguePrice> prices,
        ConfirmPriceModel model,
        [Frozen] IListPriceService listPriceService,
        CompetitionHubController controller)
    {
        var price = prices.First();

        associatedService.CataloguePrices = prices;

        listPriceService.GetCatalogueItemWithPublishedListPrices(associatedService.Id).Returns(associatedService);

        var result = (await controller.ConfirmPrice(
            internalOrgId,
            competitionId,
            solutionItem.Id,
            price.CataloguePriceId,
            model,
            additionalService.Id,
            associatedService.Id)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.HubAdditionalServiceAssociatedServices));
    }

    [Theory]
    [MockAutoData]
    public static async Task CompetitionSublocationHub_NullServiceId_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice competitionPrice,
        Solution solution,
        CompetitionSublocation sublocation,
        Organisation organisation,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IGpPracticeService gpPracticeService,
        [Frozen] IOdsService odsService,
        CompetitionHubController controller)
    {
        competitionPrice.ProvisioningType = ProvisioningType.Declarative;

        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competitionSolution.Price = competitionPrice;
        competition.Organisation = organisation;

        competition.CompetitionSolutions = [competitionSolution];
        competition.CompetitionSublocations = [sublocation];

        competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competition.Id).Returns(competition);
        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, solution.CatalogueItemId).Returns(competitionSolution);
        gpPracticeService.GetNumberOfPatients(Arg.Any<IEnumerable<string>>()).Returns([]);
        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                Arg.Any<string>(),
                Arg.Any<IEnumerable<string>>())
            .Returns([]);
        var recipients = competition.FlattenedRecipients.Select(recipient =>
        {
            var quantity = competitionSolution.Quantities.FirstOrDefault(quantity =>
                quantity.RecipientOdsCode == recipient.RecipientOdsCode);
            return new ServiceRecipientQuantityDto(
                recipient.ParentSublocationOdsCode,
                recipient.RecipientOdsCode,
                recipient.RecipientOrganisation.Name,
                quantity?.Quantity,
                null);
        });

        var expectedModel = new SublocationQuantityHubModel(
            competition.Organisation,
            solution.CatalogueItem)
        {
            SubLocations = CreateSublocationHelper.CreateSubLocations(recipients)
                .Select(s => new SubLocationModel(s)
                {
                    ForwardingLink = "testUrl",
                })
                .ToArray(),
        };

        var result = (await controller.CompetitionSublocationHub(internalOrgId, competition.Id, solution.CatalogueItemId))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt =>
                    opt.Excluding(m => m.BackLink)
                    .Excluding(m => m.Caption));
    }

    [Theory]
    [MockAutoData]
    public static async Task CompetitionSublocationHub_WithServiceId_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CompetitionAdditionalService solutionService,
        CompetitionCatalogueItemPrice competitionPrice,
        Solution solution,
        CompetitionSublocation sublocation,
        Organisation organisation,
        AdditionalService service,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IGpPracticeService gpPracticeService,
        [Frozen] IOdsService odsService,
        CompetitionHubController controller)
    {
        competitionPrice.ProvisioningType = ProvisioningType.Declarative;
        competition.Organisation = organisation;

        solutionService.CatalogueItem = service.CatalogueItem;
        solutionService.CatalogueItemId = service.CatalogueItemId;
        solutionService.Price = competitionPrice;

        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competitionSolution.Services = [solutionService];

        competition.CompetitionSolutions = [competitionSolution];
        competition.CompetitionSublocations = [sublocation];

        competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competition.Id).Returns(competition);
        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, solution.CatalogueItemId).Returns(competitionSolution);
        gpPracticeService.GetNumberOfPatients(Arg.Any<IEnumerable<string>>()).Returns([]);
        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                Arg.Any<string>(),
                Arg.Any<IEnumerable<string>>())
            .Returns([]);
        var recipients = competition.FlattenedRecipients.Select(recipient =>
        {
            var quantity = competitionSolution.Quantities.FirstOrDefault(quantity =>
                quantity.RecipientOdsCode == recipient.RecipientOdsCode);
            return new ServiceRecipientQuantityDto(
                recipient.ParentSublocationOdsCode,
                recipient.RecipientOdsCode,
                recipient.RecipientOrganisation.Name,
                quantity?.Quantity,
                null);
        });

        var expectedModel = new SublocationQuantityHubModel(
            competition.Organisation,
            solutionService.CatalogueItem)
        {
            SubLocations = CreateSublocationHelper.CreateSubLocations(recipients)
                .Select(s => new SubLocationModel(s)
                {
                    ForwardingLink = "testUrl",
                })
                .ToArray(),
        };

        var result = (await controller.CompetitionSublocationHub(internalOrgId, competition.Id, solution.CatalogueItemId, null, service.CatalogueItemId))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(
            expectedModel,
            opt => opt.Excluding(m => m.BackLink)
                .Excluding(m => m.Caption));
    }

    [Theory]
    [MockAutoData]
    public static async Task CompetitionSublocationHub_WithAdditionalServiceId_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        Organisation organisation,
        Solution solution,
        CompetitionSolution competitionSolution,
        AdditionalService additionalService,
        CompetitionAdditionalService competitionAdditionalService,
        AssociatedService associatedService,
        CompetitionCatalogueItemPrice competitionPrice,
        CompetitionCatalogueItemPrice servicePrice,
        CompetitionSublocation sublocation,
        CataloguePrice cataloguePrice,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IGpPracticeService gpPracticeService,
        [Frozen] IOdsService odsService,
        CompetitionHubController controller)
    {
        competitionPrice.ProvisioningType = ProvisioningType.Declarative;

        associatedService.CatalogueItem.CataloguePrices = [cataloguePrice];

        competitionAdditionalService.CatalogueItem = additionalService.CatalogueItem;
        competitionAdditionalService.CatalogueItemId = additionalService.CatalogueItemId;
        competitionAdditionalService.Price = competitionPrice;
        competitionAdditionalService.AssociatedServices = [
            new CompetitionAssociatedService(competition.Id, associatedService.CatalogueItemId)
            {
                CatalogueItem = associatedService.CatalogueItem,
                Price = servicePrice,
            }
        ];

        competitionSolution.Services = [competitionAdditionalService];
        competition.Organisation = organisation;
        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;

        competition.CompetitionSolutions = [competitionSolution];
        competition.CompetitionSublocations = [sublocation];

        competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competition.Id).Returns(competition);
        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, solution.CatalogueItemId).Returns(competitionSolution);
        gpPracticeService.GetNumberOfPatients(Arg.Any<IEnumerable<string>>()).Returns([]);
        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                Arg.Any<string>(),
                Arg.Any<IEnumerable<string>>())
            .Returns([]);

        var recipients = competition.FlattenedRecipients.Select(recipient =>
        {
            var quantity = competitionSolution.Quantities.FirstOrDefault(quantity =>
                quantity.RecipientOdsCode == recipient.RecipientOdsCode);
            return new ServiceRecipientQuantityDto(
                recipient.ParentSublocationOdsCode,
                recipient.RecipientOdsCode,
                recipient.RecipientOrganisation.Name,
                quantity?.Quantity,
                null);
        });

        var expectedModel = new SublocationQuantityHubModel(
            competition.Organisation,
            associatedService.CatalogueItem)
        {
            SubLocations = CreateSublocationHelper.CreateSubLocations(recipients)
                .Select(s => new SubLocationModel(s)
                {
                    ForwardingLink = "testUrl",
                })
                .ToArray(),
        };

        var result = (await controller.CompetitionSublocationHub(internalOrgId, competition.Id, solution.CatalogueItemId, additionalService.CatalogueItemId, associatedService.CatalogueItemId))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(
            expectedModel,
            opt => opt.Excluding(m => m.BackLink)
                .Excluding(m => m.Caption));
    }

    [Theory]
    [MockAutoData]
    public static async Task CompetitionSublocationHub_Returns_BadRequest(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CatalogueItemId serviceId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competition.Id).Returns(competition);
        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, competitionSolution.CatalogueItemId).Returns(competitionSolution);
        competitionSolution.Services = [];
        competition.CompetitionSolutions = [competitionSolution];

        var response = await controller.CompetitionSublocationHub(
            internalOrgId,
            competition.Id,
            competitionSolution.CatalogueItemId,
            null,
            serviceId);

        response.Should().NotBeNull();
        response.Should().BeOfType<BadRequestResult>();
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmQuantities_ReturnsWithViewModel_WhenServiceIdIsNull(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice competitionPrice,
        Solution solution,
        CompetitionSublocation sublocation,
        Organisation organisation,
        ServiceRecipient serviceRecipient,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IGpPracticeService gpPracticeService,
        [Frozen] IOdsService odsService,
        CompetitionHubController controller)
    {
        competitionPrice.ProvisioningType = ProvisioningType.Declarative;

        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competitionSolution.Price = competitionPrice;
        competition.Organisation = organisation;

        sublocation.SublocationRecipients.ForEach(r => r.RecipientOdsCode = serviceRecipient.OrgId);

        competition.CompetitionSolutions = [competitionSolution];
        competition.CompetitionSublocations = [sublocation];

        competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competition.Id).Returns(competition);
        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, solution.CatalogueItemId).Returns(competitionSolution);
        gpPracticeService.GetNumberOfPatients(Arg.Any<IEnumerable<string>>()).Returns([]);
        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                Arg.Any<string>(),
                Arg.Any<IEnumerable<string>>())
            .Returns([serviceRecipient]);
        var recipients = competition.FlattenedRecipients.Select(recipient =>
        {
            var quantity = competitionSolution.Quantities.FirstOrDefault(quantity =>
                quantity.RecipientOdsCode == recipient.RecipientOdsCode);
            return new ServiceRecipientQuantityDto(
                recipient.ParentSublocationOdsCode,
                recipient.RecipientOdsCode,
                recipient.RecipientOrganisation.Name,
                quantity?.Quantity,
                serviceRecipient.Location);
        });

        var expectedModel = new ConfirmQuantitiesModel(
            competitionSolution.CatalogueItem,
            competitionSolution.Price,
            recipients.ToList());

        var result = (await controller.ConfirmQuantities(
            internalOrgId,
            competition.Id,
            solution.CatalogueItemId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt.Excluding(m => m.BackLink)
                    .Excluding(m => m.ContinueLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmQuantities_ReturnsWithViewModel_WhenServiceIdIsNotNull(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CompetitionAdditionalService solutionService,
        CompetitionCatalogueItemPrice competitionPrice,
        Solution solution,
        CompetitionSublocation sublocation,
        Organisation organisation,
        ServiceRecipient serviceRecipient,
        AdditionalService service,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IGpPracticeService gpPracticeService,
        [Frozen] IOdsService odsService,
        CompetitionHubController controller)
    {
        competitionPrice.ProvisioningType = ProvisioningType.Declarative;

        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competitionSolution.Price = competitionPrice;
        solutionService.Price = competitionPrice;
        competition.Organisation = organisation;

        sublocation.SublocationRecipients.ForEach(r => r.RecipientOdsCode = serviceRecipient.OrgId);

        solutionService.CatalogueItem = service.CatalogueItem;
        solutionService.CatalogueItemId = service.CatalogueItemId;
        competitionSolution.Services = [solutionService];

        competition.CompetitionSolutions = [competitionSolution];
        competition.CompetitionSublocations = [sublocation];

        competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competition.Id).Returns(competition);
        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, solution.CatalogueItemId).Returns(competitionSolution);
        gpPracticeService.GetNumberOfPatients(Arg.Any<IEnumerable<string>>()).Returns([]);
        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                Arg.Any<string>(),
                Arg.Any<IEnumerable<string>>())
            .Returns([serviceRecipient]);
        var recipients = competition.FlattenedRecipients.Select(recipient =>
        {
            var quantity = competitionSolution.Quantities.FirstOrDefault(quantity =>
                quantity.RecipientOdsCode == recipient.RecipientOdsCode);
            return new ServiceRecipientQuantityDto(
                recipient.ParentSublocationOdsCode,
                recipient.RecipientOdsCode,
                recipient.RecipientOrganisation.Name,
                quantity?.Quantity,
                serviceRecipient.Location);
        });

        var expectedModel = new ConfirmQuantitiesModel(
            solutionService.CatalogueItem,
            solutionService.Price,
            recipients.ToList());

        var result = (await controller.ConfirmQuantities(
            internalOrgId,
            competition.Id,
            competitionSolution.CatalogueItemId,
            null,
            solutionService.CatalogueItemId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt.Excluding(m => m.BackLink)
                    .Excluding(m => m.ContinueLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmQuantities_ReturnsWithViewModel_WhenAdditionalServiceIdIsNotNull(
        string internalOrgId,
        Competition competition,
        Organisation organisation,
        CompetitionSolution competitionSolution,
        AdditionalService additionalService,
        CompetitionAdditionalService competitionAdditionalService,
        AssociatedService associatedService,
        CompetitionCatalogueItemPrice competitionPrice,
        CompetitionCatalogueItemPrice servicePrice,
        CompetitionSublocation sublocation,
        ServiceRecipient serviceRecipient,
        CataloguePrice cataloguePrice,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IGpPracticeService gpPracticeService,
        [Frozen] IOdsService odsService,
        CompetitionHubController controller)
    {
        competitionPrice.ProvisioningType = ProvisioningType.Declarative;

        associatedService.CatalogueItem.CataloguePrices = [cataloguePrice];

        competitionAdditionalService.CatalogueItem = additionalService.CatalogueItem;
        competitionAdditionalService.CatalogueItemId = additionalService.CatalogueItemId;
        competitionAdditionalService.Price = competitionPrice;
        competitionAdditionalService.AssociatedServices = [
            new CompetitionAssociatedService(competition.Id, associatedService.CatalogueItemId)
            {
                CatalogueItem = associatedService.CatalogueItem,
                Price = servicePrice,
            }
        ];

        competitionSolution.Services = [competitionAdditionalService];

        sublocation.SublocationRecipients.ForEach(r => r.RecipientOdsCode = serviceRecipient.OrgId);

        competition.Organisation = organisation;
        competition.CompetitionSolutions = [competitionSolution];
        competition.CompetitionSublocations = [sublocation];

        competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competition.Id).Returns(competition);
        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, competitionSolution.CatalogueItemId).Returns(competitionSolution);
        gpPracticeService.GetNumberOfPatients(Arg.Any<IEnumerable<string>>()).Returns([]);
        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                Arg.Any<string>(),
                Arg.Any<IEnumerable<string>>())
            .Returns([serviceRecipient]);
        var recipients = competition.FlattenedRecipients.Select(recipient =>
        {
            var quantity = competitionSolution.Quantities.FirstOrDefault(quantity =>
                quantity.RecipientOdsCode == recipient.RecipientOdsCode);
            return new ServiceRecipientQuantityDto(
                recipient.ParentSublocationOdsCode,
                recipient.RecipientOdsCode,
                recipient.RecipientOrganisation.Name,
                quantity?.Quantity,
                serviceRecipient.Location);
        });

        var expectedModel = new ConfirmQuantitiesModel(
            associatedService.CatalogueItem,
            competitionAdditionalService.AssociatedServices.FirstOrDefault()?.Price,
            [.. recipients]);

        var result = (await controller.ConfirmQuantities(
            internalOrgId,
            competition.Id,
            competitionSolution.CatalogueItemId,
            competitionAdditionalService.CatalogueItemId,
            associatedService.CatalogueItemId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(
                expectedModel,
                opt => opt.Excluding(m => m.BackLink)
                    .Excluding(m => m.ContinueLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectServiceRecipientQuantity_NullServiceId_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice competitionPrice,
        List<CompetitionItemQuantity> solutionQuantities,
        List<ServiceRecipientQuantityDto> recipientQuantities,
        Solution solution,
        string parentOdsCode,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionPrice.ProvisioningType = ProvisioningType.Declarative;

        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competitionSolution.Price = competitionPrice;
        competitionSolution.Quantities = solutionQuantities;
        recipientQuantities.ForEach(recipient => recipient.ParentSublocationOdsCode = parentOdsCode);

        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competition.Id).Returns(competition);
        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, solution.CatalogueItemId).Returns(competitionSolution);

        var expectedModel = new SelectServiceRecipientQuantityModel(
            solution.CatalogueItem,
            competitionPrice,
            recipientQuantities);

        var result =
            (await controller.SelectServiceRecipientQuantity(internalOrgId, competition.Id, parentOdsCode, solution.CatalogueItemId))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.SubLocations));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectServiceRecipientQuantity_WithServiceId_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CompetitionAdditionalService solutionService,
        CompetitionCatalogueItemPrice competitionPrice,
        List<CompetitionItemQuantity> solutionQuantities,
        List<ServiceRecipientQuantityDto> recipientQuantities,
        Solution solution,
        AdditionalService service,
        string parentOdsCode,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionPrice.ProvisioningType = ProvisioningType.Declarative;

        solutionService.CatalogueItem = service.CatalogueItem;
        solutionService.CatalogueItemId = service.CatalogueItemId;
        solutionService.Price = competitionPrice;
        solutionService.Quantities = solutionQuantities;
        recipientQuantities.ForEach(recipient => recipient.ParentSublocationOdsCode = parentOdsCode);

        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competitionSolution.Services = [solutionService];

        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competition.Id).Returns(competition);
        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, solution.CatalogueItemId).Returns(competitionSolution);

        var expectedModel = new SelectServiceRecipientQuantityModel(
            service.CatalogueItem,
            competitionPrice,
            recipientQuantities);

        var result =
            (await controller.SelectServiceRecipientQuantity(internalOrgId, competition.Id, parentOdsCode, solution.CatalogueItemId, null, service.CatalogueItemId))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.SubLocations));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectServiceRecipientQuantity_WithAdditionalServiceId_ReturnsViewWithModel(
        string internalOrgId,
        string parentOdsCode,
        Competition competition,
        CompetitionSolution competitionSolution,
        AdditionalService additionalService,
        CompetitionAdditionalService competitionAdditionalService,
        AssociatedService associatedService,
        CompetitionCatalogueItemPrice competitionPrice,
        CompetitionCatalogueItemPrice servicePrice,
        List<CompetitionItemQuantity> solutionQuantities,
        List<ServiceRecipientQuantityDto> recipientQuantities,
        CataloguePrice cataloguePrice,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionPrice.ProvisioningType = ProvisioningType.Declarative;
        associatedService.CatalogueItem.CataloguePrices = [cataloguePrice];

        competitionAdditionalService.CatalogueItem = additionalService.CatalogueItem;
        competitionAdditionalService.CatalogueItemId = additionalService.CatalogueItemId;
        competitionAdditionalService.Price = competitionPrice;
        competitionAdditionalService.Quantities = solutionQuantities;
        competitionAdditionalService.AssociatedServices = [
            new CompetitionAssociatedService(competition.Id, associatedService.CatalogueItemId)
            {
                CatalogueItem = associatedService.CatalogueItem,
                Price = servicePrice,
            }
        ];

        competitionSolution.Services = [competitionAdditionalService];

        competition.CompetitionSolutions = [competitionSolution];

        recipientQuantities.ForEach(recipient => recipient.ParentSublocationOdsCode = parentOdsCode);

        competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competition.Id).Returns(competition);
        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, competitionSolution.CatalogueItemId).Returns(competitionSolution);

        var expectedModel = new SelectServiceRecipientQuantityModel(
            associatedService.CatalogueItem,
            servicePrice,
            recipientQuantities);

        var result =
            (await controller.SelectServiceRecipientQuantity(internalOrgId, competition.Id, parentOdsCode, competitionSolution.CatalogueItemId, additionalService.CatalogueItemId, associatedService.CatalogueItemId))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should()
            .BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.SubLocations));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectServiceRecipientQuantity_InvalidModel_SetsModelError(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        SelectServiceRecipientQuantityModel model,
        string parentOdsCode,
        CompetitionHubController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.SelectServiceRecipientQuantity(internalOrgId, competitionId, parentOdsCode, solutionId, model))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectServiceRecipientQuantity_NullServiceId_SetsSolutionRecipientQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        string parentOdsCode,
        SelectServiceRecipientQuantityModel model,
        ServiceRecipientQuantityModel[] serviceRecipients,
        [Frozen] ICompetitionsQuantityService competitionsQuantityService,
        CompetitionHubController controller)
    {
        serviceRecipients.ForEach(x =>
        {
            x.InputQuantity = x.Quantity.ToString();
            x.ParentSublocationOdsCode = parentOdsCode;
        });

        model.SubLocations = new SubLocationModel[] { new SubLocationModel("Location One", serviceRecipients), };

        _ = await controller.SelectServiceRecipientQuantity(internalOrgId, competitionId, parentOdsCode, solutionId, model);

        await competitionsQuantityService.Received()
            .SetSolutionRecipientQuantity(
                internalOrgId,
                competitionId,
                solutionId,
                Arg.Any<List<ServiceRecipientQuantityDto>>());
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectServiceRecipientQuantity_WithServiceId_SetsSolutionRecipientQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        string parentOdsCode,
        SelectServiceRecipientQuantityModel model,
        ServiceRecipientQuantityModel[] serviceRecipients,
        [Frozen] ICompetitionsQuantityService competitionsQuantityService,
        CompetitionHubController controller)
    {
        serviceRecipients.ForEach(x =>
        {
            x.InputQuantity = x.Quantity.ToString();
            x.ParentSublocationOdsCode = parentOdsCode;
        });

        model.SubLocations = [new SubLocationModel("Location One", serviceRecipients)];

        _ = await controller.SelectServiceRecipientQuantity(internalOrgId, competitionId, parentOdsCode, solutionId, model, null, serviceId);

        await competitionsQuantityService.Received()
            .SetServiceRecipientQuantity(
                internalOrgId,
                competitionId,
                solutionId,
                serviceId,
                Arg.Any<IEnumerable<ServiceRecipientQuantityDto>>());
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectServiceRecipientQuantity_WithAdditionalServiceId_SetsSolutionRecipientQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId additionalServiceId,
        CatalogueItemId associatedServiceId,
        string parentOdsCode,
        SelectServiceRecipientQuantityModel model,
        ServiceRecipientQuantityModel[] serviceRecipients,
        [Frozen] ICompetitionsQuantityService competitionsQuantityService,
        CompetitionHubController controller)
    {
        serviceRecipients.ForEach(x =>
        {
            x.InputQuantity = x.Quantity.ToString();
            x.ParentSublocationOdsCode = parentOdsCode;
        });

        model.SubLocations = [new SubLocationModel("Location One", serviceRecipients)];

        _ = await controller.SelectServiceRecipientQuantity(internalOrgId, competitionId, parentOdsCode, solutionId, model, additionalServiceId, associatedServiceId);

        await competitionsQuantityService.Received()
            .SetAdditionalServiceAssociatedServiceQuantity(
                internalOrgId,
                competitionId,
                solutionId,
                additionalServiceId,
                associatedServiceId,
                Arg.Any<IEnumerable<ServiceRecipientQuantityDto>>());
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectServiceRecipientQuantity_NullServiceId_Redirects(
        string internalOrgId,
        int competitionId,
        string parentOdsCode,
        CatalogueItemId solutionId,
        SelectServiceRecipientQuantityModel model,
        ServiceRecipientQuantityModel[] serviceRecipients,
        CompetitionHubController controller)
    {
        serviceRecipients.ForEach(x =>
        {
            x.InputQuantity = x.Quantity.ToString();
            x.ParentSublocationOdsCode = parentOdsCode;
        });

        model.SubLocations = [new SubLocationModel("Location One", serviceRecipients)];

        var result = (await controller.SelectServiceRecipientQuantity(internalOrgId, competitionId, parentOdsCode, solutionId, model))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.CompetitionSublocationHub));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectServiceRecipientQuantity_AdditionalServiceId_Redirects(
        string internalOrgId,
        int competitionId,
        string parentOdsCode,
        CatalogueItemId solutionId,
        CatalogueItemId additionalServiceId,
        CatalogueItemId associatedServiceId,
        SelectServiceRecipientQuantityModel model,
        ServiceRecipientQuantityModel[] serviceRecipients,
        CompetitionHubController controller)
    {
        serviceRecipients.ForEach(x =>
        {
            x.InputQuantity = x.Quantity.ToString();
            x.ParentSublocationOdsCode = parentOdsCode;
        });

        model.SubLocations = [new SubLocationModel("Location One", serviceRecipients)];

        var result = (await controller.SelectServiceRecipientQuantity(internalOrgId, competitionId, parentOdsCode, solutionId, model, additionalServiceId, associatedServiceId))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.HubAdditionalServiceAssociatedServices));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectAssociatedServices_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        List<CompetitionAssociatedService> solutionServices,
        Solution solution,
        List<AssociatedService> associatedServices,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IAssociatedServicesService associatedServicesService,
        CompetitionHubController controller)
    {
        solutionServices.ForEach(x => x.CatalogueItem = associatedServices.First().CatalogueItem);

        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competitionSolution.Services = [.. solutionServices.Cast<CompetitionCatalogueItem>()];

        competition.CompetitionSolutions = [competitionSolution];

        competitionsService
            .GetCompetitionSolution(internalOrgId, competition.Id, solution.CatalogueItemId)
            .Returns(competitionSolution);

        associatedServicesService
            .GetPublishedAssociatedServicesForCatalogueItem(competitionSolution.CatalogueItemId, PracticeReorganisationTypeEnum.None)
            .Returns([.. associatedServices.Select(x => x.CatalogueItem)]);

        var expectedModel = new SelectServicesModel(
            [.. solutionServices.Select(x => x.CatalogueItem)],
            [.. associatedServices.Select(x => x.CatalogueItem)])
        {
            EntityType = "Competition",
            ParentItemName = solution.CatalogueItem.Name,
            InternalOrgId = internalOrgId,
            ParentItemId = solution.CatalogueItemId,
        };

        var result = (await controller.SelectAssociatedServices(
            internalOrgId,
            competition.Id,
            competitionSolution.CatalogueItemId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink).Excluding(m => m.ParentItemType));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectAssociatedServices_InvalidModel_SetsModelError(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        List<CompetitionAssociatedService> solutionServices,
        Solution solution,
        List<AssociatedService> associatedServices,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IAssociatedServicesService associatedServicesService,
        CompetitionHubController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        solutionServices.ForEach(
            x =>
            {
                x.CatalogueItem = associatedServices.First().CatalogueItem;
            });

        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competitionSolution.Services = solutionServices.Cast<CompetitionCatalogueItem>().ToList();

        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, solution.CatalogueItemId).Returns(competitionSolution);

        associatedServicesService.GetPublishedAssociatedServicesForCatalogueItem(competitionSolution.CatalogueItemId, PracticeReorganisationTypeEnum.None).Returns(associatedServices.Select(x => x.CatalogueItem).ToList());

        var model = new SelectServicesModel(
            solutionServices.Select(x => x.CatalogueItem).ToList(),
            associatedServices.Select(x => x.CatalogueItem).ToList())
        {
            EntityType = "Competition",
            ParentItemName = solution.CatalogueItem.Name,
            InternalOrgId = internalOrgId,
            ParentItemId = solution.CatalogueItemId,
        };

        var result = (await controller.SelectAssociatedServices(internalOrgId, competition.Id, solution.CatalogueItemId, model))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectAssociatedServices_Valid_Redirects(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        List<CompetitionAssociatedService> solutionServices,
        Solution solution,
        List<AssociatedService> associatedServices,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IAssociatedServicesService associatedServicesService,
        CompetitionHubController controller)
    {
        solutionServices.ForEach(
            x =>
            {
                x.CatalogueItem = associatedServices.First().CatalogueItem;
                x.CatalogueItemId = associatedServices.First().CatalogueItemId;
            });

        competitionSolution.CatalogueItem = solution.CatalogueItem;
        competitionSolution.CatalogueItemId = solution.CatalogueItemId;
        competitionSolution.Services = solutionServices.Cast<CompetitionCatalogueItem>().ToList();

        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionSolution(internalOrgId, competition.Id, solution.CatalogueItemId).Returns(competitionSolution);

        associatedServicesService.GetPublishedAssociatedServicesForCatalogueItem(competitionSolution.CatalogueItemId, PracticeReorganisationTypeEnum.None).Returns(associatedServices.Select(x => x.CatalogueItem).ToList());

        var model = new SelectServicesModel(
            solutionServices.Select(x => x.CatalogueItem).ToList(),
            associatedServices.Select(x => x.CatalogueItem).ToList())
        {
            EntityType = "Competition",
            ParentItemName = solution.CatalogueItem.Name,
            InternalOrgId = internalOrgId,
        };

        var result = (await controller.SelectAssociatedServices(internalOrgId, competition.Id, solution.CatalogueItemId, model))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Hub));
    }

    [Theory]
    [MockAutoData]
    public static async Task RemoveAssociatedService_IncorrectCompetitionId_ReturnsBadRequestResult(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId).Returns((Competition)null);

        var result = await controller.RemoveAssociatedService(internalOrgId, competitionId, solutionId, serviceId);

        result.Should().BeOfType<BadRequestResult>();
    }

    [Theory]
    [MockAutoData]
    public static async Task RemoveAssociatedService_IncorrectSolutionId_ReturnsBadRequestResult(
        string internalOrgId,
        Competition competition,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        var result = await controller.RemoveAssociatedService(internalOrgId, competition.Id, solutionId, serviceId);

        result.Should().BeOfType<BadRequestResult>();
    }

    [Theory]
    [MockAutoData]
    public static async Task RemoveAssociatedService_IncorrectServiceId_ReturnsBadRequestResult(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CatalogueItemId serviceId,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        var result = await controller.RemoveAssociatedService(internalOrgId, competition.Id, competitionSolution.CatalogueItemId, serviceId);

        result.Should().BeOfType<BadRequestResult>();
    }

    [Theory]
    [MockAutoData]
    public static async Task RemoveAssociatedService_Valid_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CompetitionAssociatedService solutionService,
        AssociatedService associatedService,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        solutionService.CatalogueItemId = associatedService.CatalogueItemId;
        solutionService.CatalogueItem = associatedService.CatalogueItem;

        competitionSolution.Services = [solutionService];
        competition.CompetitionSolutions = [competitionSolution];

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new RemoveServiceModel(solutionService.CatalogueItem) { EntityType = "Competition" };

        var result = await controller.RemoveAssociatedService(internalOrgId, competition.Id, competitionSolution.CatalogueItemId, solutionService.CatalogueItemId);

        var viewResult = result.Should().BeOfType<ViewResult>();
        viewResult.Subject.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task RemoveAssociatedService_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        RemoveServiceModel model,
        CompetitionHubController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = await controller.RemoveAssociatedService(internalOrgId, competitionId, solutionId, serviceId, model);

        var viewResult = result.Should().BeOfType<ViewResult>();

        viewResult.Subject.Model.Should().BeEquivalentTo(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task RemoveAssociatedService_Confirmed_RemovesAssociatedService(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        RemoveServiceModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        model.ConfirmRemoveService = true;

        var result = await controller.RemoveAssociatedService(internalOrgId, competitionId, solutionId, serviceId, model);

        result.Should().NotBeNull();
        await competitionsService.Received()
            .RemoveAssociatedService(internalOrgId, competitionId, solutionId, serviceId);
    }

    [Theory]
    [MockInlineAutoData(true)]
    [MockInlineAutoData(false)]
    public static async Task RemoveAssociatedService_RedirectsToHub(
        bool confirmed,
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        RemoveServiceModel model,
        CompetitionHubController controller)
    {
        model.ConfirmRemoveService = confirmed;

        var result = await controller.RemoveAssociatedService(internalOrgId, competitionId, solutionId, serviceId, model);

        var redirectResult = result.Should().BeOfType<RedirectToActionResult>();

        redirectResult.Subject.ActionName.Should().Be(nameof(controller.Hub));
    }

    [Theory]
    [MockAutoData]
    public static async Task GetRecipientQuantities_QuantityNotNull_ReturnsCorrectResult(
        string internalOrgId,
        string odsCode,
        CompetitionSublocationRecipient competitionRecipient,
        CompetitionItemQuantity recipientQuantity,
        ServiceRecipient serviceRecipient,
        GpPracticeSize gpPracticeSize,
        [Frozen] IOdsService odsService,
        [Frozen] IGpPracticeService gpPracticeService,
        CompetitionHubController controller)
    {
        recipientQuantity.RecipientOdsCode = competitionRecipient.RecipientOdsCode = serviceRecipient.OrgId = odsCode;

        var competitionRecipients = new List<CompetitionSublocationRecipient> { competitionRecipient };
        var recipientQuantities = new List<CompetitionItemQuantity> { recipientQuantity };

        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                internalOrgId,
                Arg.Any<IEnumerable<string>>())
            .Returns([serviceRecipient]);
        gpPracticeService.GetNumberOfPatients(Arg.Any<IEnumerable<string>>()).Returns([gpPracticeSize]);

        var serviceRecipients = await controller.GetRecipientQuantities(competitionRecipients, recipientQuantities, internalOrgId);

        var expected = new ServiceRecipientQuantityDto(
            competitionRecipient.ParentSublocationOdsCode,
            recipientQuantity.RecipientOdsCode,
            competitionRecipient.RecipientOrganisation.Name,
            recipientQuantity.Quantity,
            serviceRecipient.Location);
        var expectedList = new List<ServiceRecipientQuantityDto> { expected };

        serviceRecipients.Should().NotBeNull();
        serviceRecipients.Should().BeEquivalentTo(expectedList);
    }

    [Theory]
    [MockAutoData]
    public static async Task GetRecipientQuantities_QuantityNull_ReturnsCorrectResult(
        string internalOrgId,
        string odsCode,
        CompetitionSublocationRecipient competitionRecipient,
        ServiceRecipient serviceRecipient,
        GpPracticeSize gpPractice,
        [Frozen] IOdsService odsService,
        [Frozen] IGpPracticeService gpPracticeService,
        CompetitionHubController controller)
    {
        gpPractice.OdsCode = competitionRecipient.RecipientOdsCode = serviceRecipient.OrgId = odsCode;

        var competitionRecipients = new List<CompetitionSublocationRecipient> { competitionRecipient };
        var recipientQuantities = Enumerable.Empty<CompetitionItemQuantity>().ToList();

        odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                internalOrgId,
                Arg.Any<IEnumerable<string>>())
            .Returns([serviceRecipient]);
        gpPracticeService.GetNumberOfPatients(Arg.Any<IEnumerable<string>>()).Returns([gpPractice]);

        var serviceRecipients = await controller.GetRecipientQuantities(competitionRecipients, recipientQuantities, internalOrgId);

        var expected = new ServiceRecipientQuantityDto(
            competitionRecipient.ParentSublocationOdsCode,
            gpPractice.OdsCode,
            competitionRecipient.RecipientOrganisation.Name,
            gpPractice.NumberOfPatients,
            serviceRecipient.Location);
        var expectedList = new List<ServiceRecipientQuantityDto> { expected };

        serviceRecipients.Should().NotBeNull();
        serviceRecipients.Should().BeEquivalentTo(expectedList);
    }
}
