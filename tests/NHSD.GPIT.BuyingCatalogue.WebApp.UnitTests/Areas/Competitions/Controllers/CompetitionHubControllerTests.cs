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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;
using Xunit;
using OdsOrganisation = NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models.OdsOrganisation;

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
        competition.CompetitionSolutions = new List<CompetitionSolution>();

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
        List<AssociatedService> associatedServices,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IAssociatedServicesService associatedServicesService,
        CompetitionHubController controller)
    {
        competitionSolution.Solution = solution;
        competitionSolution.SolutionId = solution.CatalogueItemId;
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        associatedServicesService.GetPublishedAssociatedServicesForSolution(competitionSolution.SolutionId, PracticeReorganisationTypeEnum.None).Returns(associatedServices.Select(x => x.CatalogueItem).ToList());

        var expectedModel = new CompetitionSolutionHubModel(internalOrgId, competitionSolution, competition)
        {
            AssociatedServicesAvailable = associatedServices.Any(),
        };

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
        competitionSolution.SolutionId = catalogueItem.Id;
        competitionSolution.Price = competitionPrice;

        catalogueItem.CataloguePrices = new List<CataloguePrice> { cataloguePrice };
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

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
        SolutionService solutionService,
        CompetitionCatalogueItemPrice competitionPrice,
        CompetitionCatalogueItemPrice servicePrice,
        Solution solution,
        AssociatedService service,
        CataloguePrice cataloguePrice,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IListPriceService listPriceService,
        CompetitionHubController controller)
    {
        solutionService.SolutionId = solution.CatalogueItemId;
        solutionService.ServiceId = service.CatalogueItemId;
        solutionService.Price = servicePrice;

        competitionSolution.SolutionId = solution.CatalogueItemId;
        competitionSolution.Price = competitionPrice;
        competitionSolution.SolutionServices = new List<SolutionService> { solutionService };

        solution.CatalogueItem.CataloguePrices = new List<CataloguePrice> { cataloguePrice };
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        listPriceService.GetCatalogueItemWithPublishedListPrices(service.CatalogueItemId).Returns(service.CatalogueItem);

        var result = (await controller.SelectPrice(internalOrgId, competition.Id, solution.CatalogueItemId, service.CatalogueItemId))
            .As<ViewResult>();

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
        competitionSolution.SolutionId = catalogueItem.Id;
        competitionSolution.Price = competitionPrice;

        catalogueItem.CataloguePrices = new List<CataloguePrice> { cataloguePrice };
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

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
        competitionSolution.SolutionId = catalogueItem.Id;
        competitionSolution.Price = competitionPrice;

        catalogueItem.CataloguePrices = new List<CataloguePrice> { cataloguePrice };
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

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
        competitionSolution.SolutionId = catalogueItem.Id;
        competitionSolution.Price = competitionPrice;

        catalogueItem.CataloguePrices = new List<CataloguePrice> { cataloguePrice };
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

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
        SolutionService solutionService,
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
        solutionService.SolutionId = solution.CatalogueItemId;
        solutionService.ServiceId = service.CatalogueItemId;
        solutionService.Price = servicePrice;

        competitionSolution.SolutionId = solution.CatalogueItemId;
        competitionSolution.Price = competitionPrice;
        competitionSolution.SolutionServices = new List<SolutionService> { solutionService };

        service.CatalogueItem.CataloguePrices = new List<CataloguePrice> { additionalServicePrice };
        solution.CatalogueItem.CataloguePrices = new List<CataloguePrice> { cataloguePrice };
        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        listPriceService.GetCatalogueItemWithPublishedListPrices(service.CatalogueItemId).Returns(service.CatalogueItem);

        var expectedModel = new ConfirmPriceModel(service.CatalogueItem, additionalServicePrice, competitionPrice);

        var result = (await controller.ConfirmPrice(
            internalOrgId,
            competition.Id,
            solution.CatalogueItemId,
            additionalServicePrice.CataloguePriceId,
            service.CatalogueItemId)).As<ViewResult>();

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
            additionalService.Id);

        await competitionsPriceService.Received().SetServicePrice(internalOrgId, competitionId, catalogueItem.Id, additionalService.Id, price, Arg.Any<List<PricingTierDto>>());
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
    public static async Task SelectQuantity_NullServiceIdWithPerRecipientPrice_Redirects(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice competitionPrice,
        Solution solution,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionPrice.CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient;

        competitionSolution.Solution = solution;
        competitionSolution.SolutionId = solution.CatalogueItemId;
        competitionSolution.Price = competitionPrice;

        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        var result = (await controller.SelectQuantity(internalOrgId, competition.Id, solution.CatalogueItemId))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.SelectServiceRecipientQuantity));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { "competitionId", competition.Id },
                    { "solutionId", solution.CatalogueItemId },
                    { "serviceId", null },
                });
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectQuantity_WithServiceIdWithPerRecipientPrice_Redirects(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        SolutionService solutionService,
        CompetitionCatalogueItemPrice competitionPrice,
        Solution solution,
        AdditionalService service,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionPrice.CataloguePriceQuantityCalculationType = CataloguePriceQuantityCalculationType.PerServiceRecipient;

        solutionService.Service = service.CatalogueItem;
        solutionService.ServiceId = service.CatalogueItemId;
        solutionService.Price = competitionPrice;

        competitionSolution.Solution = solution;
        competitionSolution.SolutionId = solution.CatalogueItemId;
        competitionSolution.SolutionServices = new List<SolutionService> { solutionService };

        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        var result = (await controller.SelectQuantity(internalOrgId, competition.Id, solution.CatalogueItemId, service.CatalogueItemId))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.SelectServiceRecipientQuantity));
        result.RouteValues.Should()
            .BeEquivalentTo(
                new RouteValueDictionary
                {
                    { nameof(internalOrgId), internalOrgId },
                    { "competitionId", competition.Id },
                    { "solutionId", solution.CatalogueItemId },
                    { "serviceId", service.CatalogueItemId },
                });
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectQuantity_NullServiceId_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice competitionPrice,
        Solution solution,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionPrice.ProvisioningType = ProvisioningType.Declarative;

        competitionSolution.Solution = solution;
        competitionSolution.SolutionId = solution.CatalogueItemId;
        competitionSolution.Price = competitionPrice;

        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new SelectOrderItemQuantityModel(
            solution.CatalogueItem,
            competitionPrice,
            competitionSolution.Quantity);

        var result = (await controller.SelectQuantity(internalOrgId, competition.Id, solution.CatalogueItemId))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectQuantity_WithServiceId_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        SolutionService solutionService,
        CompetitionCatalogueItemPrice competitionPrice,
        Solution solution,
        AdditionalService service,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionPrice.ProvisioningType = ProvisioningType.Declarative;

        solutionService.Service = service.CatalogueItem;
        solutionService.ServiceId = service.CatalogueItemId;
        solutionService.Price = competitionPrice;

        competitionSolution.Solution = solution;
        competitionSolution.SolutionId = solution.CatalogueItemId;
        competitionSolution.SolutionServices = new List<SolutionService> { solutionService };

        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new SelectOrderItemQuantityModel(
            service.CatalogueItem,
            competitionPrice,
            solutionService.Quantity);

        var result = (await controller.SelectQuantity(internalOrgId, competition.Id, solution.CatalogueItemId, service.CatalogueItemId))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectQuantity_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        SelectOrderItemQuantityModel model,
        CompetitionHubController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.SelectQuantity(internalOrgId, competitionId, solutionId, model))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectQuantity_NullServiceId_SetsSolutionQuantity(
        string internalOrgId,
        int competitionId,
        int quantity,
        CatalogueItemId solutionId,
        SelectOrderItemQuantityModel model,
        [Frozen] ICompetitionsQuantityService competitionsQuantityService,
        CompetitionHubController controller)
    {
        model.Quantity = quantity.ToString();

        _ = await controller.SelectQuantity(internalOrgId, competitionId, solutionId, model);

        await competitionsQuantityService.Received().SetSolutionGlobalQuantity(internalOrgId, competitionId, solutionId, quantity);
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectQuantity_WithServiceId_SetsServiceQuantity(
        string internalOrgId,
        int competitionId,
        int quantity,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        SelectOrderItemQuantityModel model,
        [Frozen] ICompetitionsQuantityService competitionsQuantityService,
        CompetitionHubController controller)
    {
        model.Quantity = quantity.ToString();

        _ = await controller.SelectQuantity(internalOrgId, competitionId, solutionId, model, serviceId);

        await competitionsQuantityService.Received().SetServiceGlobalQuantity(internalOrgId, competitionId, solutionId, serviceId, quantity);
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectServiceRecipientQuantity_NullServiceId_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        CompetitionCatalogueItemPrice competitionPrice,
        List<SolutionQuantity> solutionQuantities,
        List<ServiceRecipientDto> recipientQuantities,
        Solution solution,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionPrice.ProvisioningType = ProvisioningType.Declarative;

        competitionSolution.Solution = solution;
        competitionSolution.SolutionId = solution.CatalogueItemId;
        competitionSolution.Price = competitionPrice;
        competitionSolution.Quantities = solutionQuantities;

        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new SelectServiceRecipientQuantityModel(
            solution.CatalogueItem,
            competitionPrice,
            recipientQuantities);

        var result =
            (await controller.SelectServiceRecipientQuantity(internalOrgId, competition.Id, solution.CatalogueItemId))
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
        SolutionService solutionService,
        CompetitionCatalogueItemPrice competitionPrice,
        List<ServiceQuantity> solutionQuantities,
        List<ServiceRecipientDto> recipientQuantities,
        Solution solution,
        AdditionalService service,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionPrice.ProvisioningType = ProvisioningType.Declarative;

        solutionService.Service = service.CatalogueItem;
        solutionService.ServiceId = service.CatalogueItemId;
        solutionService.Price = competitionPrice;
        solutionService.Quantities = solutionQuantities;

        competitionSolution.Solution = solution;
        competitionSolution.SolutionId = solution.CatalogueItemId;
        competitionSolution.SolutionServices = new List<SolutionService> { solutionService };

        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        var expectedModel = new SelectServiceRecipientQuantityModel(
            service.CatalogueItem,
            competitionPrice,
            recipientQuantities);

        var result =
            (await controller.SelectServiceRecipientQuantity(internalOrgId, competition.Id, solution.CatalogueItemId, service.CatalogueItemId))
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
        CompetitionHubController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.SelectServiceRecipientQuantity(internalOrgId, competitionId, solutionId, model))
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
        SelectServiceRecipientQuantityModel model,
        ServiceRecipientQuantityModel[] serviceRecipients,
        [Frozen] ICompetitionsQuantityService competitionsQuantityService,
        CompetitionHubController controller)
    {
        serviceRecipients.ForEach(x => x.InputQuantity = x.Quantity.ToString());

        model.SubLocations = new SubLocationModel[] { new SubLocationModel("Location One", serviceRecipients), };

        _ = await controller.SelectServiceRecipientQuantity(internalOrgId, competitionId, solutionId, model);

        await competitionsQuantityService.Received().SetSolutionRecipientQuantity(
                internalOrgId,
                competitionId,
                solutionId,
                Arg.Any<List<ServiceRecipientDto>>());
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectServiceRecipientQuantity_WithServiceId_SetsSolutionRecipientQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        SelectServiceRecipientQuantityModel model,
        ServiceRecipientQuantityModel[] serviceRecipients,
        [Frozen] ICompetitionsQuantityService competitionsQuantityService,
        CompetitionHubController controller)
    {
        serviceRecipients.ForEach(x => x.InputQuantity = x.Quantity.ToString());

        model.SubLocations = new SubLocationModel[] { new SubLocationModel("Location One", serviceRecipients), };

        _ = await controller.SelectServiceRecipientQuantity(internalOrgId, competitionId, solutionId, model, serviceId);

        await competitionsQuantityService.Received().SetServiceRecipientQuantity(
                internalOrgId,
                competitionId,
                solutionId,
                serviceId,
                Arg.Any<IEnumerable<ServiceRecipientDto>>());
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectServiceRecipientQuantity_NullServiceId_Redirects(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        SelectServiceRecipientQuantityModel model,
        ServiceRecipientQuantityModel[] serviceRecipients,
        CompetitionHubController controller)
    {
        serviceRecipients.ForEach(x => x.InputQuantity = x.Quantity.ToString());

        model.SubLocations = new SubLocationModel[] { new SubLocationModel("Location One", serviceRecipients), };

        var result = (await controller.SelectServiceRecipientQuantity(internalOrgId, competitionId, solutionId, model))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Hub));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectAssociatedServices_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        List<SolutionService> solutionServices,
        Solution solution,
        List<AssociatedService> associatedServices,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IAssociatedServicesService associatedServicesService,
        CompetitionHubController controller)
    {
        solutionServices.ForEach(x => x.Service = associatedServices.First().CatalogueItem);

        competitionSolution.Solution = solution;
        competitionSolution.SolutionId = solution.CatalogueItemId;
        competitionSolution.SolutionServices = solutionServices;

        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        associatedServicesService.GetPublishedAssociatedServicesForSolution(competitionSolution.SolutionId, PracticeReorganisationTypeEnum.None).Returns(associatedServices.Select(x => x.CatalogueItem).ToList());

        var expectedModel = new SelectServicesModel(
            solutionServices.Select(x => x.Service).ToList(),
            associatedServices.Select(x => x.CatalogueItem).ToList())
        {
            EntityType = "Competition",
            SolutionName = solution.CatalogueItem.Name,
            InternalOrgId = internalOrgId,
            SolutionId = solution.CatalogueItemId,
        };

        var result = (await controller.SelectAssociatedServices(
            internalOrgId,
            competition.Id,
            competitionSolution.SolutionId)).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectAssociatedServices_InvalidModel_SetsModelError(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        List<SolutionService> solutionServices,
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
                x.IsRequired = false;
                x.Service = associatedServices.First().CatalogueItem;
            });

        competitionSolution.Solution = solution;
        competitionSolution.SolutionId = solution.CatalogueItemId;
        competitionSolution.SolutionServices = solutionServices;

        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        associatedServicesService.GetPublishedAssociatedServicesForSolution(competitionSolution.SolutionId, PracticeReorganisationTypeEnum.None).Returns(associatedServices.Select(x => x.CatalogueItem).ToList());

        var model = new SelectServicesModel(
            solutionServices.Select(x => x.Service).ToList(),
            associatedServices.Select(x => x.CatalogueItem).ToList())
        {
            EntityType = "Competition",
            SolutionName = solution.CatalogueItem.Name,
            InternalOrgId = internalOrgId,
            SolutionId = solution.CatalogueItemId,
        };

        var result = (await controller.SelectAssociatedServices(internalOrgId, competition.Id, solution.CatalogueItemId, model))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectAssociatedServices_ExistingServicesNotSelected_Redirects(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        List<SolutionService> solutionServices,
        Solution solution,
        List<AssociatedService> associatedServices,
        AssociatedService existingService,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IAssociatedServicesService associatedServicesService,
        CompetitionHubController controller)
    {
        solutionServices.ForEach(
            x =>
            {
                x.IsRequired = false;
                x.Service = existingService.CatalogueItem;
            });

        competitionSolution.Solution = solution;
        competitionSolution.SolutionId = solution.CatalogueItemId;
        competitionSolution.SolutionServices = solutionServices;

        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        associatedServicesService.GetPublishedAssociatedServicesForSolution(competitionSolution.SolutionId, PracticeReorganisationTypeEnum.None).Returns(associatedServices.Select(x => x.CatalogueItem).ToList());

        var model = new SelectServicesModel(
            solutionServices.Select(x => x.Service).ToList(),
            associatedServices.Select(x => x.CatalogueItem).ToList())
        {
            EntityType = "Competition",
            SolutionName = solution.CatalogueItem.Name,
            InternalOrgId = internalOrgId,
        };

        var result = (await controller.SelectAssociatedServices(internalOrgId, competition.Id, solution.CatalogueItemId, model))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.ConfirmAssociatedServiceChanges));
    }

    [Theory]
    [MockAutoData]
    public static async Task SelectAssociatedServices_Valid_Redirects(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        List<SolutionService> solutionServices,
        Solution solution,
        List<AssociatedService> associatedServices,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IAssociatedServicesService associatedServicesService,
        CompetitionHubController controller)
    {
        solutionServices.ForEach(
            x =>
            {
                x.IsRequired = false;
                x.Service = associatedServices.First().CatalogueItem;
                x.ServiceId = associatedServices.First().CatalogueItemId;
            });

        competitionSolution.Solution = solution;
        competitionSolution.SolutionId = solution.CatalogueItemId;
        competitionSolution.SolutionServices = solutionServices;

        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        associatedServicesService.GetPublishedAssociatedServicesForSolution(competitionSolution.SolutionId, PracticeReorganisationTypeEnum.None).Returns(associatedServices.Select(x => x.CatalogueItem).ToList());

        var model = new SelectServicesModel(
            solutionServices.Select(x => x.Service).ToList(),
            associatedServices.Select(x => x.CatalogueItem).ToList())
        {
            EntityType = "Competition",
            SolutionName = solution.CatalogueItem.Name,
            InternalOrgId = internalOrgId,
        };

        var result = (await controller.SelectAssociatedServices(internalOrgId, competition.Id, solution.CatalogueItemId, model))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Hub));
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmAssociatedServiceChanges_InvalidSolution_ReturnsBadRequest(
        string internalOrgId,
        Competition competition,
        CatalogueItemId solutionId,
        string serviceIds,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        var result =
            (await controller.ConfirmAssociatedServiceChanges(internalOrgId, competition.Id, solutionId, serviceIds))
            .As<BadRequestResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmAssociatedServiceChanges_ReturnsViewWithModel(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        List<SolutionService> solutionServices,
        Solution solution,
        List<AssociatedService> associatedServices,
        AssociatedService existingService,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IAssociatedServicesService associatedServicesService,
        CompetitionHubController controller)
    {
        solutionServices.ForEach(
            x =>
            {
                x.IsRequired = false;
                x.Service = existingService.CatalogueItem;
                x.ServiceId = existingService.CatalogueItemId;
            });

        competitionSolution.Solution = solution;
        competitionSolution.SolutionId = solution.CatalogueItemId;
        competitionSolution.SolutionServices = solutionServices;

        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        associatedServicesService.GetPublishedAssociatedServicesForSolution(competitionSolution.SolutionId, PracticeReorganisationTypeEnum.None).Returns(associatedServices.Concat(new[] { existingService }).Select(x => x.CatalogueItem).ToList());

        var expectedModel = new ConfirmServiceChangesModel(internalOrgId, CatalogueItemType.AssociatedService)
        {
            ToAdd = associatedServices.Select(
                    x => new ServiceModel { CatalogueItemId = x.CatalogueItemId, Description = x.CatalogueItem.Name })
                .ToList(),
            ToRemove = solutionServices.Select(
                    x => new ServiceModel { CatalogueItemId = x.ServiceId, Description = x.Service.Name })
                .ToList(),
            Caption = solution.CatalogueItem.Name,
            EntityType = "Competition",
        };

        var result = (await controller.ConfirmAssociatedServiceChanges(
            internalOrgId,
            competition.Id,
            solution.CatalogueItemId,
            string.Join(',', associatedServices.Select(x => x.CatalogueItemId)))).As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmAssociatedServiceChanges_InvalidModel_ReturnsViewWithModel(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        ConfirmServiceChangesModel model,
        CompetitionHubController controller)
    {
        controller.ModelState.AddModelError("some-key", "some-error");

        var result = (await controller.ConfirmAssociatedServiceChanges(internalOrgId, competitionId, solutionId, model))
            .As<ViewResult>();

        result.Should().NotBeNull();
        result.Model.Should().Be(model);
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmAssociatedServiceChanges_ConfirmChangesFalse_Redirects(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        ConfirmServiceChangesModel model,
        CompetitionHubController controller)
    {
        model.ConfirmChanges = false;

        var result = (await controller.ConfirmAssociatedServiceChanges(internalOrgId, competitionId, solutionId, model))
            .As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Hub));
    }

    [Theory]
    [MockAutoData]
    public static async Task Post_ConfirmAssociatedServiceChanges_InvalidSolution_ReturnsBadRequest(
        string internalOrgId,
        Competition competition,
        CatalogueItemId solutionId,
        ConfirmServiceChangesModel model,
        [Frozen] ICompetitionsService competitionsService,
        CompetitionHubController controller)
    {
        model.ConfirmChanges = true;

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        var result =
            (await controller.ConfirmAssociatedServiceChanges(internalOrgId, competition.Id, solutionId, model))
            .As<BadRequestResult>();

        result.Should().NotBeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task ConfirmAssociatedServiceChanges_Valid_Redirects(
        string internalOrgId,
        Competition competition,
        CompetitionSolution competitionSolution,
        List<SolutionService> solutionServices,
        Solution solution,
        List<AssociatedService> associatedServices,
        AssociatedService existingService,
        ConfirmServiceChangesModel model,
        [Frozen] ICompetitionsService competitionsService,
        [Frozen] IAssociatedServicesService associatedServicesService,
        CompetitionHubController controller)
    {
        model.ConfirmChanges = true;

        solutionServices.ForEach(
            x =>
            {
                x.IsRequired = false;
                x.Service = existingService.CatalogueItem;
                x.ServiceId = existingService.CatalogueItemId;
            });

        competitionSolution.Solution = solution;
        competitionSolution.SolutionId = solution.CatalogueItemId;
        competitionSolution.SolutionServices = solutionServices;

        competition.CompetitionSolutions = new List<CompetitionSolution> { competitionSolution };

        competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competition.Id).Returns(competition);

        associatedServicesService.GetPublishedAssociatedServicesForSolution(competitionSolution.SolutionId, PracticeReorganisationTypeEnum.None).Returns(associatedServices.Concat(new[] { existingService }).Select(x => x.CatalogueItem).ToList());

        var result = (await controller.ConfirmAssociatedServiceChanges(
            internalOrgId,
            competition.Id,
            solution.CatalogueItemId,
            model)).As<RedirectToActionResult>();

        result.Should().NotBeNull();
        result.ActionName.Should().Be(nameof(controller.Hub));
    }

    [Theory]
    [MockAutoData]
    public static async Task GetRecipientQuantities_QuantityNotNull_ReturnsCorrectResult(
        string internalOrgId,
        string odsCode,
        OdsOrganisation competitionRecipient,
        RecipientQuantityBase recipientQuantity,
        ServiceRecipient serviceRecipient,
        GpPracticeSize gpPracticeSize,
        [Frozen] IOdsService odsService,
        [Frozen] IGpPracticeService gpPracticeService,
        CompetitionHubController controller)
    {
        recipientQuantity.OdsCode = competitionRecipient.Id = serviceRecipient.OrgId = odsCode;

        var competitionRecipients = new List<OdsOrganisation> { competitionRecipient };
        var recipientQuantities = new List<RecipientQuantityBase> { recipientQuantity };

        odsService.GetServiceRecipientsById(internalOrgId, Arg.Any<IEnumerable<string>>()).Returns(new List<ServiceRecipient> { serviceRecipient });
        gpPracticeService.GetNumberOfPatients(Arg.Any<IEnumerable<string>>()).Returns(new List<GpPracticeSize>() { gpPracticeSize });

        var serviceRecipients = await controller.GetRecipientQuantities(competitionRecipients, recipientQuantities, internalOrgId);

        var expected = new ServiceRecipientDto(recipientQuantity.OdsCode, competitionRecipient.Name, recipientQuantity.Quantity, serviceRecipient.Location);
        var expectedList = new List<ServiceRecipientDto> { expected };

        serviceRecipients.Should().NotBeNull();
        serviceRecipients.Should().BeEquivalentTo(expectedList);
    }

    [Theory]
    [MockAutoData]
    public static async Task GetRecipientQuantities_QuantityNull_ReturnsCorrectResult(
        string internalOrgId,
        string odsCode,
        OdsOrganisation competitionRecipient,
        ServiceRecipient serviceRecipient,
        GpPracticeSize gpPractice,
        [Frozen] IOdsService odsService,
        [Frozen] IGpPracticeService gpPracticeService,
        CompetitionHubController controller)
    {
        gpPractice.OdsCode = competitionRecipient.Id = serviceRecipient.OrgId = odsCode;

        var competitionRecipients = new List<OdsOrganisation> { competitionRecipient };
        var recipientQuantities = Enumerable.Empty<RecipientQuantityBase>().ToList();

        odsService.GetServiceRecipientsById(internalOrgId, Arg.Any<IEnumerable<string>>()).Returns(new List<ServiceRecipient> { serviceRecipient });
        gpPracticeService.GetNumberOfPatients(Arg.Any<IEnumerable<string>>()).Returns(new List<GpPracticeSize> { gpPractice });

        var serviceRecipients = await controller.GetRecipientQuantities(competitionRecipients, recipientQuantities, internalOrgId);

        var expected = new ServiceRecipientDto(gpPractice.OdsCode, competitionRecipient.Name, gpPractice.NumberOfPatients, serviceRecipient.Location);
        var expectedList = new List<ServiceRecipientDto> { expected };

        serviceRecipients.Should().NotBeNull();
        serviceRecipients.Should().BeEquivalentTo(expectedList);
    }
}
