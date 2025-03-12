using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.FormContent;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/select-recipients")]
public class CompetitionRecipientsController : Controller
{
    internal const string ConfirmRecipientsAdvice =
        "Review the organisations you’ve selected to receive the winning solution for this competition.";

    private readonly IOrganisationsService organisationsService;
    private readonly ICompetitionsService competitionsService;
    private readonly ICompetitionSublocationService competitionSublocationService;
    private readonly IOdsService odsService;

    public CompetitionRecipientsController(
        IOrganisationsService organisationsService,
        ICompetitionsService competitionsService,
        ICompetitionSublocationService competitionSublocationService,
        IOdsService odsService)
    {
        this.organisationsService =
            organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
        this.competitionSublocationService = competitionSublocationService
            ?? throw new ArgumentNullException(nameof(competitionSublocationService));
        this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
    }

    [HttpGet("upload-or-select-service-recipients")]
    public async Task<IActionResult> UploadOrSelectServiceRecipients(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetition(internalOrgId, competitionId);
        var model = new UploadOrSelectServiceRecipientModel()
        {
            Caption = competition.Name,
            BackLink = Url.Action(
                nameof(CompetitionTaskListController.Index),
                typeof(CompetitionTaskListController).ControllerName(),
                new { internalOrgId, competitionId }),
        };
        return View("ServiceRecipients/UploadOrSelectServiceRecipient", model);
    }

    [HttpPost("upload-or-select-service-recipients")]
    public IActionResult UploadOrSelectServiceRecipients(
        UploadOrSelectServiceRecipientModel model,
        string internalOrgId,
        int competitionId)
    {
        if (!ModelState.IsValid)
            return View("ServiceRecipients/UploadOrSelectServiceRecipient", model);

        if (model.ShouldUploadRecipients.GetValueOrDefault())
        {
            return RedirectToAction(
                nameof(CompetitionImportServiceRecipientsController.Index),
                typeof(CompetitionImportServiceRecipientsController).ControllerName(),
                new { internalOrgId, competitionId });
        }

        return RedirectToAction(
            nameof(SelectSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId, isInitialSelection = true });
    }

    [HttpGet("select-sublocations")]
    public async Task<IActionResult> SelectSublocations(
        string internalOrgId,
        int competitionId,
        bool isInitialSelection)
    {
        Competition competition =
            await competitionsService.GetCompetitionWithSublocations(internalOrgId, competitionId);

        IEnumerable<OdsOrganisation> possibleSublocations =
            await odsService.GetSublocationsByParentInternalIdentifier(internalOrgId);

        var model = new SelectSublocationsModel(competition, possibleSublocations, isInitialSelection)
        {
            BackLink = Url.Action(
                nameof(UploadOrSelectServiceRecipients),
                typeof(CompetitionRecipientsController).ControllerName(),
                new { internalOrgId, competitionId }),
        };
        return View("ServiceRecipients/SelectSublocations", model);
    }

    [HttpPost("select-sublocations")]
    public async Task<IActionResult> SelectSublocations(
        [FilteredFormContent] Dictionary<string, string> form,
        string internalOrgId,
        int competitionId,
        bool isInitialSelection)
    {
        List<string> sublocationIds = form.Keys.ToList();

        await competitionsService.AddSublocations(internalOrgId, competitionId, sublocationIds);

        return RedirectToAction(
            nameof(AddSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });
    }

    [HttpGet("add-sublocations")]
    public async Task<IActionResult> AddSublocations(string internalOrgId, int competitionId)
    {
        var backLink = Url.Action(
            nameof(SelectSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });

        return await SelectSublocationsOverview(internalOrgId, competitionId, false, backLink);
    }

    [HttpPost("add-sublocations")]
    public async Task<IActionResult> AddSublocations()
    {
        throw new NotImplementedException();
    }

    [HttpGet("confirm-sublocations")]
    public async Task<IActionResult> ConfirmSublocations(string internalOrgId, int competitionId)
    {
        var backLink = Url.Action(
            nameof(CompetitionTaskListController.Index),
            typeof(CompetitionTaskListController).ControllerName(),
            new { internalOrgId, competitionId });

        return await SelectSublocationsOverview(internalOrgId, competitionId, true, backLink);
    }

