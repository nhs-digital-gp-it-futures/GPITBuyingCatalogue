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
    private const string RemoveServiceViewName = "Services/RemoveService";
    private const string ConfirmPriceViewName = "PriceSelection/ConfirmPrice";

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
        var competition = await competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competitionId);
        var solution = await competitionsService.GetCompetitionSolution(internalOrgId, competitionId, solutionId);
        if (solution is null) return BadRequest();

        var model = new CompetitionSolutionHubModel(internalOrgId, solution, competition.FlattenedRecipients, competition.ContractLength)
        {
            BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId }),
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
        CatalogueItemId? additionalServiceId = null,
        CatalogueItemId? serviceId = null,
        int? selectedPriceId = null)
    {
        var solution = await competitionsService.GetCompetitionSolution(internalOrgId, competitionId, solutionId);
        if (solution is null) return BadRequest();

        var existingPrice = serviceId is null
            ? solution.Price
            : GetServiceItem(solution, serviceId, additionalServiceId)?.Price;

        var catalogueItem = await listPriceService.GetCatalogueItemWithPublishedListPrices(serviceId ?? solutionId);

        var model = new SelectPriceModel(catalogueItem)
        {
            BackLink = additionalServiceId is not null
                ? Url.Action(
                    nameof(HubAdditionalServiceAssociatedServices),
                    new { internalOrgId, competitionId, solutionId, additionalServiceItemId = additionalServiceId })
                : Url.Action(nameof(Hub), new { internalOrgId, competitionId, solutionId }),
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
        CatalogueItemId? additionalServiceId = null,
        CatalogueItemId? serviceId = null,
        RoutingSource? source = null)
    {
        var solution = await competitionsService.GetCompetitionSolution(internalOrgId, competitionId, solutionId);
        if (solution is null) return BadRequest();

        var existingPrice = serviceId is null
            ? solution.Price
            : GetServiceItem(solution, serviceId, additionalServiceId)?.Price;

        var catalogueItem = await listPriceService.GetCatalogueItemWithPublishedListPrices(serviceId ?? solutionId);
        var price = catalogueItem.CataloguePrices.First(x => x.CataloguePriceId == priceId);
        var model = new ConfirmPriceModel(catalogueItem, price, existingPrice);

        if (additionalServiceId is not null)
        {
            model.BackLink = Url.Action(
                nameof(HubAdditionalServiceAssociatedServices),
                new { internalOrgId, competitionId, solutionId, additionalServiceItemId = additionalServiceId });
        }
        else if (source is RoutingSource.TaskList)
        {
            model.BackLink = Url.Action(nameof(Hub), new { internalOrgId, competitionId, solutionId });
        }
        else
        {
            model.BackLink = Url.Action(
                nameof(SelectPrice),
                new { internalOrgId, competitionId, solutionId, serviceId, selectedPriceId = priceId });
        }

        return View(ConfirmPriceViewName, model);
    }

    [HttpPost("{solutionId}/select-price/{priceId}/confirm")]
    public async Task<IActionResult> ConfirmPrice(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        int priceId,
        ConfirmPriceModel model,
        CatalogueItemId? additionalServiceId = null,
        CatalogueItemId? serviceId = null)
    {
        if (!ModelState.IsValid)
        {
            return View(ConfirmPriceViewName, model);
        }

        var prices = await listPriceService.GetCatalogueItemWithPublishedListPrices(serviceId ?? solutionId);
        var price = prices.CataloguePrices.First(x => x.CataloguePriceId == priceId);

        if (additionalServiceId is not null)
        {
            await competitionsPriceService.SetAdditionalServiceAssociatedServicePrice(
                internalOrgId,
                competitionId,
                solutionId,
                additionalServiceId.GetValueOrDefault(),
                serviceId.GetValueOrDefault(),
                price,
                model.AgreedPrices);

            return RedirectToAction(
                nameof(HubAdditionalServiceAssociatedServices),
                new
                {
                    internalOrgId,
                    competitionId,
                    solutionId,
                    additionalServiceItemId = additionalServiceId.GetValueOrDefault(),
                });
        }

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
        CatalogueItemId? additionalServiceId = null,
        CatalogueItemId? serviceId = null)
    {
        var competition = await competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competitionId);
        var solution = await competitionsService.GetCompetitionSolution(internalOrgId, competitionId, solutionId);

        CompetitionCatalogueItem item = serviceId is null
            ? solution
            : GetServiceItem(solution, serviceId, additionalServiceId);

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
            BackLink = additionalServiceId is not null
                ? Url.Action(
                    nameof(HubAdditionalServiceAssociatedServices),
                    new { internalOrgId, competitionId, solutionId, additionalServiceItemId = additionalServiceId })
                : Url.Action(nameof(Hub), new { internalOrgId, competitionId, solutionId }),
            Caption = catalogueItem.Name,
            SubLocations = CreateSublocationHelper.CreateSubLocations(recipients ?? [])
                .Select(sublocation => new SubLocationModel(sublocation)
                {
                    ForwardingLink = Url.Action(
                        nameof(SelectServiceRecipientQuantity),
                        typeof(CompetitionHubController).ControllerName(),
                        new { internalOrgId, sublocation.OdsCode, competitionId, solutionId, additionalServiceId, serviceId }),
                }).ToArray(),
        };

        return View(SublocationHubViewName, model);
    }

    [HttpPost("{solutionId}/select-quantity")]
    public async Task<IActionResult> CompetitionSublocationHub(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        SublocationQuantityHubModel model,
        CatalogueItemId? additionalServiceId = null,
        CatalogueItemId? serviceId = null)
    {
        var competition = await competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competitionId);
        var solution = await competitionsService.GetCompetitionSolution(internalOrgId, competitionId, solutionId);

        CompetitionCatalogueItem item = serviceId is null
            ? solution
            : GetServiceItem(solution, serviceId, additionalServiceId);

        if (item is null) return BadRequest();

        var quantities = item.Quantities;

        var recipients = await GetRecipientQuantities(
            competition.FlattenedRecipients.ToList(),
            quantities?.ToList(),
            internalOrgId);

        return !recipients.All(recipient => recipient.Quantity.HasValue) ?
             RedirectToAction(nameof(Hub), new { internalOrgId, competitionId, solutionId })
             : RedirectToAction(nameof(ConfirmQuantities), new { internalOrgId, competitionId, solutionId, additionalServiceId, serviceId });
    }

    [HttpGet("sublocations/confirm-quantities")]
    public async Task<IActionResult> ConfirmQuantities(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId? additionalServiceId = null,
        CatalogueItemId? serviceId = null)
    {
        var competition = await competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competitionId);
        var solution = await competitionsService.GetCompetitionSolution(internalOrgId, competitionId, solutionId);

        CompetitionCatalogueItem item = serviceId is null
            ? solution
            : GetServiceItem(solution, serviceId, additionalServiceId);

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
            BackLink = additionalServiceId is null
                ? Url.Action(
                    nameof(CompetitionSublocationHub),
                    new { internalOrgId, competitionId, solutionId, serviceId })
                : Url.Action(
                    nameof(CompetitionSublocationHub),
                    new { internalOrgId, competitionId, solutionId, additionalServiceId, serviceId }),
            ContinueLink = additionalServiceId is null
                ? Url.Action(
                    nameof(Hub),
                    new { internalOrgId, competitionId, solutionId, serviceId })
                : Url.Action(
                    nameof(HubAdditionalServiceAssociatedServices),
                    new { internalOrgId, competitionId, solutionId, additionalServiceItemId = additionalServiceId }),
        };

        return View(ConfirmQuantitiesViewName, model);
    }

    [HttpGet("{solutionId}/select-recipient-quantity/{odsCode}")]
    public async Task<IActionResult> SelectServiceRecipientQuantity(
        string internalOrgId,
        int competitionId,
        string odsCode,
        CatalogueItemId solutionId,
        CatalogueItemId? additionalServiceId = null,
        CatalogueItemId? serviceId = null)
    {
        var competition = await competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competitionId);
        var solution = await competitionsService.GetCompetitionSolution(internalOrgId, competitionId, solutionId);

        (IPrice price, CatalogueItem item, IEnumerable<ServiceRecipientQuantityDto> recipientQuantities) =
            await GetRecipientQuantityDetails(competition, solution, internalOrgId, odsCode, additionalServiceId, serviceId);

        var model = new SelectServiceRecipientQuantityModel(item, price, recipientQuantities)
        {
            BackLink = additionalServiceId is null
            ? Url.Action(nameof(CompetitionSublocationHub), new { internalOrgId, competitionId, solutionId, serviceId })
            : Url.Action(nameof(CompetitionSublocationHub), new { internalOrgId, competitionId, solutionId, additionalServiceId, serviceId }),
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
        CatalogueItemId? additionalServiceId = null,
        CatalogueItemId? serviceId = null)
    {
        if (!ModelState.IsValid)
        {
            return View(ServiceSublocationRecipientViewName, model);
        }

        List<ServiceRecipientQuantityDto> quantities = [.. model.SubLocations[0].ServiceRecipients
            .Select(x => new ServiceRecipientQuantityDto
            {
                ParentSublocationOdsCode = odsCode,
                RecipientOdsCode = x.RecipientOdsCode,
                Quantity = string.IsNullOrWhiteSpace(x.InputQuantity)
                    ? null
                    : int.Parse(x.InputQuantity),
            })];

        if (additionalServiceId is not null)
        {
            await competitionsQuantityService.SetAdditionalServiceAssociatedServiceQuantity(
                internalOrgId,
                competitionId,
                solutionId,
                additionalServiceId.GetValueOrDefault(),
                serviceId.GetValueOrDefault(),
                quantities);

            return RedirectToAction(
                nameof(HubAdditionalServiceAssociatedServices),
                new
                {
                    internalOrgId,
                    competitionId,
                    solutionId,
                    additionalServiceItemId = additionalServiceId.GetValueOrDefault(),
                });
        }

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

        return RedirectToAction(nameof(CompetitionSublocationHub), new { internalOrgId, competitionId, solutionId, additionalServiceId, serviceId });
    }

    [HttpGet("{solutionId}/additional-services/{additionalServiceItemId}/associated-services")]
    public async Task<IActionResult> HubAdditionalServiceAssociatedServices(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId additionalServiceItemId)
    {
        var competition = await competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(internalOrgId, competitionId);
        var solution = await competitionsService.GetCompetitionSolution(internalOrgId, competitionId, solutionId);
        if (solution is null) return BadRequest();

        var additionalService = solution.GetAdditionalServices().FirstOrDefault(x => x.CatalogueItemId == additionalServiceItemId);
        if (additionalService is null || !additionalService.CompetitionAssociatedServices.Any())
        {
            return RedirectToAction(nameof(Hub), new { internalOrgId, competitionId, solutionId });
        }

        return View(new AdditionalServiceAssociatedServicesHubModel()
        {
            BackLink = Url.Action(nameof(Hub), new { internalOrgId, competitionId, solutionId }),
            InternalOrgId = internalOrgId,
            CompetitionId = competitionId,
            SolutionId = solutionId,
            AdditionalServiceItemId = additionalServiceItemId,
            AdditionalServiceName = additionalService.CatalogueItem.Name,
            AssociatedServicesRemaining = additionalService.AssociatedServicesRemaining,
            AssociatedServices = additionalService.CompetitionAssociatedServices.Select(s =>
                new AdditionalServiceAssociatedServiceItemModel(
                    additionalServiceItemId,
                    s.CatalogueItem,
                    s.Quantity,
                    competition.FlattenedRecipients.ToDictionary(
                        y => y,
                        y => s.Quantities.FirstOrDefault(z => z.RecipientOdsCode == y.RecipientOdsCode)?.Quantity),
                    s.Price)
                {
                    InternalOrgId = internalOrgId,
                    CompetitionId = solution.CompetitionId,
                    SolutionId = solutionId,
                    ContractLength = competition.ContractLength,
                }),
        });
    }

    [HttpGet("{solutionId}/associated-services")]
    public async Task<IActionResult> SelectAssociatedServices(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId? serviceId = null,
        CatalogueItemType? parentCatalogueItemType = CatalogueItemType.Solution)
    {
        return View(
            SelectAssociatedServicesViewName,
            await GetSelectServicesModel(internalOrgId, competitionId, solutionId, serviceId, parentCatalogueItemType));
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
                await GetSelectServicesModel(internalOrgId, competitionId, solutionId, model.ParentItemId, model.ParentItemType));
        }

        var serviceIds = model.Services?
            .Where(x => x.IsSelected)
            .Select(x => x.CatalogueItemId)
            .ToArray() ?? [];

        if (model.ParentItemType == CatalogueItemType.AdditionalService)
        {
            await competitionsService.AddAssociatedServicesToAdditionalService(
                internalOrgId,
                competitionId,
                solutionId,
                model.ParentItemId,
                serviceIds);

            return RedirectToAction(
                nameof(HubAdditionalServiceAssociatedServices),
                new { internalOrgId, competitionId, solutionId, additionalServiceItemId = model.ParentItemId });
        }

        await competitionsService.AddAssociatedServices(
            internalOrgId,
            competitionId,
            solutionId,
            serviceIds);

        return RedirectToAction(nameof(Hub), new { internalOrgId, competitionId, solutionId });
    }

    [HttpGet("{solutionId}/additional-services/{additionalServiceItemId}/associated-services/{serviceId}/remove")]
    public async Task<IActionResult> RemoveAdditionalServiceAssociatedService(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId additionalServiceItemId,
        CatalogueItemId serviceId)
    {
        var solution = await competitionsService.GetCompetitionSolution(internalOrgId, competitionId, solutionId);
        if (solution == null) return BadRequest();

        var additionalService = solution.GetAdditionalServices().FirstOrDefault(x => x.CatalogueItemId == additionalServiceItemId);
        if (additionalService == null) return BadRequest();

        var associatedService = additionalService.CompetitionAssociatedServices.FirstOrDefault(x => x.CatalogueItemId == serviceId);
        if (associatedService == null) return BadRequest();

        var model = new RemoveServiceModel
        {
            ServiceName = associatedService.CatalogueItem.Name,
            ServiceType = associatedService.CatalogueItem.CatalogueItemType,
            BackLink = Url.Action(nameof(Hub), new { internalOrgId, competitionId, solutionId }),
            EntityType = "Competition",
        };

        return View(RemoveServiceViewName, model);
    }

    [HttpPost("{solutionId}/additional-services/{additionalServiceItemId}/associated-services/{serviceId}/remove")]
    public async Task<IActionResult> RemoveAdditionalServiceAssociatedService(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId additionalServiceItemId,
        CatalogueItemId serviceId,
        RemoveServiceModel model)
    {
        if (!ModelState.IsValid)
            return View(RemoveServiceViewName, model);

        if (model.ConfirmRemoveService.GetValueOrDefault())
        {
            await competitionsService.RemoveAssociatedServicesFromAdditionalService(
                internalOrgId, competitionId, solutionId, additionalServiceItemId, serviceId);
        }

        return RedirectToAction(nameof(HubAdditionalServiceAssociatedServices), new
        {
            internalOrgId,
            competitionId,
            solutionId,
            additionalServiceItemId,
        });
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

        var service = solution.GetAssociatedServices().FirstOrDefault(x => x.CatalogueItemId == serviceId);
        if (service == null) return BadRequest();

        var model = new RemoveServiceModel
        {
            ServiceName = service.CatalogueItem.Name,
            ServiceType = service.CatalogueItem.CatalogueItemType,
            BackLink = Url.Action(nameof(Hub), new { internalOrgId, competitionId, solutionId }),
            EntityType = "Competition",
        };

        return View(RemoveServiceViewName, model);
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
            return View(RemoveServiceViewName, model);

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

    private static CompetitionCatalogueItem GetServiceItem(
        CompetitionSolution competitionSolution,
        CatalogueItemId? serviceId,
        CatalogueItemId? additionalServiceId)
    {
        if (competitionSolution is null || serviceId is null)
            return null;

        if (additionalServiceId is null)
            return competitionSolution.Services.FirstOrDefault(x => x.CatalogueItemId == serviceId);

        var additionalService = competitionSolution.GetAdditionalServices().FirstOrDefault(x => x.CatalogueItemId == additionalServiceId);
        return additionalService?.CompetitionAssociatedServices.FirstOrDefault(x => x.CatalogueItemId == serviceId);
    }

    private async Task<SelectServicesModel> GetSelectServicesModel(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId? serviceId = null,
        CatalogueItemType? parentItemType = CatalogueItemType.Solution)
    {
        CatalogueItemId itemId = solutionId;
        string itemName = string.Empty;
        IEnumerable<CatalogueItem> currentServices;
        IEnumerable<CatalogueItem> associatedServices;
        var solution = await competitionsService.GetCompetitionSolution(internalOrgId, competitionId, solutionId)
            ?? throw new ArgumentException("Solution not found", nameof(solutionId));

        if (parentItemType is CatalogueItemType.AdditionalService)
        {
            if (!serviceId.HasValue)
                throw new ArgumentNullException(nameof(serviceId));

            itemId = serviceId.Value;
            var additionalService = solution.GetAdditionalServices().FirstOrDefault(x => x.CatalogueItemId == serviceId)
                ?? throw new ArgumentException("Service not found", nameof(serviceId));

            currentServices = additionalService.CompetitionAssociatedServices.Select(x => x.CatalogueItem);
            itemName = additionalService.CatalogueItem.Name;
        }
        else
        {
            currentServices = solution.GetAssociatedServices().Select(x => x.CatalogueItem);
            itemName = solution.CatalogueItem.Name;
        }

        associatedServices = await associatedServicesService.GetPublishedAssociatedServicesForCatalogueItem(itemId, PracticeReorganisationTypeEnum.None);

        return new SelectServicesModel(currentServices, associatedServices)
        {
            BackLink = Url.Action(nameof(Hub), new { internalOrgId, competitionId, solutionId }),
            InternalOrgId = internalOrgId,
            ParentItemName = itemName,
            EntityType = "Competition",
            ParentItemId = itemId,
            ParentItemType = parentItemType,
        };
    }

    private async Task<(
        IPrice Price,
        CatalogueItem CatalogueItem,
        IEnumerable<ServiceRecipientQuantityDto> RecipientQuantities)>
        GetRecipientQuantityDetails(
            Competition competition,
            CompetitionSolution competitionSolution,
            string internalOrgId,
            string parentOdsCode,
            CatalogueItemId? additionalServiceId = null,
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

        var service = GetServiceItem(competitionSolution, serviceId, additionalServiceId);

        return service is null
            ? throw new ArgumentException(ServiceNotFoundErrorMessage)
            : ((IPrice Price, CatalogueItem CatalogueItem, IEnumerable<ServiceRecipientQuantityDto> RecipientQuantities))(
                service.Price,
                service.CatalogueItem,
                await GetRecipientQuantities(
                    competition.FlattenedRecipients.ToList(),
                    service.Quantities.Cast<CompetitionItemQuantity>().ToList(),
                    internalOrgId,
                    parentOdsCode));
    }
}
