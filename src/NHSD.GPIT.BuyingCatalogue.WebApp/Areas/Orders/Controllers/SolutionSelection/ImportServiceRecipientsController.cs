using System;
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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels.ImportServiceRecipients;
using ServiceRecipient = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.ServiceRecipient;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;

[Authorize("Buyer")]
[Area("Orders")]
[Route("order/organisation/{internalOrgId}/order/{callOffId}/import-service-recipients")]
public class ImportServiceRecipientsController(
    IServiceRecipientImportService importService,
    ICatalogueItemService catalogueItemService,
    IOrderService orderService,
    IOdsService odsService) : Controller
{
    internal const int OdsCodeLength = 8;
    internal const int OrganisationNameLength = 256;
    internal const string InvalidFormat = "The selected file does not meet the required format";
    internal const string EmptyFile = "The selected file is empty";

    internal static readonly string OdsCodeExceedsLimit =
        $"At least one of your ODS codes is more than {OdsCodeLength} characters";

    internal static readonly string OrganisationExceedsLimit =
        $"At least one of your Service Recipient names is more than {OrganisationNameLength} characters";

    private readonly IServiceRecipientImportService importService =
        importService ?? throw new ArgumentNullException(nameof(importService));

    private readonly ICatalogueItemService catalogueItemService =
        catalogueItemService ?? throw new ArgumentNullException(nameof(catalogueItemService));

    private readonly IOrderService orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
    private readonly IOdsService odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));

    [HttpGet]
    public async Task<IActionResult> Index(
        string internalOrgId,
        CallOffId callOffId)
    {
        await importService.Clear(new(User.UserId(), internalOrgId, callOffId));

        var model = new ImportServiceRecipientModel
        {
            BackLink = Url.Action(
                nameof(ServiceRecipientsController.UploadOrSelectServiceRecipients),
                typeof(ServiceRecipientsController).ControllerName(),
                new { internalOrgId, callOffId }),
            Caption = callOffId.ToString(),
            DownloadTemplateLink = Url.Action(nameof(DownloadTemplate), new { internalOrgId, callOffId }),
        };

        return View("ServiceRecipients/ImportServiceRecipients/Index", model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(
        string internalOrgId,
        CallOffId callOffId,
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
                User.UserId(),
                internalOrgId,
                callOffId),
            importedServiceRecipients);

        return RedirectToAction(
            nameof(Validate),
            new { internalOrgId, callOffId });
    }

    [HttpGet("validate")]
    public async Task<IActionResult> Validate(
        string internalOrgId,
        CallOffId callOffId,
        bool acceptLossOfOdsIfMismatch)
    {
        var cacheKey = new DistributedCacheKey(User.UserId(), internalOrgId, callOffId);
        var cachedRecipients = await importService.GetCached(cacheKey);

        if (cachedRecipients is null)
            return RedirectToAction(nameof(Index), new { internalOrgId, callOffId });

        var backAndCancelLink = Url.Action(nameof(CancelImport), new { internalOrgId, callOffId });

        ValidationStatus validationStatus = acceptLossOfOdsIfMismatch
            ? ValidationStatus.PartialSuccess
            : ValidationStatus.Success;

        HashSet<string> requestedRecipientOdsCodes = cachedRecipients.Select(x => x.OdsCode).ToHashSet();

        IReadOnlyList<ServiceRecipient> organisationServiceRecipients =
            await odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                internalOrgId,
                requestedRecipientOdsCodes);

        HashSet<string> actualServiceRecipientsAsHashSet =
            organisationServiceRecipients.Select(x => x.OrgId).ToHashSet();

        if (actualServiceRecipientsAsHashSet.Count == 0)
        {
            validationStatus = ValidationStatus.Failure;
            return RedirectToAction(
                nameof(ValidationComplete),
                new { internalOrgId, callOffId, validationStatus });
        }

        var mismatchedOdsCodes =
            new HashSet<string>(requestedRecipientOdsCodes);
        mismatchedOdsCodes.ExceptWith(actualServiceRecipientsAsHashSet);

        var shouldShowValidateOdsScreen = mismatchedOdsCodes.Count > 0 && !acceptLossOfOdsIfMismatch;

        if (shouldShowValidateOdsScreen)
        {
            OrderWrapper wrapper = await orderService.GetOrderThin(callOffId, internalOrgId);

            var orderDescription = wrapper.Order.Description;

            var model = new ValidateOdsModel(
                cachedRecipients.Where(x => mismatchedOdsCodes.Contains(x.OdsCode)))
            {
                BackLink = backAndCancelLink,
                Caption = orderDescription,
                CancelLink = backAndCancelLink,
                ContinueLink = Url.Action(
                    nameof(Validate),
                    new { internalOrgId, callOffId, acceptLossOfOdsIfMismatch = true }),
            };

            return View("ServiceRecipients/ImportServiceRecipients/ValidateOds", model);
        }

        List<(string Expected, string Actual, string OdsCode)> mismatchedNames =
            GetMismatchedNames(cachedRecipients.ToList(), organisationServiceRecipients);

        var shouldShowValidateNamesScreen = mismatchedNames.Count > 0;

        if (shouldShowValidateNamesScreen)
        {
            OrderWrapper wrapper = await orderService.GetOrderThin(callOffId, internalOrgId);

            var orderDescription = wrapper.Order.Description;

            var continueLink = Url.Action(
                nameof(ValidationComplete),
                typeof(CompetitionImportServiceRecipientsController).ControllerName(),
                new { internalOrgId, callOffId, validationStatus = ValidationStatus.PartialSuccess });

            var model = new ValidateNamesModel(mismatchedNames)
            {
                BackLink = backAndCancelLink,
                CancelLink = backAndCancelLink,
                Caption = orderDescription,
                ContinueLink = continueLink,
            };
            return View("ServiceRecipients/ImportServiceRecipients/ValidateNames", model);
        }

        if (callOffId.IsAmendment)
        {
            return RedirectToAction(nameof(ValidateAmendment), new { internalOrgId, callOffId });
        }

        return RedirectToAction(
            nameof(ValidationComplete),
            new { internalOrgId, callOffId, validationStatus });
    }

    [HttpGet("validation-complete")]
    public async Task<IActionResult> ValidationComplete(
        string internalOrgId,
        CallOffId callOffId,
        ValidationStatus validationStatus)
    {
        OrderWrapper wrapper = await orderService.GetOrderThin(callOffId, internalOrgId);

        var orderDescription = wrapper.Order.Description;

        var cacheKey = new DistributedCacheKey(User.UserId(), internalOrgId, callOffId);

        IList<ServiceRecipientImportModel> cachedRecipients = await importService.GetCached(cacheKey);
        if (cachedRecipients is null)
            return RedirectToAction(nameof(Index), new { internalOrgId, callOffId });

        HashSet<string> requestedRecipientOdsCodes = cachedRecipients.Select(x => x.OdsCode).ToHashSet();

        IReadOnlyList<ServiceRecipient> organisationServiceRecipients =
            await odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                internalOrgId,
                requestedRecipientOdsCodes);

        List<SublocationModel> recipientsAsSublocations = organisationServiceRecipients.GroupBy(x => x.LocationOrgId)
            .Select(x => new SublocationModel
            {
                OdsCode = x.Key,
                ServiceRecipients = x.Select(y => new ServiceRecipientModel(y))
                    .ToList(),
            })
            .ToList();

        var model = new ValidationCompleteModel(orderDescription, validationStatus, recipientsAsSublocations);

        return View("ServiceRecipients/ImportServiceRecipients/ValidationComplete", model);
    }

    [HttpPost("validation-complete")]
    public async Task<IActionResult> ValidationComplete(
        string internalOrgId,
        CallOffId callOffId,
        ValidationCompleteModel model)
    {
        if (model.ValidationStatus is not (ValidationStatus.Success or ValidationStatus.PartialSuccess))
        {
            return RedirectToAction(
                nameof(CancelImport),
                new { internalOrgId, callOffId });
        }

        OrderWrapper wrapper = await orderService.GetOrderThin(callOffId, internalOrgId);

        List<OrderSublocation> sublocationModelAsEntityModel = model.Sublocations.Select(x => new OrderSublocation
            {
                OrderId = wrapper.Order.Id,
                SublocationOdsCode = x.OdsCode,
                OwnerOdsCode = wrapper.Order.OrderingParty.ExternalIdentifier,
                SublocationRecipients = x.ServiceRecipients.Select(y => new OrderSublocationRecipient
                    {
                        OrderId = wrapper.Order.Id,
                        RecipientOdsCode = y.OdsCode,
                        ParentSublocationOdsCode = x.OdsCode,
                    })
                    .ToList(),
            })
            .ToList();

        await orderService.SetSublocationsAndRecipients(
            callOffId,
            internalOrgId,
            sublocationModelAsEntityModel);

        await importService.Clear(
            new DistributedCacheKey(User.UserId(), internalOrgId, callOffId));

        return RedirectToAction(
            nameof(ServiceRecipientsController.ConfirmSublocations),
            typeof(ServiceRecipientsController).ControllerName(),
            new { internalOrgId, callOffId });
    }

    [HttpGet("validate-amendment")]
    public async Task<IActionResult> ValidateAmendment(
        string internalOrgId,
        CallOffId callOffId,
        bool acceptNoRemovalForAmendment)
    {
        var cacheKey = new DistributedCacheKey(User.UserId(), internalOrgId, callOffId);
        IList<ServiceRecipientImportModel> cachedRecipients = await importService.GetCached(cacheKey);

        if (cachedRecipients is null)
            return RedirectToAction(nameof(Index), new { internalOrgId, callOffId });

        var backAndCancelLink = Url.Action(nameof(CancelImport), new { internalOrgId, callOffId });

        ValidationStatus validationStatus = acceptNoRemovalForAmendment
            ? ValidationStatus.PartialSuccess
            : ValidationStatus.Success;

        OrderWrapper wrapper =
            await orderService.GetOrderWithSublocationsAndSublocationRecipients(callOffId, internalOrgId);

        HashSet<string> previousRecipientsAsHashSet =
            wrapper.Previous.FlattenedRecipients.Select(x => x.RecipientOdsCode).ToHashSet();

        HashSet<string> requestedRecipientOdsCodesForMissing = RequestedRecipientOdsCodes();

        HashSet<string> requestedRecipientOdsCodesForNew = RequestedRecipientOdsCodes();

        // new recipient ods codes 
        requestedRecipientOdsCodesForNew.ExceptWith(previousRecipientsAsHashSet);

        if (requestedRecipientOdsCodesForNew.Count == 0)
        {
            var failedModel = new ValidateAmendmentRecipientsModel
            {
                ContinueLink = Url.Action(
                    nameof(ServiceRecipientsController.SelectSublocations),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId }),
                CancelLink = backAndCancelLink,
            };
            return View("ServiceRecipients/ImportServiceRecipients/ValidateAmendmentRecipientsFailed", failedModel);
        }

        // missing recipient ods codes
        previousRecipientsAsHashSet.ExceptWith(requestedRecipientOdsCodesForMissing);

        List<ServiceRecipientModel> newRecipients =
            wrapper.Order.FlattenedRecipients.Where(x => requestedRecipientOdsCodesForNew.Contains(x.RecipientOdsCode))
                .Select(x => new ServiceRecipientModel(x, false))
                .OrderBy(x => x.LocationOrgId)
                .ToList();

        var hasMissing = previousRecipientsAsHashSet.Count > 0;

        var model = new ValidateAmendmentRecipientsModel
        {
            NewRecipients = newRecipients, HasMissing = hasMissing, CancelLink = backAndCancelLink,
        };

        return View("ServiceRecipients/ImportServiceRecipients/ValidateAmendmentRecipients", model);

        // Return new for each instance so hash set can be modified
        HashSet<string> RequestedRecipientOdsCodes()
        {
            return cachedRecipients.Select(x => x.OdsCode).ToHashSet();
        }
    }

    [HttpPost("validation-amendment")]
    public async Task<IActionResult> ValidateAmendment(
        string internalOrgId,
        CallOffId callOffId,
        ValidateAmendmentRecipientsModel model)
    {
        throw new NotImplementedException();
    }

    [HttpGet("download-template")]
    public async Task<IActionResult> DownloadTemplate(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId)
    {
        _ = internalOrgId;
        _ = callOffId;
        _ = catalogueItemId;

        using var stream = new MemoryStream();
        await importService.CreateServiceRecipientTemplate(stream);
        stream.Position = 0;

        return File(stream.ToArray(), "application/octet-stream", "service_recipient_template.csv");
    }

    [HttpGet("cancel-import")]
    public IActionResult CancelImport(
        string internalOrgId,
        CallOffId callOffId,
        CatalogueItemId catalogueItemId)
    {
        importService.Clear(new(User.UserId(), internalOrgId, callOffId, catalogueItemId));

        return RedirectToAction(
            nameof(ServiceRecipientsController.UploadOrSelectServiceRecipients),
            typeof(ServiceRecipientsController).ControllerName(),
            new { internalOrgId, callOffId, catalogueItemId });
    }

    private static List<(string Expected, string Actual, string OdsCode)> GetMismatchedNames(
        IReadOnlyList<ServiceRecipientImportModel> importedServiceRecipients,
        IReadOnlyList<ServiceRecipient> serviceRecipients)
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