    [HttpPost("confirm-sublocations")]
    public async Task<IActionResult> ConfirmSublocations()
    {
        throw new NotImplementedException();
    }

    [HttpGet("remove-sublocations")]
    public async Task<IActionResult> RemoveSublocations(
        string internalOrgId,
        int competitionId,
        string sublocationOdsCodes)
    {
        var splitSublocationOdsCodes = sublocationOdsCodes?.Split(
            [','],
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];

        Competition competition =
            await competitionsService.GetCompetitionWithSublocations(internalOrgId, competitionId);

        var model = new RemoveSublocationsModel(competition)
        {
            BackLink = Url.Action(
                nameof(ConfirmSublocations),
                typeof(CompetitionRecipientsController).ControllerName(),
                new { internalOrgId, competitionId }),
        };

        return View("ServiceRecipients/RemoveSublocations", model);
    }

    [HttpPost("remove-sublocations")]
    public async Task<IActionResult> RemoveSublocations()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{sublocationId}")]
    public async Task<IActionResult> SelectSublocationRecipients(
        string internalOrgId,
        int competitionId,
        string sublocationId,
        string recipientIds = "",
        string importedRecipients = "",
        SelectionMode? selectionMode = null)
    {
        Organisation organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

        Competition competition = await competitionsService.GetCompetition(internalOrgId, competitionId);
        CompetitionSublocation competitionSublocation =
            await competitionSublocationService.GetCompetitionSublocationWithRecipients(
                organisation.ExternalIdentifier,
                competitionId,
                sublocationId);

        var sublocationAsSublocationModel = new SublocationModel
        {
            Name = competitionSublocation.SublocationOrganisation.Name,
            OdsCode = competitionSublocation.SublocationOdsCode,
            ServiceRecipients = competitionSublocation.SublocationRecipients.Select(
                    x => new ServiceRecipientModel
                    {
                        OdsCode = x.RecipientOdsCode,
                        Name = x.RecipientOrganisation.Name,
                        Location = competition.Organisation.Name,
                        Selected = true,
                    })
                .ToList(),
        };

        List<ServiceRecipientModel> possibleRecipients = await GetServiceRecipientsBySublocation(sublocationId);
        var splitRecipientIds = string.Join(',', recipientIds, importedRecipients)
            .Split(
                ',',
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        var model = new SelectRecipientsV2Model(
            competition,
            sublocationAsSublocationModel,
            possibleRecipients,
            splitRecipientIds,
            selectionMode)
        {
            BackLink = Url.Action(
                nameof(ConfirmSublocations),
                typeof(CompetitionRecipientsController).ControllerName(),
                new { internalOrgId, competitionId }),
        };

        return View("ServiceRecipients/SelectRecipientsV2", model);
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string internalOrgId,
        int competitionId,
        string recipientIds = "",
        string importedRecipients = "",
        SelectionMode? selectionMode = null)
    {
        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
        var competition = await competitionsService.GetCompetitionWithRecipients(internalOrgId, competitionId);
        var recipients = await GetServiceRecipients(internalOrgId);
        var splitRecipientIds = string.Join(',', recipientIds, importedRecipients)
            .Split(
                ',',
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        const string pageAdvice =
            "Select the organisations that will receive the winning solution for this competition or upload them using a CSV file.";

        var model = new SelectRecipientsModel(
            organisation,
            recipients,
            competition.Recipients.Select(x => x.Id),
            [],
            splitRecipientIds,
            selectionMode)
        {
            Title = "Service Recipients",
            BackLink = Url.Action(
                nameof(UploadOrSelectServiceRecipients),
                typeof(CompetitionRecipientsController).ControllerName(),
                new { internalOrgId, competitionId }),
            Caption = competition.Name,
            Advice = pageAdvice,
            HasImportedRecipients = !string.IsNullOrWhiteSpace(importedRecipients),
        };

        return View("ServiceRecipients/SelectRecipients", model);
    }

    [HttpPost]
    public IActionResult Index(string internalOrgId, int competitionId, SelectRecipientsModel model)
    {
        if (ModelState.IsValid)
        {
            return RedirectToAction(
                nameof(ConfirmRecipients),
                new
                {
                    internalOrgId,
                    competitionId,
                    recipientIds = string.Join(',', model.GetSelectedServiceRecipients().Select(x => x.OdsCode)),
                });
        }

        model.ShouldExpand = true;
        return View("ServiceRecipients/SelectRecipients", model);
    }

    [HttpGet("confirm-recipients")]
    public async Task<IActionResult> ConfirmRecipients(
        string internalOrgId,
        int competitionId,
        string recipientIds,
        bool? hasImported = null)
    {
        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
        var competition = await competitionsService.GetCompetition(internalOrgId, competitionId);

        var recipientOdsCodes = recipientIds.Split(
            ',',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var recipients = await odsService.GetServiceRecipientsById(internalOrgId, recipientOdsCodes);

        var model = new ConfirmChangesModel(organisation)
        {
            BackLink = hasImported.GetValueOrDefault()
                ? Url.Action(
                    nameof(CompetitionImportServiceRecipientsController.Index),
                    typeof(CompetitionImportServiceRecipientsController).ControllerName(),
                    new { internalOrgId, competitionId })
                : Url.Action(nameof(Index), new { internalOrgId, competitionId, recipientIds }),
            Caption = competition.Name,
            Selected = recipients.Select(
                    x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OrgId, Location = x.Location })
                .ToList(),
            Advice = ConfirmRecipientsAdvice,
        };

        return View("ServiceRecipients/ConfirmChanges", model);
    }

    [HttpPost("confirm-recipients")]
    public async Task<IActionResult> ConfirmRecipients(
        string internalOrgId,
        int competitionId,
        ConfirmChangesModel model)
    {
        await competitionsService.SetCompetitionRecipients(competitionId, model.Selected.Select(x => x.OdsCode));

        return RedirectToAction(
            nameof(CompetitionTaskListController.Index),
            typeof(CompetitionTaskListController).ControllerName(),
            new { internalOrgId, competitionId });
    }

    private async Task<List<ServiceRecipientModel>> GetServiceRecipients(string internalOrgId)
    {
        var recipients = await odsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId);

        return recipients
            .OrderBy(x => x.Name)
            .Select(x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OrgId, Location = x.Location, })
            .ToList();
    }

