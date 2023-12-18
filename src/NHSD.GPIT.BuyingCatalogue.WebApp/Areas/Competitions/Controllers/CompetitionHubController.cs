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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Pricing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;
using OdsOrganisation = NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models.OdsOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Development")]
[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/hub")]
public class CompetitionHubController : Controller
{
    private const string OrderItemViewName = "QuantitySelection/SelectOrderItemQuantity";
    private const string ServiceRecipientViewName = "QuantitySelection/SelectServiceRecipientQuantity";
    private const string SelectAssociatedServicesViewName = "Services/SelectAssociatedServices";

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
        var solution = competition.CompetitionSolutions.FirstOrDefault(x => x.SolutionId == solutionId);
        var associatedServices = await associatedServicesService.GetPublishedAssociatedServicesForSolution(solutionId, PracticeReorganisationTypeEnum.None);

        var model = new CompetitionSolutionHubModel(internalOrgId, solution, competition)
        {
            BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId }),
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
        var solution = competition.CompetitionSolutions.First(x => x.SolutionId == solutionId);

        var existingPrice = serviceId is not null
            ? solution.SolutionServices.FirstOrDefault(x => x.ServiceId == serviceId)?.Price
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
        var solution = competition.CompetitionSolutions.First(x => x.SolutionId == solutionId);

        var existingPrice = serviceId is not null
            ? solution.SolutionServices.FirstOrDefault(x => x.ServiceId == serviceId)?.Price
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
    public async Task<IActionResult> SelectQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId? serviceId = null)
    {
        var competition = await competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId);
        var competitionSolution = competition.CompetitionSolutions.FirstOrDefault(x => x.SolutionId == solutionId);

        (IPrice price, CatalogueItem catalogueItem, int? quantity) =
            GetGlobalQuantityDetails(competitionSolution, serviceId);

        if (price?.IsPerServiceRecipient() ?? false)
        {
            return RedirectToAction(
                nameof(SelectServiceRecipientQuantity),
                new { internalOrgId, competitionId, solutionId, serviceId });
        }

        var model = new SelectOrderItemQuantityModel(catalogueItem, price, quantity)
        {
            BackLink = Url.Action(nameof(Hub), new { internalOrgId, competitionId, solutionId }),
        };

        return View(OrderItemViewName, model);
    }

    [HttpPost("{solutionId}/select-quantity")]
    public async Task<IActionResult> SelectQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        SelectOrderItemQuantityModel model,
        CatalogueItemId? serviceId = null)
    {
        if (!ModelState.IsValid)
        {
            return View(OrderItemViewName, model);
        }

        if (serviceId is null)
        {
            await competitionsQuantityService.SetSolutionGlobalQuantity(
                internalOrgId,
                competitionId,
                solutionId,
                int.Parse(model.Quantity));
        }
        else
        {
            await competitionsQuantityService.SetServiceGlobalQuantity(
                internalOrgId,
                competitionId,
                solutionId,
                serviceId.GetValueOrDefault(),
                int.Parse(model.Quantity));
        }

        return RedirectToAction(nameof(Hub), new { internalOrgId, competitionId, solutionId });
    }

    [HttpGet("{solutionId}/select-recipient-quantity")]
    public async Task<IActionResult> SelectServiceRecipientQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId? serviceId = null)
    {
        var competition = await competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId);
        var competitionSolution = competition.CompetitionSolutions.FirstOrDefault(x => x.SolutionId == solutionId);

        (IPrice price, CatalogueItem item, IEnumerable<ServiceRecipientDto> recipientQuantities) =
            await GetRecipientQuantityDetails(competition, competitionSolution, internalOrgId, serviceId);

        var model = new SelectServiceRecipientQuantityModel(item, price, recipientQuantities)
        {
            BackLink = Url.Action(nameof(Hub), new { internalOrgId, competitionId, solutionId }),
        };

        return View(ServiceRecipientViewName, model);
    }

    [HttpPost("{solutionId}/select-recipient-quantity")]
    public async Task<IActionResult> SelectServiceRecipientQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        SelectServiceRecipientQuantityModel model,
        CatalogueItemId? serviceId = null)
    {
        if (!ModelState.IsValid)
        {
            return View(ServiceRecipientViewName, model);
        }

        var quantities = model.ServiceRecipients
            .Select(
                x => new ServiceRecipientDto
                {
                    OdsCode = x.OdsCode,
                    Quantity = string.IsNullOrWhiteSpace(x.InputQuantity)
                        ? x.Quantity
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

        return RedirectToAction(nameof(Hub), new { internalOrgId, competitionId, solutionId });
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

        var competition = await competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId);
        var solution = competition.CompetitionSolutions.First(x => x.SolutionId == solutionId);
        var existingServices = solution.GetAssociatedServices();

        if (existingServices.Any(x => !serviceIds.Contains(x.ServiceId)))
        {
            return RedirectToAction(
                nameof(ConfirmAssociatedServiceChanges),
                new { internalOrgId, competitionId, solutionId, serviceIds = string.Join(',', serviceIds) });
        }

        await competitionsService.SetAssociatedServices(internalOrgId, competitionId, solutionId, serviceIds);

        return RedirectToAction(nameof(Hub), new { internalOrgId, competitionId, solutionId });
    }

    [HttpGet("{solutionId}/associated-services/confirm")]
    public async Task<IActionResult> ConfirmAssociatedServiceChanges(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        string serviceIds)
    {
        var competition = await competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId);
        var solution = competition.CompetitionSolutions.FirstOrDefault(x => x.SolutionId == solutionId);
        if (solution == null) return BadRequest();

        var associatedServices =
            await associatedServicesService.GetPublishedAssociatedServicesForSolution(solutionId, PracticeReorganisationTypeEnum.None);

        var existingServiceIds = solution.GetAssociatedServices()
            .Select(x => x.ServiceId)
            .ToList();

        var selectedServiceIds = serviceIds?.Split(',')
            .Select(CatalogueItemId.ParseExact)
            .ToArray() ?? Array.Empty<CatalogueItemId>();

        var toAdd = selectedServiceIds
            .Where(x => !existingServiceIds.Contains(x))
            .Select(
                x => new ServiceModel
                {
                    CatalogueItemId = x, Description = associatedServices.FirstOrDefault(s => s.Id == x)?.Name,
                });

        var toRemove = existingServiceIds
            .Where(x => !selectedServiceIds.Contains(x))
            .Select(
                x => new ServiceModel
                {
                    CatalogueItemId = x, Description = associatedServices.FirstOrDefault(s => s.Id == x)?.Name,
                });

        var model = new ConfirmServiceChangesModel(internalOrgId, CatalogueItemType.AssociatedService)
        {
            BackLink = Url.Action(
                nameof(SelectAssociatedServices),
                new { internalOrgId, competitionId, solutionId }),
            ToAdd = toAdd.ToList(),
            ToRemove = toRemove.ToList(),
            Caption = solution.Solution.CatalogueItem.Name,
            EntityType = "Competition",
        };

        return View("Services/ConfirmChanges", model);
    }

    [HttpPost("{solutionId}/associated-services/confirm")]
    public async Task<IActionResult> ConfirmAssociatedServiceChanges(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        ConfirmServiceChangesModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Services/ConfirmChanges", model);
        }

        if (model.ConfirmChanges is false)
        {
            return RedirectToAction(
                nameof(Hub),
                new { internalOrgId, competitionId, solutionId });
        }

        var competition = await competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId);
        var solution = competition.CompetitionSolutions.FirstOrDefault(x => x.SolutionId == solutionId);
        if (solution == null) return BadRequest();

        var associatedServices = solution.GetAssociatedServices();
        var associatedServiceIds = associatedServices.Select(x => x.ServiceId);

        var servicesToAdd = model.ToAdd?.Select(x => x.CatalogueItemId) ?? Enumerable.Empty<CatalogueItemId>();
        var serviceToRemove = model.ToRemove?.Select(x => x.CatalogueItemId) ?? Enumerable.Empty<CatalogueItemId>();

        var serviceIds = associatedServiceIds.Concat(servicesToAdd)
            .Except(serviceToRemove);

        await competitionsService.SetAssociatedServices(
            internalOrgId,
            competitionId,
            solutionId,
            serviceIds);

        return RedirectToAction(
            nameof(Hub),
            new { internalOrgId, competitionId, solutionId });
    }

    internal async Task<IEnumerable<ServiceRecipientDto>> GetRecipientQuantities(
        ICollection<OdsOrganisation> competitionRecipients,
        ICollection<RecipientQuantityBase> recipientQuantities,
        string internalOrgId)
    {
        var competitionRecipientIds = competitionRecipients.Select(x => x.Id);
        var practiceListSizes = await gpPracticeService.GetNumberOfPatients(competitionRecipientIds);
        var organisations = await odsService.GetServiceRecipientsById(internalOrgId, competitionRecipientIds);

        return competitionRecipients.Select(
            x => new ServiceRecipientDto(
                x.Id,
                x.Name,
                recipientQuantities?.FirstOrDefault(y => x.Id == y.OdsCode)?.Quantity ?? practiceListSizes?.FirstOrDefault(y => y.OdsCode == x.Id)?.NumberOfPatients,
                organisations?.FirstOrDefault(y => x.Id == y.OrgId).Location));
    }

    private static (IPrice Price, CatalogueItem CatalogueItem, int? Quantity) GetGlobalQuantityDetails(
        CompetitionSolution competitionSolution,
        CatalogueItemId? serviceId = null)
    {
        if (serviceId is null)
        {
            return (competitionSolution.Price, competitionSolution.Solution.CatalogueItem,
                competitionSolution.Quantity);
        }

        var service = competitionSolution.SolutionServices.FirstOrDefault(x => x.ServiceId == serviceId);

        return service is null ? (null, null, null) : (service.Price, service.Service, service.Quantity);
    }

    private async Task<SelectServicesModel> GetSelectServicesModel(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId)
    {
        var competition = await competitionsService.GetCompetitionWithSolutionsHub(internalOrgId, competitionId);
        var solution = competition.CompetitionSolutions.First(x => x.SolutionId == solutionId);

        var currentServices =
            solution.GetAssociatedServices().Select(x => x.Service);

        var associatedServices = await associatedServicesService.GetPublishedAssociatedServicesForSolution(solutionId, PracticeReorganisationTypeEnum.None);

        return new SelectServicesModel(currentServices, associatedServices)
        {
            BackLink = Url.Action(nameof(Hub), new { internalOrgId, competitionId, solutionId }),
            InternalOrgId = internalOrgId,
            SolutionName = solution.Solution.CatalogueItem.Name,
            EntityType = "Competition",
            SolutionId = solutionId,
        };
    }

    private async Task<(IPrice Price, CatalogueItem CatalogueItem, IEnumerable<ServiceRecipientDto> RecipientQuantities)>
        GetRecipientQuantityDetails(
            Competition competition,
            CompetitionSolution competitionSolution,
            string internalOrgId,
            CatalogueItemId? serviceId = null)
    {
        if (serviceId is null)
        {
            return (competitionSolution.Price, competitionSolution.Solution.CatalogueItem,
                await GetRecipientQuantities(
                    competition.Recipients,
                    competitionSolution.Quantities.Cast<RecipientQuantityBase>().ToList(),
                    internalOrgId));
        }

        var service = competitionSolution.SolutionServices.FirstOrDefault(x => x.ServiceId == serviceId);
        if (service is null) throw new ArgumentException("Service not found");

        return (service.Price, service.Service,
            await GetRecipientQuantities(
                competition.Recipients,
                service.Quantities.Cast<RecipientQuantityBase>().ToList(),
                internalOrgId));
    }
}
