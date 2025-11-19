using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/select-recipients")]
public class CompetitionRecipientsController(
    IOrganisationsService organisationsService,
    ICompetitionsService competitionsService,
    ICompetitionSublocationService competitionSublocationService,
    IOdsService odsService)
    : Controller
{
    internal const string ConfirmRecipientsAdvice =
        "Review the organisations you’ve selected to receive the winning solution for this competition.";

    private readonly IOrganisationsService organisationsService =
        organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));

    private readonly ICompetitionsService competitionsService =
        competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));

    private readonly ICompetitionSublocationService competitionSublocationService = competitionSublocationService
        ?? throw new ArgumentNullException(nameof(competitionSublocationService));

    private readonly IOdsService odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));

    [HttpGet("upload-or-select-service-recipients")]
    public async Task<IActionResult> UploadOrSelectServiceRecipients(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetition(internalOrgId, competitionId);

        if (competition is null)
        {
            return NotFound();
        }

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
    public async Task<IActionResult> UploadOrSelectServiceRecipients(
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

        var competitionHasSublocations =
            await competitionsService.GetCompetitionHasAnySublocations(internalOrgId, competitionId);

        return RedirectToAction(
            competitionHasSublocations ? nameof(ConfirmSublocations) : nameof(SelectSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });
    }

    [HttpGet("select-sublocations")]
    public async Task<IActionResult> SelectSublocations(
        string internalOrgId,
        int competitionId)
    {
        Competition competition =
            await competitionsService.GetCompetitionWithSublocations(internalOrgId, competitionId);

        if (competition is null)
        {
            return NotFound();
        }

        IEnumerable<OdsOrganisation> possibleSublocations =
            await odsService.GetSublocationsByParentOdsCode(competition.Organisation.ExternalIdentifier);

        var backLink = Url.Action(
            nameof(UploadOrSelectServiceRecipients),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });

        var model = new SelectSublocationsModel(
            competition,
            possibleSublocations,
            backLink);
        return View("ServiceRecipients/SelectSublocations", model);
    }

    [HttpPost("select-sublocations")]
    public async Task<IActionResult> SelectSublocations(
        SelectSublocationsModel selectSublocations,
        string internalOrgId,
        int competitionId)
    {
        if (!ModelState.IsValid)
        {
            return View("ServiceRecipients/SelectSublocations", selectSublocations);
        }

        HashSet<string> sublocationOdsCodes =
            selectSublocations.RenderedSublocations.Where(x => x.Selected).Select(y => y.Value).ToHashSet();

        Competition competition =
            await competitionsService.GetCompetitionWithSublocations(internalOrgId, competitionId);

        HashSet<string> competitionSublocations =
            competition.CompetitionSublocations.Select(x => x.SublocationOdsCode).ToHashSet();

        HashSet<string> removes = competitionSublocations.Except(sublocationOdsCodes).ToHashSet();

        var stringOfRemoves = JoinEnumerableStringsToCommaSeparatedString(removes);

        if (removes.Count > 0)
        {
            var stringOfSublocations = JoinEnumerableStringsToCommaSeparatedString(sublocationOdsCodes);

            return RedirectToAction(
                nameof(RemoveSublocations),
                typeof(CompetitionRecipientsController).ControllerName(),
                new
                {
                    internalOrgId, competitionId, sublocations = stringOfSublocations, removes = stringOfRemoves,
                });
        }

        await competitionsService.SetSublocations(internalOrgId, competitionId, sublocationOdsCodes);

        return RedirectToAction(
            nameof(ConfirmSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });
    }

    [HttpGet("remove-sublocations")]
    public async Task<IActionResult> RemoveSublocations(
        string internalOrgId,
        int competitionId,
        string sublocations,
        string removes)
    {
        var parsedSublocations = SplitCommaSeparatedString(sublocations);

        var parsedRemoves = SplitCommaSeparatedString(removes);

        Competition competition =
            await competitionsService.GetCompetition(internalOrgId, competitionId);

        if (competition is null)
        {
            return NotFound();
        }

        var backLink = Url.Action(
            nameof(ConfirmSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });

        var model = new RemoveSublocationsModel(
            competition,
            parsedSublocations,
            parsedRemoves,
            backLink);

        return View("ServiceRecipients/RemoveSublocations", model);
    }

    [HttpPost("remove-sublocations")]
    public async Task<IActionResult> RemoveSublocations(
        RemoveSublocationsModel removeSublocationsModel,
        string internalOrgId,
        int competitionId)
    {
        if (removeSublocationsModel.ConfirmRemove is null || removeSublocationsModel.SublocationOdsCodes is not
                { Count: > 0 })
        {
            return BadRequest();
        }

        if (removeSublocationsModel.ConfirmRemove is false)
        {
            return RedirectToAction(
                nameof(ConfirmSublocations),
                typeof(CompetitionRecipientsController).ControllerName(),
                new { internalOrgId, competitionId });
        }

        HashSet<string> sublocations = removeSublocationsModel.SublocationOdsCodes.ToHashSet();

        await competitionsService.SetSublocations(
                internalOrgId,
                competitionId,
                sublocations);

        return RedirectToAction(
            nameof(ConfirmSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });
    }

    [HttpGet("{sublocationOdsCode}")]
    public async Task<IActionResult> SelectSublocationRecipients(
        string internalOrgId,
        int competitionId,
        string sublocationOdsCode,
        SelectionMode? selectionMode = null)
    {
        var externalOrganisationId =
            await organisationsService.GetOrganisationExternalIdentifierByInternalIdentifier(internalOrgId);

        if (externalOrganisationId is null)
        {
            return NotFound();
        }

        CompetitionSublocation competitionSublocation =
            await competitionSublocationService.GetCompetitionSublocationWithRecipients(
                externalOrganisationId,
                competitionId,
                sublocationOdsCode);

        if (competitionSublocation is null)
        {
            return NotFound();
        }

        var sublocationAsSublocationModel = new SublocationModel(competitionSublocation, true);

        List<ServiceRecipientModel> possibleRecipients =
            await GetServiceRecipientModelsBySublocation(sublocationOdsCode);

        var backLink = Url.Action(
            nameof(ConfirmSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });

        var model = new SelectSublocationRecipientsModel(
            competitionSublocation.Competition,
            sublocationAsSublocationModel,
            possibleRecipients,
            backLink,
            selectionMode);

        return View("ServiceRecipients/SelectSublocationRecipients", model);
    }

    [HttpPost("{sublocationOdsCode}")]
    public async Task<IActionResult> SelectSublocationRecipients(
        SelectSublocationRecipientsModel selectSublocationRecipientsModel,
        string internalOrgId,
        int competitionId,
        string sublocationOdsCode)
    {
        if (!ModelState.IsValid)
        {
            return View("ServiceRecipients/SelectSublocationRecipients", selectSublocationRecipientsModel);
        }

        var externalOrganisationId =
            await organisationsService.GetOrganisationExternalIdentifierByInternalIdentifier(internalOrgId);

        if (externalOrganisationId is null)
        {
            return BadRequest();
        }

        CompetitionSublocation sublocation =
            await competitionSublocationService.GetCompetitionSublocationWithRecipients(
                externalOrganisationId,
                competitionId,
                sublocationOdsCode);

        if (sublocation is null)
        {
            return BadRequest();
        }

        HashSet<string> pageSelections = selectSublocationRecipientsModel.RenderedServiceRecipients
            .Where(x => x.Selected)
            .Select(y => y.Value)
            .ToHashSet();

        await competitionSublocationService.SetSublocationRecipients(
            externalOrganisationId,
            competitionId,
            sublocationOdsCode,
            pageSelections);

        return RedirectToAction(
            nameof(ConfirmSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });
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
    public IActionResult ConfirmSublocations(
        SelectSublocationsOverviewModel model,
        string internalOrgId,
        int competitionId)
    {
        return SelectSublocationsOverviewDynamicRedirect(model, internalOrgId, competitionId);
    }

    [HttpGet("confirm-sublocation-recipients")]
    public async Task<IActionResult> ConfirmSublocationRecipients(
        string internalOrgId,
        int competitionId)
    {
        Competition competition =
            await competitionsService.GetCompetitionWithSublocationsAndSublocationRecipients(
                internalOrgId,
                competitionId);

        if (competition is null)
        {
            return NotFound();
        }

        var backLinkUrl = Url.Action(
            nameof(ConfirmSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });

        var continueLinkUrl = Url.Action(
            nameof(CompetitionTaskListController.Index),
            typeof(CompetitionTaskListController).ControllerName(),
            new { internalOrgId, competitionId });

        var model = new ConfirmSublocationRecipientsModel(
            competition,
            backLinkUrl,
            continueLinkUrl);

        return View("ServiceRecipients/ConfirmSublocationRecipients", model);
    }

    [HttpPost("confirm-sublocation-recipients")]
    public IActionResult ConfirmSublocationRecipientsPost(
        string internalOrgId,
        int competitionId)
    {
        return RedirectToAction(
            nameof(CompetitionTaskListController.Index),
            typeof(CompetitionTaskListController).ControllerName(),
            new { internalOrgId, competitionId });
    }

    private static string[] SplitCommaSeparatedString(string sublocationsToRemove)
    {
        return sublocationsToRemove?.Split(
            ',',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
    }

    private static string JoinEnumerableStringsToCommaSeparatedString(IEnumerable<string> stringEnumerable)
    {
        return string.Join(",", stringEnumerable);
    }

    private async Task<List<ServiceRecipientModel>> GetServiceRecipientModelsBySublocation(string sublocationOdsCode)
    {
        IEnumerable<ServiceRecipient> recipients =
            await odsService.GetServiceRecipientsBySublocation(sublocationOdsCode);

        return recipients
            .Select(x => new ServiceRecipientModel(x))
            .ToList();
    }

    private async Task<IActionResult> SelectSublocationsOverview(
        string internalOrgId,
        int competitionId,
        bool isConfirm,
        string backLink)
    {
        Competition competition =
            await competitionsService.GetCompetitionWithSublocations(internalOrgId, competitionId);

        if (competition is null)
        {
            return NotFound();
        }

        var sublocations = new List<SublocationModel>();

        foreach (CompetitionSublocation s in competition.CompetitionSublocations)
        {
            await MapSublocationToSublocationModel(s);
        }

        var addOrChangeSublocationsLink = Url.Action(
            nameof(SelectSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });

        var model = new SelectSublocationsOverviewModel(
            isConfirm,
            competition,
            sublocations,
            addOrChangeSublocationsLink,
            backLink);

        return View("ServiceRecipients/SelectSublocationsOverview", model);

        async Task MapSublocationToSublocationModel(CompetitionSublocation competitionSublocation)
        {
            var recipientLink = Url.Action(
                nameof(SelectSublocationRecipients),
                typeof(CompetitionRecipientsController).ControllerName(),
                new { internalOrgId, competitionId, sublocationOdsCode = competitionSublocation.SublocationOdsCode });

            var serviceRecipientCount =
                await competitionSublocationService.GetCountForCompetitionSublocationRecipients(
                    competition.Organisation.ExternalIdentifier,
                    competitionId,
                    competitionSublocation.SublocationOdsCode);

            TaskProgress taskProgress = serviceRecipientCount == 0 ? TaskProgress.NotStarted : TaskProgress.Completed;

            var sublocationModel = new SublocationModel(
                competitionSublocation,
                recipientLink,
                serviceRecipientCount,
                taskProgress);
            sublocations.Add(sublocationModel);
        }
    }

    private RedirectToActionResult SelectSublocationsOverviewDynamicRedirect(
        SelectSublocationsOverviewModel model,
        string internalOrgId,
        int competitionId)
    {
        var sublocationToComplete = model.Sublocations.Any(x => x.ServiceRecipientCount == 0);

        if (sublocationToComplete)
        {
            return RedirectToAction(
                nameof(CompetitionTaskListController.Index),
                typeof(CompetitionTaskListController).ControllerName(),
                new { internalOrgId, competitionId });
        }

        return RedirectToAction(
            nameof(ConfirmSublocationRecipients),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });
    }
}
