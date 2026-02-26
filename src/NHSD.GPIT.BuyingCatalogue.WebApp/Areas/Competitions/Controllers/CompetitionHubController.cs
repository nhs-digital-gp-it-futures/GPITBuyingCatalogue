using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrice;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/hub")]
public class CompetitionHubController : Controller
{
    private const string ServiceSublocationRecipientViewName = "QuantitySelection/SelectServiceSublocationRecipientQuantity";
    private const string SelectAssociatedServicesViewName = "Services/SelectAssociatedServices";
    private const string SublocationHubViewName = "QuantitySelection/SublocationHub";
    private const string ConfirmQuantitiesViewName = "QuantitySelection/ConfirmQuantities";
    private const string ServiceNotFoundErrorMessage = "Service not found";

    private readonly IOdsService odsService;
    private readonly ICompetitionsService competitionsService;
    private readonly ICompetitionsPriceService competitionsPriceService;
    private readonly ICompetitionsQuantityService competitionsQuantityService;
    private readonly IListPriceService listPriceService;
    private readonly IGpPracticeService gpPracticeService;
    private readonly IAssociatedServicesService associatedServicesService;

    public CompetitionHubController(
        IOdsService odsService,
        ICompetitionsService competitionsService,
        ICompetitionsPriceService competitionsPriceService,
        ICompetitionsQuantityService competitionsQuantityService,
        IListPriceService listPriceService,
        IGpPracticeService gpPracticeService,
        IAssociatedServicesService associatedServicesService)
    {
        this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
        this.competitionsPriceService = competitionsPriceService
            ?? throw new ArgumentNullException(nameof(competitionsPriceService));
        this.competitionsQuantityService = competitionsQuantityService
            ?? throw new ArgumentNullException(nameof(competitionsQuantityService));
        this.listPriceService = listPriceService ?? throw new ArgumentNullException(nameof(listPriceService));
        this.gpPracticeService = gpPracticeService ?? throw new ArgumentNullException(nameof(gpPracticeService));
        this.associatedServicesService = associatedServicesService
            ?? throw new ArgumentNullException(nameof(associatedServicesService));
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId);

        var model = new PricingDashboardModel(competition)
        {
            BackLink = Url.Action(
                nameof(CompetitionTaskListController.Index),
                typeof(CompetitionTaskListController).ControllerName(),
                new { internalOrgId, competitionId }),
            InternalOrgId = internalOrgId,
        };