    private async Task<List<ServiceRecipientModel>> GetServiceRecipientsBySublocation(string internalOrgId)
    {
        IEnumerable<ServiceRecipient> recipients = await odsService.GetServiceRecipientsBySublocation(internalOrgId);

        return recipients
            .OrderBy(x => x.Name)
            .Select(x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OrgId, Location = x.Location })
            .ToList();
    }

    private async Task<IActionResult> SelectSublocationsOverview(
        string internalOrgId,
        int competitionId,
        bool isConfirm,
        string backlink)
    {
        Competition competition =
            await competitionsService.GetCompetitionWithSublocations(internalOrgId, competitionId);

        var sublocations = new List<SublocationModel>();

        foreach (CompetitionSublocation s in competition.CompetitionSublocations)
        {
            {
                var sublocationModel = new SublocationModel
                {
                    Name = s.SublocationOrganisation.Name,
                    ServiceRecipientCount = await competitionsService.GetCountForCompetitionSublocationRecipients(
                        internalOrgId,
                        competitionId,
                        s.SublocationOdsCode),
                    OdsCode = s.SublocationOdsCode,
                    RecipientHref = Url.Action(
                        nameof(SelectSublocationRecipients),
                        typeof(CompetitionRecipientsController).ControllerName(),
                        new { internalOrgId, competitionId, sublocationId = s.SublocationOdsCode }),
                };
                sublocations.Add(sublocationModel);
            }
        }

        var addOrChangeSublocationsHref = "";

        var model = new SelectSublocationsOverviewModel(
            isConfirm,
            competition,
            sublocations,
            addOrChangeSublocationsHref) { BackLink = backlink };

        return View("ServiceRecipients/SelectSublocationsOverview", model);
    }
}
