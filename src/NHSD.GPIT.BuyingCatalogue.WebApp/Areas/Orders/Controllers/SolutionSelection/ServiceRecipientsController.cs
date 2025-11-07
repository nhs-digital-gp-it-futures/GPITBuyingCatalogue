using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/service-recipients")]
    [ServiceFilter(typeof(OrderIsEditableActionFilterAttribute))]
    public class ServiceRecipientsController(
        IOdsService odsService,
        IOrderService orderService,
        IOrderSublocationService orderSublocationService,
        IOrganisationsService organisationsService)
        : Controller
    {
        private readonly IOdsService odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));

        private readonly IOrderService orderService =
            orderService ?? throw new ArgumentNullException(nameof(orderService));

        private readonly IOrderSublocationService orderSublocationService =
            orderSublocationService ?? throw new ArgumentNullException(nameof(orderSublocationService));

        private readonly IOrganisationsService organisationsService =
            organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));

        [HttpGet("upload-or-select-service-recipients")]
        public IActionResult UploadOrSelectServiceRecipients(
            string internalOrgId,
            CallOffId callOffId)
        {
            var model = new UploadOrSelectServiceRecipientModel()
            {
                Caption = $"Order {callOffId}",
                BackLink =
                        Url.Action(
                            nameof(OrderController.Order),
                            typeof(OrderController).ControllerName(),
                            new { internalOrgId, callOffId }),
            };
            return View("ServiceRecipients/UploadOrSelectServiceRecipient", model);
        }

        [HttpPost("upload-or-select-service-recipients")]
        public async Task<IActionResult> UploadOrSelectServiceRecipients(
            UploadOrSelectServiceRecipientModel model,
            string internalOrgId,
            CallOffId callOffId)
        {
            if (!ModelState.IsValid)
                return View("ServiceRecipients/UploadOrSelectServiceRecipient", model);

            if (model.ShouldUploadRecipients.GetValueOrDefault())
            {
                return RedirectToAction(
                    nameof(Index),
                    typeof(ImportServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var orderHasSublocations = await orderService.GetOrderHasAnySublocations(callOffId, internalOrgId);

            return RedirectToAction(
                orderHasSublocations ? nameof(ConfirmSublocations) : nameof(SelectSublocations),
                typeof(ServiceRecipientsController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("select-sublocations")]
        public async Task<IActionResult> SelectSublocations(
            string internalOrgId,
            CallOffId callOffId)
        {
            OrderWrapper wrapper =
                await orderService.GetOrderWithSublocations(callOffId, internalOrgId);

            if (wrapper is null)
            {
                return NotFound();
            }

            IEnumerable<OdsOrganisation> possibleSublocations =
                await odsService.GetSublocationsByParentOdsCode(wrapper.Order.OrderingParty.ExternalIdentifier);

            var backLink = Url.Action(
                nameof(UploadOrSelectServiceRecipients),
                typeof(ServiceRecipientsController).ControllerName(),
                new { internalOrgId, callOffId });

            var model = new SelectSublocationsModel(
                wrapper,
                possibleSublocations,
                backLink);

            return View("ServiceRecipients/SelectSublocations", model);
        }

        [HttpPost("select-sublocations")]
        public async Task<IActionResult> SelectSublocations(
            SelectSublocationsModel selectSublocations,
            string internalOrgId,
            CallOffId callOffId)
        {
            if (!ModelState.IsValid)
            {
                return View("ServiceRecipients/SelectSublocations", selectSublocations);
            }

            HashSet<string> sublocationOdsCodes =
                selectSublocations.RenderedSublocations.Where(x => x.Selected).Select(y => y.Value).ToHashSet();

            OrderWrapper wrapper =
                await orderService.GetOrderWithSublocations(callOffId, internalOrgId);

            HashSet<string> orderSublocations =
                wrapper.Order.OrderSublocations.Select(x => x.SublocationOdsCode).ToHashSet();

            HashSet<string> removes = orderSublocations.Except(sublocationOdsCodes).ToHashSet();

            var stringOfRemoves = JoinEnumerableStringsToCommaSeparatedString(removes);

            if (removes.Count > 0)
            {
                var stringOfSublocations = JoinEnumerableStringsToCommaSeparatedString(sublocationOdsCodes);

                return RedirectToAction(
                    nameof(RemoveSublocations),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new
                    {
                        internalOrgId, callOffId, sublocations = stringOfSublocations, removes = stringOfRemoves,
                    });
            }

            await orderService.SetSublocations(callOffId, internalOrgId, sublocationOdsCodes);

            return RedirectToAction(
                nameof(ConfirmSublocations),
                typeof(ServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });
        }

        [HttpGet("remove-sublocations")]
        public async Task<IActionResult> RemoveSublocations(
            string internalOrgId,
            CallOffId callOffId,
            string sublocations,
            string removes)
        {
            var parsedSublocations = SplitCommaSeparatedString(sublocations);

            var parsedRemoves = SplitCommaSeparatedString(removes);
            OrderWrapper wrapper =
                await orderService.GetOrderThin(callOffId, internalOrgId);

            if (wrapper is null)
            {
                return NotFound();
            }

            var backLink = Url.Action(
                nameof(ConfirmSublocations),
                typeof(ServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });

            var model = new RemoveSublocationsModel(
                wrapper.Order,
                parsedSublocations,
                parsedRemoves,
                backLink);

            return View("ServiceRecipients/RemoveSublocations", model);
        }

        [HttpPost("remove-sublocations")]
        public async Task<IActionResult> RemoveSublocations(
            RemoveSublocationsModel removeSublocationsModel,
            string internalOrgId,
            CallOffId callOffId)
        {
            if (!ModelState.IsValid)
            {
                return View("ServiceRecipients/RemoveSublocations", removeSublocationsModel);
            }

            if (removeSublocationsModel.SublocationOdsCodes is not
                { Count: > 0 })
            {
                return BadRequest();
            }

            if (removeSublocationsModel.ConfirmRemove is false)
            {
                return RedirectToAction(
                    nameof(ConfirmSublocations),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { callOffId, internalOrgId });
            }

            HashSet<string> sublocations = removeSublocationsModel.SublocationOdsCodes.ToHashSet();

            await orderService.SetSublocations(
                callOffId,
                internalOrgId,
                sublocations);

            return RedirectToAction(
                nameof(ConfirmSublocations),
                typeof(ServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });
        }

        [HttpGet("{sublocationOdsCode}")]
        public async Task<IActionResult> SelectSublocationRecipients(
            string internalOrgId,
            CallOffId callOffId,
            string sublocationOdsCode,
            SelectionMode? selectionMode = null)
        {
            var externalOrganisationId =
                await organisationsService.GetOrganisationExternalIdentifierByInternalIdentifier(internalOrgId);

            if (externalOrganisationId is null)
            {
                return NotFound();
            }

            var orderId = await orderService.GetOrderId(callOffId);

            OrderSublocation orderSublocation =
                await orderSublocationService.GetOrderSublocationWithRecipients(
                    externalOrganisationId,
                    orderId,
                    sublocationOdsCode);

            if (orderSublocation is null)
            {
                return NotFound();
            }

            var sublocationAsSublocationModel = new SublocationModel(orderSublocation, true);

            List<ServiceRecipientModel> possibleRecipients =
                await GetServiceRecipientModelsBySublocation(sublocationOdsCode);

            var backLink = Url.Action(
                    nameof(ConfirmSublocations),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId });

            if (callOffId.IsAmendment)
            {
                OrderWrapper orderHistory =
                    await orderService.GetOrderWithSublocationsAndSublocationRecipients(callOffId, internalOrgId);

                IReadOnlyList<ServiceRecipientModel> previousRecipients =
                    orderHistory.Previous?.OrderSublocations
                        .FirstOrDefault(x => x.SublocationOdsCode == sublocationOdsCode)
                        ?
                        .SublocationRecipients.Select(y => new ServiceRecipientModel(y, true))
                        .ToList() ?? [];

                var allPossibleRecipientsAlreadySelectedInPrevious = possibleRecipients.All(x =>
                    previousRecipients?.Any(y => x.OdsCode == y.OdsCode) ?? false);

                if (allPossibleRecipientsAlreadySelectedInPrevious)
                {
                    var backAndContinueLink = Url.Action(
                        nameof(ConfirmSublocations),
                        typeof(ServiceRecipientsController).ControllerName(),
                        new { callOffId, internalOrgId });

                    var noNewModel = new NoNewRecipientsForSublocationAmendmentModel(
                        orderHistory.Order,
                        previousRecipients.Select(x => $"{x.Name} ({x.OdsCode})").ToList(),
                        backAndContinueLink);

                    return View("ServiceRecipients/NoNewRecipientsForSublocationAmendment", noNewModel);
                }

                var amendmentModel = new SelectSublocationRecipientsModel(
                    orderHistory.Order,
                    previousRecipients,
                    sublocationAsSublocationModel,
                    possibleRecipients,
                    backLink,
                    selectionMode);

                return View("ServiceRecipients/SelectSublocationRecipients", amendmentModel);
            }

            var model = new SelectSublocationRecipientsModel(
                orderSublocation.Order,
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
            CallOffId callOffId,
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

            var orderId = await orderService.GetOrderId(callOffId);

            OrderSublocation sublocation =
                await orderSublocationService.GetOrderSublocationWithRecipients(
                    externalOrganisationId,
                    orderId,
                    sublocationOdsCode);

            if (sublocation is null)
            {
                return BadRequest();
            }

            HashSet<string> pageSelections = selectSublocationRecipientsModel.RenderedServiceRecipients
                .Where(x => x.Selected)
                .Select(y => y.Value)
                .ToHashSet();

            await orderSublocationService.SetSublocationRecipients(
                externalOrganisationId,
                orderId,
                sublocationOdsCode,
                pageSelections);

            return RedirectToAction(
                nameof(ConfirmSublocations),
                typeof(ServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });
        }

        [HttpGet("confirm-sublocations")]
        public async Task<IActionResult> ConfirmSublocations(string internalOrgId, CallOffId callOffId)
        {
            var backLink = Url.Action(
                    nameof(UploadOrSelectServiceRecipients),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId });

            return await SelectSublocationsOverview(callOffId, internalOrgId, true, backLink);
        }

        [HttpPost("confirm-sublocations")]
        public async Task<IActionResult> ConfirmSublocations(
            SelectSublocationsOverviewModel model,
            string internalOrgId,
            CallOffId callOffId)
        {
            return await SelectSublocationsOverviewDynamicRedirect(model, internalOrgId, callOffId);
        }

        [HttpGet("confirm-sublocation-recipients")]
        public async Task<IActionResult> ConfirmSublocationRecipients(
            string internalOrgId,
            CallOffId callOffId)
        {
            OrderWrapper wrapper =
                await orderService.GetOrderWithSublocationsAndSublocationRecipients(
                    callOffId,
                    internalOrgId);

            if (wrapper is null)
            {
                return NotFound();
            }

            var backLinkUrl = Url.Action(
                nameof(ConfirmSublocations),
                typeof(ServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });

            var continueLinkUrl = Url.Action(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { callOffId, internalOrgId });

            if (wrapper.IsAmendment && !wrapper.HasNewOrderRecipients)
            {
                var amendModel = new NoNewRecipientsForAmendmentModel(
                    wrapper.Order,
                    backLinkUrl,
                    continueLinkUrl);

                return View("ServiceRecipients/NoNewRecipientsForAmendment", amendModel);
            }

            if (wrapper.IsAmendment)
            {
                var amendWithNewRecipientsModel = new ConfirmSublocationRecipientsModel(
                    wrapper,
                    backLinkUrl,
                    continueLinkUrl);

                return View("ServiceRecipients/ConfirmSublocationRecipients", amendWithNewRecipientsModel);
            }

            var model = new ConfirmSublocationRecipientsModel(
                wrapper.Order,
                backLinkUrl,
                continueLinkUrl);

            return View("ServiceRecipients/ConfirmSublocationRecipients", model);
        }

        [HttpGet("select-recipient-for-practice-reorganisation")]
        public async Task<IActionResult> SelectRecipientForPracticeReorganisation(
            string internalOrgId, CallOffId callOffId, string recipientIds, string selectedRecipientId)
        {
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            var orderType = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order.OrderType;

            var selectedOdsCodes = UrlStringToValues(recipientIds);
            var serviceRecipientsList = MapToModel(
                await odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(internalOrgId, selectedOdsCodes), false);

            var backLink = Url.Action(nameof(ConfirmSublocations), new { internalOrgId, callOffId });

            var model = new RecipientForPracticeReorganisationModel(
                organisation,
                callOffId,
                orderType,
                serviceRecipientsList,
                selectedRecipientId,
                backLink);

            return View("MergerOrSplit/SelectRecipientForPracticeReorganisation", model);
        }

        [HttpPost("select-recipient-for-practice-reorganisation")]
        public IActionResult SelectRecipientForPracticeReorganisation(
            string internalOrgId, CallOffId callOffId, string recipientIds, RecipientForPracticeReorganisationModel model)
        {
            if (!ModelState.IsValid) return View("MergerOrSplit/SelectRecipientForPracticeReorganisation", model);

            var selectedRecipientId = model.SelectedRecipientId;

            return RedirectToAction(
                nameof(ConfirmChanges),
                new { internalOrgId, callOffId, recipientIds, selectedRecipientId });
        }

        [HttpGet("confirm-recipients")]
        public async Task<IActionResult> ConfirmChanges(
            string internalOrgId, CallOffId callOffId, string recipientIds, string selectedRecipientId)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var orderType = wrapper.Order.OrderType;

            var selectedOdsCodes = UrlStringToValues(recipientIds);
            var serviceRecipientsList = MapToModel(
                await odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(internalOrgId, selectedOdsCodes), false);

            var retainedRecipient = serviceRecipientsList.FirstOrDefault(r => r.OdsCode == selectedRecipientId);
            if (retainedRecipient is null)
                return BadRequest($"The selected merger or split recipient {selectedRecipientId} isn't in the list of recipients");

            serviceRecipientsList.Remove(retainedRecipient);

            var backLink = Url.Action(
                    nameof(SelectRecipientForPracticeReorganisation),
                    new { internalOrgId, callOffId, recipientIds, selectedRecipientId });

            var addRemoveRecipientsLink = Url.Action(
                nameof(ConfirmSublocations),
                new { internalOrgId, callOffId });

            var model = new ConfirmChangesModel(
                callOffId,
                orderType,
                serviceRecipientsList,
                retainedRecipient,
                backLink,
                addRemoveRecipientsLink);

            return View("MergerOrSplit/ConfirmChanges", model);
        }

        [HttpPost("confirm-recipients")]
        public async Task<IActionResult> ConfirmChanges(
            string internalOrgId, CallOffId callOffId, ConfirmChangesModel model)
        {
            var wrapper = await orderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId);
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

            await orderService.SetOrderPracticeReorganisationRecipient(
                internalOrgId, callOffId, model.PracticeReorganisationRecipient.OdsCode);

            var selectedRecipients = await odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                internalOrgId, model.SelectedRecipients.Select(x => x.OdsCode));

            var recipientsAsSublocationModel = selectedRecipients
                .GroupBy(x => x.LocationOrgId)
                .Select(x => new SublocationModel
                {
                    OdsCode = x.Key,
                    ServiceRecipients = x.Select(y => new ServiceRecipientModel(y)).ToList(),
                })
                .ToList();

            var sublocationsAsEntityModel = recipientsAsSublocationModel.Select(sl => new OrderSublocation
            {
                OrderId = wrapper.Order.Id,
                OwnerOdsCode = organisation.ExternalIdentifier,
                SublocationOdsCode = sl.OdsCode,
                SublocationRecipients = sl.ServiceRecipients
                    .Select(sr => wrapper.CreateRecipientWithExistingOrderContext(sr.OdsCode, sl.OdsCode))
                    .ToList(),
            }).ToList();

            await orderService.SetSublocationsAndRecipients(callOffId, internalOrgId, sublocationsAsEntityModel);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        private static string JoinEnumerableStringsToCommaSeparatedString(IEnumerable<string> stringEnumerable)
        {
            return string.Join(",", stringEnumerable);
        }

        private static string[] SplitCommaSeparatedString(string sublocationsToRemove)
        {
            return sublocationsToRemove?.Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
        }

        private static List<ServiceRecipientModel> MapToModel(
            IEnumerable<ServiceRecipient> recipients,
            bool orderByName)
        {
            if (orderByName)
            {
                recipients = recipients.OrderBy(x => x.Name);
            }

            return recipients
                .Select(x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OrgId, Location = x.Location })
                .ToList();
        }

        private static string[] UrlStringToValues(string recipientIds)
        {
            return recipientIds?.Split(
                RecipientsConstants.Delimiter,
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        private async Task<IActionResult> SelectSublocationsOverview(
            CallOffId callOffId,
            string internalOrgId,
            bool isConfirm,
            string backLink)
        {
            OrderWrapper wrapper =
                await orderService.GetOrderWithSublocations(callOffId, internalOrgId);

            if (wrapper is null)
            {
                return NotFound();
            }

            var sublocations = new List<SublocationModel>();

            foreach (OrderSublocation s in wrapper.Order.OrderSublocations)
            {
                await MapSublocationToSublocationModel(s);
            }

            var addOrChangeSublocationsLink = Url.Action(
                nameof(SelectSublocations),
                typeof(ServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });

            var model = new SelectSublocationsOverviewModel(
                isConfirm,
                wrapper.Order,
                sublocations,
                addOrChangeSublocationsLink,
                backLink);

            return View("ServiceRecipients/SelectSublocationsOverview", model);

            async Task MapSublocationToSublocationModel(OrderSublocation orderSublocation)
            {
                var recipientLink = Url.Action(
                    nameof(SelectSublocationRecipients),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { callOffId, internalOrgId, sublocationOdsCode = orderSublocation.SublocationOdsCode });

                var serviceRecipientCount =
                    await orderSublocationService.GetCountForOrderSublocationRecipients(
                        wrapper.Order.OrderingParty.ExternalIdentifier,
                        wrapper.Order.Id,
                        orderSublocation.SublocationOdsCode);

                var previousRecipientCount = 0;

                if (wrapper.IsAmendment)
                {
                    var previousRevisionOrderId = wrapper.PreviousOrders[^1].Id;

                    previousRecipientCount = await orderSublocationService.GetCountForOrderSublocationRecipients(
                        wrapper.Order.OrderingParty.ExternalIdentifier,
                        previousRevisionOrderId,
                        orderSublocation.SublocationOdsCode);
                }

                TaskProgress taskProgress = serviceRecipientCount switch
                {
                    0 => TaskProgress.NotStarted,
                    > 0 when !wrapper.IsAmendment => TaskProgress.Completed,
                    > 0 when wrapper.IsAmendment && serviceRecipientCount == previousRecipientCount =>
                        TaskProgress.Completed,
                    > 0 when wrapper.IsAmendment && serviceRecipientCount > previousRecipientCount =>
                        TaskProgress.Amended,
                    _ => throw new InvalidOperationException("No valid case for service recipient count"),
                };

                var sublocationModel = new SublocationModel(
                    orderSublocation,
                    recipientLink,
                    serviceRecipientCount,
                    taskProgress);

                sublocations.Add(sublocationModel);
            }
        }

        private async Task<IActionResult> SelectSublocationsOverviewDynamicRedirect(
            SelectSublocationsOverviewModel model,
            string internalOrgId,
            CallOffId callOffId)
        {
            if (model.Sublocations.Any(x => x.ServiceRecipientCount == 0))
            {
                return RedirectToAction(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { callOffId, internalOrgId });
            }

            var wrapper = await orderService.GetOrderWithSublocationsAndSublocationRecipients(callOffId, internalOrgId);
            var isMerger = wrapper?.Order?.OrderType.MergerOrSplit == true;

            if (isMerger)
            {
                var allRecipientOds = wrapper.Order.OrderSublocations
                    .SelectMany(s => s.SublocationRecipients)
                    .Select(r => r.RecipientOdsCode)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToList();

                var recipientIds = allRecipientOds.ToRecipientsString();
                var selectedRecipientId = wrapper.Order.AssociatedServicesOnlyDetails?.PracticeReorganisationOdsCode;

                return RedirectToAction(
                    nameof(SelectRecipientForPracticeReorganisation),
                    new { internalOrgId, callOffId, recipientIds, selectedRecipientId });
            }

            return RedirectToAction(
                nameof(ConfirmSublocationRecipients),
                typeof(ServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });
        }

        private async Task<List<ServiceRecipientModel>> GetServiceRecipientModelsBySublocation(
            string sublocationOdsCode)
        {
            IEnumerable<ServiceRecipient> recipients =
                await odsService.GetServiceRecipientsBySublocation(sublocationOdsCode);

            return recipients
                .Select(x => new ServiceRecipientModel(x))
                .ToList();
        }
    }
}
