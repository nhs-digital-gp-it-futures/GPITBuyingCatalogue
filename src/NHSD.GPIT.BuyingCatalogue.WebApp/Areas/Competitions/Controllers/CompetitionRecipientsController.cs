using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}")]
public class CompetitionRecipientsController : Controller
{
    internal const string ConfirmRecipientsAdvice =
        "Review the organisations you’ve selected to receive the winning solution for this competition.";

    private readonly IOrganisationsService organisationsService;
    private readonly ICompetitionsService competitionsService;
    private readonly IOdsService odsService;

    public CompetitionRecipientsController(
        IOrganisationsService organisationsService,
        ICompetitionsService competitionsService,
        IOdsService odsService)
    {
        this.organisationsService =
            organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
        this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
    }

    [HttpGet("select-recipients")]
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
            Enumerable.Empty<string>(),
            splitRecipientIds,
            selectionMode)
        {
            Title = "Service Recipients",
            BackLink = Url.Action(
                nameof(CompetitionTaskListController.Index),
                typeof(CompetitionTaskListController).ControllerName(),
                new { internalOrgId, competitionId }),
            Caption = competition.Name,
            Advice = pageAdvice,
            ImportRecipientsLink = Url.Action(
                nameof(CompetitionImportServiceRecipientsController.Index),
                typeof(CompetitionImportServiceRecipientsController).ControllerName(),
                new { internalOrgId, competitionId }),
            HasImportedRecipients = !string.IsNullOrWhiteSpace(importedRecipients),
        };

        return View("ServiceRecipients/SelectRecipients", model);
    }

    [HttpPost("select-recipients")]
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
        string recipientIds)
    {
        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
        var competition = await competitionsService.GetCompetition(internalOrgId, competitionId);

        var recipientOdsCodes = recipientIds.Split(
            ',',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var recipients = await odsService.GetServiceRecipientsById(internalOrgId, recipientOdsCodes);

        var model = new ConfirmChangesModel(organisation)
        {
            BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId, recipientIds }),
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
}