        return View(model);
    }

    [HttpGet("{solutionId}")]
    public async Task<IActionResult> Hub(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId)
    {
        var competition = await competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId);
        var solution = competition.CompetitionSolutions.FirstOrDefault(x => x.CatalogueItemId == solutionId);
        if (solution is null) return BadRequest();

        var associatedServices = await associatedServicesService.GetPublishedAssociatedServicesForCatalogueItem(solutionId, PracticeReorganisationTypeEnum.None);
        var selectedAssociatedServices = solution.AssociatedServices;

        var model = new CompetitionSolutionHubModel(internalOrgId, solution, competition)
        {
            BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId }),
            AssociatedServicesRemaining = associatedServices.Any(x => selectedAssociatedServices.All(y => x.Id != y.CatalogueItemId)),
            AssociatedServicesAvailable = associatedServices.Any(),
            AssociatedServicesUrl = Url.Action(
                nameof(SelectAssociatedServices),
                new { internalOrgId, competitionId, solutionId }),
        };

        return View(model);
    }

    [HttpGet("{solutionId}/select-price")]
    public async Task<IActionResult> SelectPrice(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId? serviceId = null,
        int? selectedPriceId = null)
    {
        var competition = await competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId);
        var solution = competition.CompetitionSolutions.First(x => x.CatalogueItemId == solutionId);

        var existingPrice = serviceId is not null
            ? solution.Services.FirstOrDefault(x => x.CatalogueItemId == serviceId)?.Price
            : solution.Price;

        var catalogueItem = await listPriceService.GetCatalogueItemWithPublishedListPrices(serviceId ?? solutionId);

        var model = new SelectPriceModel(catalogueItem)
        {
            BackLink = Url.Action(nameof(Hub), new { internalOrgId, competitionId, solutionId }),
            SelectedPriceId = selectedPriceId ?? existingPrice?.CataloguePriceId,
        };

        return View("PriceSelection/SelectPrice", model);
    }

    [HttpPost("{solutionId}/select-price")]
    public async Task<IActionResult> SelectPrice(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        SelectPriceModel model,
        CatalogueItemId? serviceId = null)
    {
        if (!ModelState.IsValid)
        {
            var itemWithPrices =
                await listPriceService.GetCatalogueItemWithPublishedListPrices(serviceId ?? solutionId);

            model.Prices = itemWithPrices.CataloguePrices.OrderBy(cp => cp.CataloguePriceType).ToList();
            return View("PriceSelection/SelectPrice", model);
        }

        var priceId = model.SelectedPriceId!.Value;

        return RedirectToAction(
            nameof(ConfirmPrice),
            new
            {
                internalOrgId,
                competitionId,
                solutionId,
                priceId,
                serviceId,
            });
    }

    [HttpGet("{solutionId}/select-price/{priceId}/confirm")]
    public async Task<IActionResult> ConfirmPrice(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        int priceId,
        CatalogueItemId? serviceId = null,
        RoutingSource? source = null)
    {
        var competition = await competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId);
        var solution = competition.CompetitionSolutions.First(x => x.CatalogueItemId == solutionId);

        var existingPrice = serviceId is not null
            ? solution.Services.FirstOrDefault(x => x.CatalogueItemId == serviceId)?.Price
            : solution.Price;

        var catalogueItem = await listPriceService.GetCatalogueItemWithPublishedListPrices(serviceId ?? solutionId);
        var price = catalogueItem.CataloguePrices.First(x => x.CataloguePriceId == priceId);

        var model = new ConfirmPriceModel(catalogueItem, price, existingPrice)
        {
            BackLink = source is RoutingSource.TaskList
                ? Url.Action(nameof(Hub), new { internalOrgId, competitionId, solutionId })
                : Url.Action(
                    nameof(SelectPrice),
                    new
                    {
                        internalOrgId,
                        competitionId,
                        solutionId,
                        serviceId,
                        selectedPriceId = priceId,
                    }),
        };

        return View("PriceSelection/ConfirmPrice", model);
    }

    [HttpPost("{solutionId}/select-price/{priceId}/confirm")]
    public async Task<IActionResult> ConfirmPrice(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        int priceId,
        ConfirmPriceModel model,
        CatalogueItemId? serviceId = null)
    {
        if (!ModelState.IsValid)
        {
            return View("PriceSelection/ConfirmPrice", model);
        }

        var prices = await listPriceService.GetCatalogueItemWithPublishedListPrices(serviceId ?? solutionId);
        var price = prices.CataloguePrices.First(x => x.CataloguePriceId == priceId);

        if (serviceId is not null)
        {
            await competitionsPriceService.SetServicePrice(
                internalOrgId,
                competitionId,
                solutionId,
                serviceId.GetValueOrDefault(),
                price,
                model.AgreedPrices);
        }
        else
        {
            await competitionsPriceService.SetSolutionPrice(
                internalOrgId,
                competitionId,
                solutionId,
                price,
                model.AgreedPrices);
        }

        return RedirectToAction(nameof(Hub), new { internalOrgId, competitionId, solutionId });
    }

    [HttpGet("{solutionId}/select-quantity")]
    public async Task<IActionResult> CompetitionSublocationHub(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId? serviceId = null)
    {
        var competition = await competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId);
        var competitionSolution =
            competition.CompetitionSolutions.FirstOrDefault(solution => solution.CatalogueItemId == solutionId);

        CompetitionCatalogueItem item = serviceId is not null
            ? competitionSolution?.Services.FirstOrDefault(x => x.CatalogueItemId == serviceId)
            : competitionSolution;

        if (item is null) return BadRequest();

        var quantities = item.Quantities;
        var recipients = await GetRecipientQuantities(
            competition.FlattenedRecipients.ToList(),
            quantities,
            internalOrgId);
        var catalogueItem = item.CatalogueItem;

        var model = new SublocationQuantityHubModel(
            competition.Organisation,
            catalogueItem)
        {
            BackLink = Url.Action(nameof(Hub), new { internalOrgId, competitionId, solutionId }),
            Caption = catalogueItem.Name,
            SubLocations = CreateSublocationHelper.CreateSubLocations(recipients ?? [])
                .Select(sublocation => new SubLocationModel(sublocation)
                {
                    ForwardingLink = Url.Action(
                        nameof(SelectServiceRecipientQuantity),
                        typeof(CompetitionHubController).ControllerName(),
                        new { internalOrgId, sublocation.OdsCode, competitionId, serviceId }),
                }).ToArray(),
        };

        return View(SublocationHubViewName, model);
    }

    [HttpPost("{solutionId}/select-quantity")]
    public async Task<IActionResult> CompetitionSublocationHub(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        SelectOrderItemQuantityModel model,
        CatalogueItemId? serviceId = null)
    {
        var competition = await competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId);
        var competitionSolution =
            competition.CompetitionSolutions.FirstOrDefault(solution => solution.CatalogueItemId == solutionId);

        CompetitionCatalogueItem item = serviceId is not null
            ? competitionSolution?.Services.FirstOrDefault(x => x.CatalogueItemId == serviceId)
            : competitionSolution;

        if (item is null) return BadRequest();

        var quantities = item.Quantities;

        var recipients = await GetRecipientQuantities(
            competition.FlattenedRecipients.ToList(),
            quantities?.ToList(),
            internalOrgId);

        return !recipients.All(recipient => recipient.Quantity.HasValue) ?
             RedirectToAction(nameof(Hub), new { internalOrgId, competitionId, solutionId })
             : RedirectToAction(nameof(ConfirmQuantities), new { internalOrgId, competitionId, solutionId, serviceId });
    }

    [HttpGet("sublocations/confirm-quantities")]
    public async Task<IActionResult> ConfirmQuantities(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId? serviceId = null)
    {
        var competition = await competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId);
        var competitionSolution =
            competition.CompetitionSolutions.FirstOrDefault(solution => solution.CatalogueItemId == solutionId);

        CompetitionCatalogueItem item = serviceId is not null
            ? competitionSolution?.Services.FirstOrDefault(x => x.CatalogueItemId == serviceId)
            : competitionSolution;

        if (item is null) return BadRequest();

        var quantities = item.Quantities;
        var catalogueItem = item.CatalogueItem;
        var price = item.Price;
        var recipients = await GetRecipientQuantities(
            competition.FlattenedRecipients.ToList(),
            quantities?.ToList(),
            internalOrgId);

        var model = new ConfirmQuantitiesModel(
            catalogueItem,
            price,
            recipients.ToList())
        {
            BackLink = Url.Action(
                nameof(CompetitionSublocationHub),
                typeof(CompetitionHubController).ControllerName(),
                new { internalOrgId, competitionId, solutionId, serviceId }),
            ContinueLink = Url.Action(
                nameof(Hub),
                typeof(CompetitionHubController).ControllerName(),
                new { internalOrgId, competitionId, solutionId, serviceId }),
        };

        return View(ConfirmQuantitiesViewName, model);
    }

    [HttpGet("{solutionId}/select-recipient-quantity/{odsCode}")]
    public async Task<IActionResult> SelectServiceRecipientQuantity(
        string internalOrgId,
        int competitionId,
        string odsCode,
        CatalogueItemId solutionId,
        CatalogueItemId? serviceId = null)
    {
        var competition = await competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId);
        var competitionSolution = competition.CompetitionSolutions.FirstOrDefault(x => x.CatalogueItemId == solutionId);

        (IPrice price, CatalogueItem item, IEnumerable<ServiceRecipientQuantityDto> recipientQuantities) =
            await GetRecipientQuantityDetails(competition, competitionSolution, internalOrgId, odsCode, serviceId);

        var model = new SelectServiceRecipientQuantityModel(item, price, recipientQuantities)
        {
            BackLink = Url.Action(nameof(CompetitionSublocationHub), new { internalOrgId, competitionId, solutionId, serviceId }),
        };

        return View(ServiceSublocationRecipientViewName, model);
    }

    [HttpPost("{solutionId}/select-recipient-quantity/{odsCode}")]
    public async Task<IActionResult> SelectServiceRecipientQuantity(
        string internalOrgId,
        int competitionId,
        string odsCode,
        CatalogueItemId solutionId,
        SelectServiceRecipientQuantityModel model,
        CatalogueItemId? serviceId = null)
    {
        if (!ModelState.IsValid)
        {
            return View(ServiceSublocationRecipientViewName, model);
        }

        List<ServiceRecipientQuantityDto> quantities = model.SubLocations[0].ServiceRecipients
            .Select(x => new ServiceRecipientQuantityDto
            {
                ParentSublocationOdsCode = odsCode,
                RecipientOdsCode = x.RecipientOdsCode,
                Quantity = string.IsNullOrWhiteSpace(x.InputQuantity)
                    ? null
                    : int.Parse(x.InputQuantity),
            })
            .ToList();

        if (serviceId is null)
        {
            await competitionsQuantityService.SetSolutionRecipientQuantity(
                internalOrgId,
                competitionId,
                solutionId,
                quantities);
        }
        else
        {
            await competitionsQuantityService.SetServiceRecipientQuantity(
                internalOrgId,
                competitionId,
                solutionId,
                serviceId.GetValueOrDefault(),
                quantities);
        }

        return RedirectToAction(nameof(CompetitionSublocationHub), new { internalOrgId, competitionId, solutionId, serviceId });
    }

    [HttpGet("{solutionId}/associated-services")]
    public async Task<IActionResult> SelectAssociatedServices(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId)
    {
        return View(
            SelectAssociatedServicesViewName,
            await GetSelectServicesModel(internalOrgId, competitionId, solutionId));
    }

    [HttpPost("{solutionId}/associated-services")]
    public async Task<IActionResult> SelectAssociatedServices(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        SelectServicesModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(
                SelectAssociatedServicesViewName,
                await GetSelectServicesModel(internalOrgId, competitionId, solutionId));
        }

        var serviceIds = model.Services?
            .Where(x => x.IsSelected)
            .Select(x => x.CatalogueItemId)
            .ToArray() ?? Array.Empty<CatalogueItemId>();

        await competitionsService.AddAssociatedServices(internalOrgId, competitionId, solutionId, serviceIds);

        return RedirectToAction(nameof(Hub), new { internalOrgId, competitionId, solutionId });
    }

    [HttpGet("{solutionId}/associated-services/{serviceId}/remove")]
    public async Task<IActionResult> RemoveAssociatedService(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId)
    {
        var competition = await competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId);
        var solution = competition?.CompetitionSolutions.FirstOrDefault(x => x.CatalogueItemId == solutionId);
        if (solution == null) return BadRequest();

        var service = solution.AssociatedServices.FirstOrDefault(x => x.CatalogueItemId == serviceId);
        if (service == null) return BadRequest();

        var model = new RemoveServiceModel(service.CatalogueItem)
        {
            BackLink = Url.Action(nameof(Hub), new { internalOrgId, competitionId, solutionId }),
            EntityType = "Competition",
        };

        return View("Services/RemoveService", model);
    }

    [HttpPost("{solutionId}/associated-services/{serviceId}/remove")]
    public async Task<IActionResult> RemoveAssociatedService(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        RemoveServiceModel model)
    {
        if (!ModelState.IsValid)
            return View("Services/RemoveService", model);

        if (model.ConfirmRemoveService.GetValueOrDefault())
        {
            await competitionsService.RemoveAssociatedService(internalOrgId, competitionId, solutionId, serviceId);
        }

        return RedirectToAction(nameof(Hub), new { internalOrgId, competitionId, solutionId });
    }

    internal async Task<IEnumerable<ServiceRecipientQuantityDto>> GetRecipientQuantities(
        IReadOnlyList<CompetitionSublocationRecipient> competitionRecipients,
        ICollection<CompetitionItemQuantity> recipientQuantities,
        string internalOrgId,
        string parentOdsCode = null)
    {
        List<string> competitionRecipientIds = competitionRecipients.Select(x => x.RecipientOdsCode).ToList();
        var practiceListSizes = await gpPracticeService.GetNumberOfPatients(competitionRecipientIds);
        IEnumerable<ServiceRecipient> organisations =
            await odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                internalOrgId,
                competitionRecipientIds);

        return competitionRecipients
            .Where(x => parentOdsCode == null || x.ParentSublocationOdsCode == parentOdsCode)
            .Select(x =>
        {
            var quantity = recipientQuantities?.FirstOrDefault(y => x.RecipientOdsCode == y.RecipientOdsCode)
                    ?.Quantity
                ?? practiceListSizes?.FirstOrDefault(y => y.OdsCode == x.RecipientOdsCode)?.NumberOfPatients;

            var location = organisations?.FirstOrDefault(y => x.RecipientOdsCode == y.OrgId)?.Location;

            return new ServiceRecipientQuantityDto(
                x.ParentSublocationOdsCode,
                x.RecipientOdsCode,
                x.RecipientOrganisation.Name,
                quantity,
                location);
        });
    }

    private async Task<SelectServicesModel> GetSelectServicesModel(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId)
    {
        var competition = await competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId);
        var solution = competition.CompetitionSolutions.First(x => x.CatalogueItemId == solutionId);

        var currentServices =
            solution.AssociatedServices.Select(x => x.CatalogueItem);

        var associatedServices = await associatedServicesService.GetPublishedAssociatedServicesForCatalogueItem(solutionId, PracticeReorganisationTypeEnum.None);

        return new SelectServicesModel(currentServices, associatedServices)
        {
            BackLink = Url.Action(nameof(Hub), new { internalOrgId, competitionId, solutionId }),
            InternalOrgId = internalOrgId,
            SolutionName = solution.CatalogueItem.Name,
            EntityType = "Competition",
            SolutionId = solutionId,
        };
    }

    private async Task<(IPrice Price, CatalogueItem CatalogueItem, IEnumerable<ServiceRecipientQuantityDto>
            RecipientQuantities)>
        GetRecipientQuantityDetails(
            Competition competition,
            CompetitionSolution competitionSolution,
            string internalOrgId,
            string parentOdsCode,
            CatalogueItemId? serviceId = null)
    {
        if (serviceId is null)
        {
            return (competitionSolution.Price, competitionSolution.CatalogueItem,
                await GetRecipientQuantities(
                    competition.FlattenedRecipients.ToList(),
                    competitionSolution.Quantities.Cast<CompetitionItemQuantity>().ToList(),
                    internalOrgId,
                    parentOdsCode));
        }

        var service = competitionSolution.Services.FirstOrDefault(x => x.CatalogueItemId == serviceId);
        if (service is null) throw new ArgumentException(ServiceNotFoundErrorMessage);

        return (service.Price, service.CatalogueItem,
            await GetRecipientQuantities(
                competition.FlattenedRecipients.ToList(),
                service.Quantities.Cast<CompetitionItemQuantity>().ToList(),
                internalOrgId,
                parentOdsCode));
    }
}
