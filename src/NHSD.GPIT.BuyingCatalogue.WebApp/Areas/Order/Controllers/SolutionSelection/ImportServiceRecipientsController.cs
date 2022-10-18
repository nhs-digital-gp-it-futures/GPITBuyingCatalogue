﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CatalogueItems;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.ImportServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.ServiceRecipients;
using ServiceRecipient = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceRecipient;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;

[Authorize("Buyer")]
[Area("Order")]
[Route("order/organisation/{internalOrgId}/order/{callOffId}/item/{catalogueItemId}/import-service-recipients")]
public class ImportServiceRecipientsController : Controller
{
    internal const int OdsCodeLength = 8;
    internal const int OrganisationNameLength = 256;
    internal const string InvalidFormat = "The selected file does not meet the required format";
    internal const string EmptyFile = "The selected file is empty";

    internal static readonly string OdsCodeExceedsLimit =
        $"At least one of your ODS codes is more than {OdsCodeLength} characters";

    internal static readonly string OrganisationExceedsLimit =
        $"At least one of your Service Recipient names is more than {OrganisationNameLength} characters";

    private readonly IServiceRecipientImportService importService;
    private readonly ICatalogueItemService catalogueItemService;
    private readonly IOdsService odsService;

    public ImportServiceRecipientsController(
        IServiceRecipientImportService importService,
        ICatalogueItemService catalogueItemService,
        IOdsService odsService)
    {
        this.importService = importService ?? throw new ArgumentNullException(nameof(importService));
        this.catalogueItemService =
            catalogueItemService ?? throw new ArgumentNullException(nameof(catalogueItemService));
        this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        ServiceRecipientImportMode? importMode = ServiceRecipientImportMode.Edit)
    {
        _ = importMode;
        importService.Clear(new(User.UserId(), internalOrgId, callOffId, catalogueItemId));

        var catalogueItemName = await catalogueItemService.GetCatalogueItemName(catalogueItemId);
        var model = new ImportServiceRecipientModel(internalOrgId, callOffId, catalogueItemId, catalogueItemName)
        {
            BackLink = Url.Action(
                GetServiceRecipientRedirectAction(importMode!.Value),
                typeof(ServiceRecipientsController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId }),
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        ImportServiceRecipientModel model,
        ServiceRecipientImportMode? importMode = ServiceRecipientImportMode.Edit)
    {
        if (!ModelState.IsValid)
            return View(model);

        IList<ServiceRecipientImportModel> importedServiceRecipients =
            await importService.ReadFromStream(model.File.OpenReadStream());
        var (validatedSuccessfully, error) = ValidateServiceRecipients(importedServiceRecipients);

        if (!validatedSuccessfully)
        {
            ModelState.AddModelError(nameof(model.File), error);
            return View(model);
        }

        importService.Store(
            new(
                User.UserId(),
                internalOrgId,
                callOffId,
                catalogueItemId),
            importedServiceRecipients);

        return RedirectToAction(
            nameof(ValidateOds),
            new { internalOrgId, callOffId, catalogueItemId, importMode });
    }

    [HttpGet("validate-ods")]
    public async Task<IActionResult> ValidateOds(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        ServiceRecipientImportMode? importMode = ServiceRecipientImportMode.Edit)
    {
        var cacheKey = new ServiceRecipientCacheKey(User.UserId(), internalOrgId, callOffId, catalogueItemId);
        var cachedRecipients = importService.GetCached(cacheKey);
        if (cachedRecipients is null)
            return RedirectToAction(nameof(Index), new { internalOrgId, callOffId, catalogueItemId, importMode });

        var organisationServiceRecipients =
            await odsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId);

        var mismatchedOdsCodes = GetMismatchedOdsCodes(cachedRecipients, organisationServiceRecipients).ToList();
        if (mismatchedOdsCodes.Any())
        {
            var catalogueItemName = await catalogueItemService.GetCatalogueItemName(catalogueItemId);
            var model = new ValidateOdsModel(
                internalOrgId,
                callOffId,
                catalogueItemId,
                catalogueItemName,
                importMode!.Value,
                mismatchedOdsCodes)
            {
                BackLink = Url.Action(nameof(Index), new { internalOrgId, callOffId, catalogueItemId, importMode }),
            };

            return View(model);
        }

        return RedirectToAction(
            nameof(ValidateNames),
            new
            {
                internalOrgId, callOffId, catalogueItemId, importMode,
            });
    }

