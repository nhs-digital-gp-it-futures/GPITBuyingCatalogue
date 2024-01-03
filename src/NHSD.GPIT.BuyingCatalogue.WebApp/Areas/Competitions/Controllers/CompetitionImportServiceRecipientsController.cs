using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;
using ServiceRecipient = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceRecipient;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/import-recipients")]
public class CompetitionImportServiceRecipientsController : Controller
{
    internal const int OdsCodeLength = 8;
    internal const int OrganisationNameLength = 256;
    internal const string InvalidFormat = "The selected file does not meet the required format";
    internal const string EmptyFile = "The selected file is empty";

    internal static readonly string OdsCodeExceedsLimit =
        $"At least one of your ODS codes is more than {OdsCodeLength} characters";

    internal static readonly string OrganisationExceedsLimit =
        $"At least one of your Service Recipient names is more than {OrganisationNameLength} characters";

    private const string CompetitionCacheKey = "competitions";
    private readonly IServiceRecipientImportService importService;
    private readonly ICompetitionsService competitionsService;
    private readonly IOdsService odsService;

    public CompetitionImportServiceRecipientsController(
        IServiceRecipientImportService importService,
        ICompetitionsService competitionsService,
        IOdsService odsService)
    {
        this.importService = importService ?? throw new ArgumentNullException(nameof(importService));
        this.competitionsService =
            competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
        this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string internalOrgId,
        int competitionId)
    {
        await importService.Clear(new(User.UserId(), internalOrgId, CompetitionCacheKey, competitionId));

        var competitionName = await competitionsService.GetCompetitionName(internalOrgId, competitionId);
        var model = new ImportServiceRecipientModel
        {
            BackLink = Url.Action(
                nameof(CompetitionRecipientsController.Index),
                typeof(CompetitionRecipientsController).ControllerName(),
                new { internalOrgId, competitionId }),
            Caption = competitionName,
            DownloadTemplateLink = Url.Action(nameof(DownloadTemplate), new { internalOrgId, competitionId }),
        };

        return View("ServiceRecipients/ImportServiceRecipients/Index", model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(
        string internalOrgId,
        int competitionId,
        ImportServiceRecipientModel model)
    {
        if (!ModelState.IsValid)
            return View("ServiceRecipients/ImportServiceRecipients/Index", model);

        IList<ServiceRecipientImportModel> importedServiceRecipients =
            await importService.ReadFromStream(model.File.OpenReadStream());
        var (validatedSuccessfully, error) = ValidateServiceRecipients(importedServiceRecipients);

        if (!validatedSuccessfully)
        {
            ModelState.AddModelError(nameof(model.File), error);
            return View("ServiceRecipients/ImportServiceRecipients/Index", model);
        }

        await importService.Store(
            new(
                User.UserId(), internalOrgId, CompetitionCacheKey, competitionId),
            importedServiceRecipients);

        return RedirectToAction(
            nameof(ValidateOds),
            new { internalOrgId, competitionId });
    }

    [HttpGet("validate-ods")]
    public async Task<IActionResult> ValidateOds(
        string internalOrgId,
        int competitionId)
    {
        var cacheKey = new ServiceRecipientCacheKey(User.UserId(), internalOrgId, CompetitionCacheKey, competitionId);
        var cachedRecipients = await importService.GetCached(cacheKey);
        if (cachedRecipients is null)
            return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });

        var organisationServiceRecipients =
            await odsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId);

