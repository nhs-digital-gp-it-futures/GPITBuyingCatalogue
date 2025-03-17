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
using NHSD.GPIT.BuyingCatalogue.WebApp.FormContent;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using stringDict = System.Collections.Generic.Dictionary<string, string>;

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
            await odsService.GetSublocationsByParentInternalIdentifier(internalOrgId);

        var model = new SelectSublocationsModel(competition, possibleSublocations)
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
        [FilteredFormContent] stringDict form,
        string internalOrgId,
        int competitionId,
        bool isInitialSelection)
    {
        HashSet<string> sublocationIds = form.Keys.ToHashSet();

        if (isInitialSelection)
        {
            await competitionsService.AddSublocations(internalOrgId, competitionId, sublocationIds);

            return RedirectToAction(
                nameof(AddSublocations),
                typeof(CompetitionRecipientsController).ControllerName(),
                new { internalOrgId, competitionId });
        }

        Competition competition =
            await competitionsService.GetCompetitionWithSublocations(internalOrgId, competitionId);

        HashSet<string> competitionSublocations =
            competition.CompetitionSublocations.Select(x => x.SublocationOdsCode).ToHashSet();

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
        string sublocationsToRemove,
        string sublocationsToAdd)
    {
        var splitSublocationsToRemove = SplitCommaSeparatedString(sublocationsToRemove);

        var splitSublocationsToAdd = SplitCommaSeparatedString(sublocationsToAdd);

        Competition competition =
            await competitionsService.GetCompetitionWithSublocations(internalOrgId, competitionId);

        var model = new RemoveSublocationsModel(competition)
        {
            BackLink = Url.Action(
                nameof(ConfirmSublocations),
                typeof(CompetitionRecipientsController).ControllerName(),
                new { internalOrgId, competitionId }),
            SublocationIdsToRemove = splitSublocationsToRemove.ToHashSet(),
            SublocationIdsToAdd = splitSublocationsToAdd.ToHashSet(),
        };

        return View("ServiceRecipients/RemoveSublocations", model);
    }

    [HttpPost("remove-sublocations")]
    public async Task<IActionResult> RemoveSublocations(
        [FilteredFormContent] stringDict form,
        string internalOrgId,
        int competitionId)
    {
        const string confirmationKey = "ConfirmRemove";
        const string removePrefix = "remove";
        const string addPrefix = "add";

        var userHasConfirmed = form[confirmationKey] == "True";

        if (userHasConfirmed)
        {
            HashSet<string> removes = form.Where(kvp => kvp.Key.ToString().StartsWith(removePrefix))
                .Select(kvp => kvp.Value)
                .ToHashSet();

            HashSet<string> adds = form.Where(kvp => kvp.Key.ToString().StartsWith(addPrefix))
                .Select(kvp => kvp.Value)
                .ToHashSet();

            if (adds.Count > 0)
            {
                await competitionsService.AddSublocations(internalOrgId, competitionId, adds);
            }

            if (removes.Count > 0)
            {
                await competitionsService.RemoveSublocations(internalOrgId, competitionId, removes);
            }
        }

        return RedirectToAction(
            nameof(ConfirmSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });
    }

    [HttpGet("{sublocationId}")]
    public async Task<IActionResult> SelectSublocationRecipients(
        string internalOrgId,
        int competitionId,
        string sublocationId,
        string recipientIds,
        string importedRecipients,
        SelectionMode? selectionMode = null)
    {
        Competition competition = await competitionsService.GetCompetition(internalOrgId, competitionId);
        CompetitionSublocation competitionSublocation =
            await competitionSublocationService.GetCompetitionSublocationWithRecipients(
                competition.Organisation.ExternalIdentifier,
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
        var splitRecipientIds = SplitCommaSeparatedString(string.Join(',', recipientIds, importedRecipients));

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

    [HttpPost("{sublocationId}")]
    public async Task<IActionResult> SelectSublocationRecipients(
        [FilteredFormContent] stringDict form,
        string internalOrgId,
        int competitionId,
        string sublocationId)
    {
        const string selectPrefix = "recipient";
        const int odsCodeSplitIndex = 1;

        Competition competition =
            await competitionsService.GetCompetitionWithSublocations(internalOrgId, competitionId);

        CompetitionSublocation sublocation =
            await competitionSublocationService.GetCompetitionSublocationWithRecipients(
                competition.Organisation.ExternalIdentifier,
                competitionId,
                sublocationId);

        HashSet<string> pageSelections = form.Where(kvp => kvp.Key.ToString().StartsWith(selectPrefix))
            .Select(kvp => kvp.Key.Split('-')[odsCodeSplitIndex])
            .ToHashSet();

        HashSet<string> currentRecipients =
            sublocation.SublocationRecipients.Select(x => x.RecipientOdsCode).ToHashSet();

        HashSet<string> adds = [..pageSelections];
        adds.ExceptWith(currentRecipients);

        HashSet<string> removes = [..currentRecipients];
        removes.ExceptWith(pageSelections);

        if (adds.Count > 0)
        {
            await competitionSublocationService.AddSublocationRecipients(
                competition.Organisation.ExternalIdentifier,
                competitionId,
                sublocationId,
                adds);
        }

        if (removes.Count > 0)
        {
            await competitionSublocationService.RemoveSublocationRecipients(
                competition.Organisation.ExternalIdentifier,
                competitionId,
                sublocationId,
                removes);
        }

        return RedirectToAction(
            nameof(ConfirmSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });
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
        var splitRecipientIds = SplitCommaSeparatedString(string.Join(',', recipientIds, importedRecipients));

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

        var recipientOdsCodes = SplitCommaSeparatedString(recipientIds);

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

        var addOrChangeSublocationsHref = Url.Action(
            nameof(SelectSublocations),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });

        var model = new SelectSublocationsOverviewModel(
            isConfirm,
            competition,
            sublocations,
            addOrChangeSublocationsHref) { BackLink = backlink };

        return View("ServiceRecipients/SelectSublocationsOverview", model);
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
