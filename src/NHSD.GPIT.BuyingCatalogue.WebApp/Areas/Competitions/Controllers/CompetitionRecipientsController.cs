using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
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

        IEnumerable<OdsOrganisation> possibleSublocations =
            await odsService.GetSublocationsByParentOdsCode(competition.Organisation.ExternalIdentifier);

        var backLinkHref = Url.Action(
            nameof(UploadOrSelectServiceRecipients),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });

        var model = new SelectSublocationsModel(
            competition,
            possibleSublocations,
            backLinkHref);
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

        HashSet<string> sublocationIds =
            selectSublocations.RenderedSublocations.Where(x => x.Value).Select(y => y.Name).ToHashSet();

        Competition competition =
            await competitionsService.GetCompetitionWithSublocations(internalOrgId, competitionId);

        HashSet<string> competitionSublocations =
            competition.CompetitionSublocations.Select(x => x.SublocationOdsCode).ToHashSet();

        if (competitionSublocations.Count == 0)
        {
            await competitionsService.AddSublocations(internalOrgId, competitionId, sublocationIds);

            return RedirectToAction(
                nameof(AddSublocations),
                typeof(CompetitionRecipientsController).ControllerName(),
                new { internalOrgId, competitionId });
        }

        HashSet<string> removes = [..competitionSublocations];
        removes.ExceptWith(sublocationIds);

        HashSet<string> adds = [..sublocationIds];
        adds.ExceptWith(competitionSublocations);

        var stringOfRemoves = JoinEnumerableStringsToCommaSeparatedString(removes);

        var stringOfAdds = JoinEnumerableStringsToCommaSeparatedString(adds);

        if (removes.Count > 0)
        {
            return RedirectToAction(
                nameof(RemoveSublocations),
                typeof(CompetitionRecipientsController).ControllerName(),
                new
                {
                    internalOrgId,
                    competitionId,
                    sublocationsToRemove = stringOfRemoves,
                    sublocationsToAdd = stringOfAdds,
                });
        }

        if (adds.Count > 0)
        {
            await competitionsService.AddSublocations(internalOrgId, competitionId, adds);
        }

        return RedirectToAction(
            nameof(ConfirmSublocations),
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
    public IActionResult AddSublocations(
        SelectSublocationsOverviewModel model,
        string internalOrgId,
        int competitionId)
    {
        return SelectSublocationsOverviewDynamicRedirect(model, internalOrgId, competitionId);
    }

    [HttpGet("remove-sublocations")]
    public async Task<IActionResult> RemoveSublocations(
        string internalOrgId,
        int competitionId,
        string sublocationsToRemove,
        string sublocationsToAdd)
    {
        var splitSublocationsToRemove = SplitCommaSeparatedString(sublocationsToRemove);

        var splitSublocationsToAdd = SplitCommaSeparatedString(sublocationsToAdd);

        Competition competition =
            await competitionsService.GetCompetition(internalOrgId, competitionId);

        var backLinkHref = Url.Action(
            nameof(ConfirmSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });

        var model = new RemoveSublocationsModel(
            competition,
            splitSublocationsToRemove,
            splitSublocationsToAdd,
            backLinkHref);

        return View("ServiceRecipients/RemoveSublocations", model);
    }

    [HttpPost("remove-sublocations")]
    public async Task<IActionResult> RemoveSublocations(
        RemoveSublocationsModel removeSublocationsModel,
        string internalOrgId,
        int competitionId)
    {
        if (removeSublocationsModel.ConfirmRemove)
        {
            HashSet<string> adds = removeSublocationsModel.SublocationIdsToAdd?.ToHashSet();
            HashSet<string> removes = removeSublocationsModel.SublocationIdsToRemove?.ToHashSet();

            if (adds is not null && adds.Count > 0)
            {
                await competitionsService.AddSublocations(
                    internalOrgId,
                    competitionId,
                    adds);
            }

            if (removes is not null && removes.Count > 0)
            {
                await competitionsService.RemoveSublocations(
                    internalOrgId,
                    competitionId,
                    removes);
            }
        }

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

        CompetitionSublocation competitionSublocation =
            await competitionSublocationService.GetCompetitionSublocationWithRecipients(
                externalOrganisationId,
                competitionId,
                sublocationOdsCode);

        var sublocationAsSublocationModel = new SublocationModel(competitionSublocation, true);

        List<ServiceRecipientModel> possibleRecipients =
            await GetServiceRecipientModelsBySublocation(sublocationOdsCode);

        var backLinkHref = Url.Action(
            nameof(ConfirmSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });

        var model = new SelectSublocationRecipientsModel(
            competitionSublocation.Competition,
            sublocationAsSublocationModel,
            possibleRecipients,
            backLinkHref,
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

        CompetitionSublocation sublocation =
            await competitionSublocationService.GetCompetitionSublocationWithRecipients(
                externalOrganisationId,
                competitionId,
                sublocationOdsCode);

        HashSet<string> pageSelections = selectSublocationRecipientsModel.RenderedServiceRecipients
            .Where(x => x.Selected)
            .Select(y => y.OdsCode)
            .ToHashSet();

        HashSet<string> currentRecipients =
            sublocation.SublocationRecipients?.Select(x => x.RecipientOdsCode).ToHashSet() ?? [];

        HashSet<string> adds = [..pageSelections];
        adds.ExceptWith(currentRecipients);

        HashSet<string> removes = [..currentRecipients];
        removes.ExceptWith(pageSelections);

        if (adds.Count > 0)
        {
            await competitionSublocationService.AddSublocationRecipients(
                externalOrganisationId,
                competitionId,
                sublocationOdsCode,
                adds);
        }

        if (removes.Count > 0)
        {
            await competitionSublocationService.RemoveSublocationRecipients(
                externalOrganisationId,
                competitionId,
                sublocationOdsCode,
                removes);
        }

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
    public IActionResult ConfirmSublocationsPost(
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
        string backLinkHref)
    {
        Competition competition =
            await competitionsService.GetCompetitionWithSublocations(internalOrgId, competitionId);

        var sublocations = new List<SublocationModel>();

        foreach (CompetitionSublocation s in competition.CompetitionSublocations)
        {
            {
                var recipientHref = Url.Action(
                    nameof(SelectSublocationRecipients),
                    typeof(CompetitionRecipientsController).ControllerName(),
                    new { internalOrgId, competitionId, sublocationOdsCode = s.SublocationOdsCode });

                var serviceRecipientCount =
                    await competitionSublocationService.GetCountForCompetitionSublocationRecipients(
                        competition.Organisation.ExternalIdentifier,
                        competitionId,
                        s.SublocationOdsCode);

                var sublocationModel = new SublocationModel(s, recipientHref, serviceRecipientCount);
                sublocations.Add(sublocationModel);
            }
        }

        var addOrChangeSublocationsHref = Url.Action(
            nameof(SelectSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });

        var model = new SelectSublocationsOverviewModel(
            isConfirm,
            competition,
            sublocations,
            addOrChangeSublocationsHref,
            backLinkHref);

        return View("ServiceRecipients/SelectSublocationsOverview", model);
    }

    private IActionResult SelectSublocationsOverviewDynamicRedirect(
        SelectSublocationsOverviewModel model,
        string internalOrgId,
        int competitionId)
    {
        SublocationModel sublocationToComplete = model.Sublocations.FirstOrDefault(x => x.ServiceRecipientCount == 0);

        if (sublocationToComplete != null)
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
}