        var mismatchedOdsCodes = GetMismatchedOdsCodes(cachedRecipients, organisationServiceRecipients).ToList();
        if (mismatchedOdsCodes.Any())
        {
            var competitionName = await competitionsService.GetCompetitionName(internalOrgId, competitionId);
            var model = new ValidateOdsModel(
                mismatchedOdsCodes)
            {
                BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId }),
                Caption = competitionName,
                CancelLink = Url.Action(nameof(CancelImport), new { internalOrgId, competitionId }),
                ValidateNamesLink = Url.Action(
                    nameof(ValidateNames),
                    new { internalOrgId, competitionId }),
            };

            return View("ServiceRecipients/ImportServiceRecipients/ValidateOds", model);
        }

        return RedirectToAction(
            nameof(ValidateNames),
            new
            {
                internalOrgId, competitionId,
            });
    }

    [HttpGet("validate-names")]
    public async Task<IActionResult> ValidateNames(
        string internalOrgId,
        int competitionId)
    {
        var cacheKey = new ServiceRecipientCacheKey(User.UserId(), internalOrgId, CompetitionCacheKey, competitionId);
        var cachedRecipients = await importService.GetCached(cacheKey);
        if (cachedRecipients is null)
            return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });

        var organisationServiceRecipients =
            (await odsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId)).ToList();

        var mismatchedRecipients = GetMismatchedNames(cachedRecipients.ToList(), organisationServiceRecipients);
        if (mismatchedRecipients.Any())
        {
            var competitionName = await competitionsService.GetCompetitionName(internalOrgId, competitionId);
            var model = new ValidateNamesModel(mismatchedRecipients)
            {
                BackLink = Url.Action(
                    GetNameValidationBacklink(cachedRecipients, organisationServiceRecipients),
                    new { internalOrgId, competitionId }),
                CancelLink = Url.Action(nameof(CancelImport), new { internalOrgId, competitionId }),
                Caption = competitionName,
            };
            return View("ServiceRecipients/ImportServiceRecipients/ValidateNames", model);
        }

        var validOdsCodes = GetValidOdsCodes(cachedRecipients, organisationServiceRecipients);
        await importService.Clear(cacheKey);
        return RedirectToAction(
            nameof(CompetitionRecipientsController.Index),
            typeof(CompetitionRecipientsController).ControllerName(),
            new
            {
                internalOrgId,
                competitionId,
                importedRecipients = string.Join(',', validOdsCodes),
            });
    }

    [HttpPost("validate-names")]
    public async Task<IActionResult> ValidateNames(
        string internalOrgId,
        int competitionId,
        ValidateNamesModel model)
    {
        var cacheKey = new ServiceRecipientCacheKey(User.UserId(), internalOrgId, CompetitionCacheKey, competitionId);
        var cachedRecipients = await importService.GetCached(cacheKey);
        var organisationServiceRecipients =
            await odsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId);

        var validOdsCodes = GetValidOdsCodes(cachedRecipients, organisationServiceRecipients);

        await importService.Clear(cacheKey);

        return RedirectToAction(
            nameof(CompetitionRecipientsController.Index),
            typeof(CompetitionRecipientsController).ControllerName(),
            new
            {
                internalOrgId, competitionId, importedRecipients = string.Join(',', validOdsCodes),
            });
    }

    [HttpGet("download-template")]
    public async Task<IActionResult> DownloadTemplate(
        string internalOrgId,
        int competitionId)
    {
        _ = internalOrgId;
        _ = competitionId;

        using var stream = new MemoryStream();
        await importService.CreateServiceRecipientTemplate(stream);
        stream.Position = 0;

        return File(stream.ToArray(), "application/octet-stream", "service_recipient_template.csv");
    }

    [HttpGet("cancel-import")]
    public IActionResult CancelImport(
        string internalOrgId,
        int competitionId)
    {
        importService.Clear(new(User.UserId(), internalOrgId, CompetitionCacheKey, competitionId));

        return RedirectToAction(
            nameof(CompetitionRecipientsController.Index),
            typeof(CompetitionRecipientsController).ControllerName(),
            new { internalOrgId, competitionId });
    }

    private static List<ServiceRecipientImportModel> GetMismatchedOdsCodes(
        IEnumerable<ServiceRecipientImportModel> importedServiceRecipients,
        IEnumerable<ServiceRecipient> serviceRecipients)
        => importedServiceRecipients.Where(
            r => serviceRecipients.All(
                x => !string.Equals(x.OrgId, r.OdsCode, StringComparison.OrdinalIgnoreCase))).ToList();

    private static List<(string Expected, string Actual, string OdsCode)> GetMismatchedNames(
        List<ServiceRecipientImportModel> importedServiceRecipients,
        List<ServiceRecipient> serviceRecipients)
    {
        return (from importedRecipient in importedServiceRecipients
            from serviceRecipient in serviceRecipients
            where string.Equals(
                importedRecipient.OdsCode,
                serviceRecipient.OrgId,
                StringComparison.OrdinalIgnoreCase)
            where !string.Equals(
                importedRecipient.Organisation,
                serviceRecipient.Name,
                StringComparison.OrdinalIgnoreCase)
            select (importedRecipient.Organisation, serviceRecipient.Name, serviceRecipient.OrgId)).ToList();
    }

    private static string GetNameValidationBacklink(
        IEnumerable<ServiceRecipientImportModel> importedServiceRecipients,
        IEnumerable<ServiceRecipient> serviceRecipients) =>
        GetMismatchedOdsCodes(importedServiceRecipients, serviceRecipients).Any()
            ? nameof(ValidateOds)
            : nameof(Index);

    private static string[] GetValidOdsCodes(
        IEnumerable<ServiceRecipientImportModel> importedServiceRecipients,
        IEnumerable<ServiceRecipient> serviceRecipients)
    {
        var importedRecipients = importedServiceRecipients.ToList();
        var organisationRecipients = serviceRecipients.ToList();

        var validOdsCodes = organisationRecipients.Where(
                x => importedRecipients.Any(
                    r => string.Equals(x.OrgId, r.OdsCode, StringComparison.OrdinalIgnoreCase)))
            .Select(x => x.OrgId)
            .Distinct()
            .ToArray();

        return validOdsCodes;
    }

    private static (bool Validated, string Error) ValidateServiceRecipients(
        IList<ServiceRecipientImportModel> importedRecipients)
    {
        if (importedRecipients == null)
        {
            return (false, InvalidFormat);
        }

        if (!importedRecipients.Any())
        {
            return (false, EmptyFile);
        }

        foreach (var recipient in importedRecipients)
        {
            if (string.IsNullOrWhiteSpace(recipient.Organisation) || string.IsNullOrWhiteSpace(recipient.OdsCode))
            {
                return (false, InvalidFormat);
            }

            if (recipient.OdsCode.Length > OdsCodeLength)
            {
                return (false, OdsCodeExceedsLimit);
            }

            if (recipient.Organisation.Length > OrganisationNameLength)
            {
                return (false, OrganisationExceedsLimit);
            }
        }

        return (true, null);
    }
}