    [HttpGet("validate-names")]
    public async Task<IActionResult> ValidateNames(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        ServiceRecipientImportMode? importMode = ServiceRecipientImportMode.Edit)
    {
        var cacheKey = new ServiceRecipientCacheKey(User.UserId(), internalOrgId, callOffId, catalogueItemId);
        var cachedRecipients = importService.GetCached(cacheKey);
        if (cachedRecipients is null)
            return RedirectToAction(nameof(Index), new { internalOrgId, callOffId, catalogueItemId, importMode });

        var organisationServiceRecipients =
            (await odsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId)).ToList();

        var mismatchedRecipients = GetMismatchedNames(cachedRecipients.ToList(), organisationServiceRecipients);
        if (mismatchedRecipients.Any())
        {
            var catalogueItemName = await catalogueItemService.GetCatalogueItemName(catalogueItemId);
            var model = new ValidateNamesModel(
                internalOrgId,
                callOffId,
                catalogueItemId,
                catalogueItemName,
                importMode!.Value,
                mismatchedRecipients)
            {
                BackLink = Url.Action(
                    GetNameValidationBacklink(cachedRecipients, organisationServiceRecipients),
                    new { internalOrgId, callOffId, catalogueItemId, importMode }),
            };
            return View(model);
        }

        var validOdsCodes = GetValidOdsCodes(cachedRecipients, organisationServiceRecipients);
        importService.Clear(cacheKey);
        return RedirectToAction(
            GetServiceRecipientRedirectAction(importMode!.Value),
            typeof(ServiceRecipientsController).ControllerName(),
            new
            {
                internalOrgId,
                callOffId,
                catalogueItemId,
                importedRecipients = string.Join(',', validOdsCodes),
            });
    }

    [HttpPost("validate-names")]
    public async Task<IActionResult> ValidateNames(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        ValidateNamesModel model,
        ServiceRecipientImportMode? importMode = ServiceRecipientImportMode.Edit)
    {
        var cacheKey = new ServiceRecipientCacheKey(User.UserId(), internalOrgId, callOffId, catalogueItemId);
        var cachedRecipients = importService.GetCached(cacheKey);
        var organisationServiceRecipients =
            await odsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId);

        var validOdsCodes = GetValidOdsCodes(cachedRecipients, organisationServiceRecipients);

        importService.Clear(cacheKey);

        return RedirectToAction(
            GetServiceRecipientRedirectAction(importMode!.Value),
            typeof(ServiceRecipientsController).ControllerName(),
            new
            {
                internalOrgId, callOffId, catalogueItemId, importedRecipients = string.Join(',', validOdsCodes),
            });
    }

    [HttpGet("download-template")]
    public async Task<IActionResult> DownloadTemplate(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        ServiceRecipientImportMode? importMode = ServiceRecipientImportMode.Edit)
    {
        _ = internalOrgId;
        _ = callOffId;
        _ = catalogueItemId;
        _ = importMode;

        using var stream = new MemoryStream();
        await importService.CreateServiceRecipientTemplate(stream);
        stream.Position = 0;

        return File(stream.ToArray(), "application/octet-stream", "service_recipient_template.csv");
    }

    [HttpGet("cancel-import")]
    public IActionResult CancelImport(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId,
        ServiceRecipientImportMode? importMode = ServiceRecipientImportMode.Edit)
    {
        importService.Clear(new(User.UserId(), internalOrgId, callOffId, catalogueItemId));

        return RedirectToAction(
            GetServiceRecipientRedirectAction(importMode!.Value),
            typeof(ServiceRecipientsController).ControllerName(),
            new { internalOrgId, callOffId, catalogueItemId });
    }

    private static string GetServiceRecipientRedirectAction(ServiceRecipientImportMode importMode)
        => importMode == ServiceRecipientImportMode.Add
            ? nameof(ServiceRecipientsController.AddServiceRecipients)
            : nameof(ServiceRecipientsController.EditServiceRecipients);

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
                    r => string.Equals(x.OrgId, r.OdsCode, StringComparison.OrdinalIgnoreCase) && string.Equals(
                        x.Name,
                        r.Organisation,
                        StringComparison.OrdinalIgnoreCase)))
            .Select(x => x.OrgId)
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
