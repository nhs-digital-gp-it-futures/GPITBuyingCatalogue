using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
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
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/merger-or-split-service-recipients")]
    [ServiceFilter(typeof(OrderIsEditableActionFilterAttribute))]
    public class MergerOrSplitServiceRecipientsController(
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

        [HttpGet("select-sublocations")]
        public async Task<IActionResult> SelectSublocations(string internalOrgId, CallOffId callOffId)
        {
            var wrapper = await orderService.GetOrderWithSublocations(callOffId, internalOrgId);
            if (wrapper is null) return NotFound();

            var possibleSublocations =
                await odsService.GetSublocationsByParentOdsCode(wrapper.Order.OrderingParty.ExternalIdentifier);

            var backLink = Url.Action(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { callOffId, internalOrgId });

            var model = new SelectSublocationsModel(wrapper, possibleSublocations, backLink)
            {
                CustomTitle = "Select sublocations for this merger",
                CustomAdvice =
                $"Select all the {wrapper.Order.OrderingParty.Name} sublocations that will be involved in this merger. " +
                "They must all be using the same Catalogue Solution.",
                FormLabelText = string.Empty,
            };
            return View("ServiceRecipients/SelectSublocations", model);
        }

        [HttpPost("select-sublocations")]
        public async Task<IActionResult> SelectSublocations(
            SelectSublocationsModel selectSublocations,
            string internalOrgId,
            CallOffId callOffId)
        {
            if (!ModelState.IsValid)
                return View("ServiceRecipients/SelectSublocations", selectSublocations);

            var sublocationOdsCodes = selectSublocations.RenderedSublocations
                .Where(x => x.Selected)
                .Select(y => y.Value)
                .ToHashSet();

            var wrapper = await orderService.GetOrderWithSublocations(callOffId, internalOrgId);
            var orderSublocations = wrapper.Order.OrderSublocations.Select(x => x.SublocationOdsCode).ToHashSet();

            var removes = orderSublocations.Except(sublocationOdsCodes).ToHashSet();

            if (removes.Count > 0)
            {
                var stringOfSublocations = JoinEnumerableStringsToCommaSeparatedString(sublocationOdsCodes);
                var stringOfRemoves = JoinEnumerableStringsToCommaSeparatedString(removes);

                return RedirectToAction(
                    nameof(RemoveSublocations),
                    typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId, sublocations = stringOfSublocations, removes = stringOfRemoves });
            }

            await orderService.SetSublocations(callOffId, internalOrgId, sublocationOdsCodes);

            return RedirectToAction(
                nameof(AddSublocations),
                typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });
        }

        [HttpGet("add-sublocations")]
        public async Task<IActionResult> AddSublocations(string internalOrgId, CallOffId callOffId)
        {
            var backLink = Url.Action(
                nameof(SelectSublocations),
                typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });

            return await SelectSublocationsOverview(callOffId, internalOrgId, false, backLink);
        }

        [HttpPost("add-sublocations")]
        public async Task<IActionResult> AddSublocations(
            SelectSublocationsOverviewModel model,
            string internalOrgId,
            CallOffId callOffId)
        {
            return await SelectSublocationsOverviewDynamicRedirect(model, internalOrgId, callOffId);
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
                typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
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
                nameof(AddSublocations),
                typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
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
                typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });

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
                        typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
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
                    selectionMode)
                {
                    CustomTitle = "Add merging Service Recipients",
                    CustomAdvice = "Select all the practices that will be involved in the merger. They must all be using the same Catalogue Solution.",
                };

                return View("ServiceRecipients/SelectSublocationRecipients", amendmentModel);
            }

            var model = new SelectSublocationRecipientsModel(
                orderSublocation.Order,
                sublocationAsSublocationModel,
                possibleRecipients,
                backLink,
                selectionMode)
            {
                CustomTitle = "Add merging Service Recipients",
                CustomAdvice = "Select all the practices that will be involved in the merger. They must all be using the same Catalogue Solution.",
            };

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
                nameof(AddSublocations),
                typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });
        }

        [HttpGet("select-recipient-for-practice-reorganisation")]
        public async Task<IActionResult> SelectRecipientForPracticeReorganisation(
            string internalOrgId,
            CallOffId callOffId,
            string recipientIds,
            string selectedRecipientId)
        {
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            var orderType = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order.OrderType;

            var selectedRecipientOdsCodes = UrlStringToValues(recipientIds);

            var serviceRecipients = MapToModel(
                await odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                    internalOrgId,
                    selectedRecipientOdsCodes),
                orderByName: false);

            var model = new RecipientForPracticeReorganisationModel(
                organisation,
                callOffId,
                orderType,
                serviceRecipients)
            {
                SelectedOdsCode = selectedRecipientId,
                BackLink = Url.Action(
                    nameof(AddSublocations),
                    typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId }),
                ShowSublocationsHeading = false,
            };

            return View(model);
        }

        [HttpPost("select-recipient-for-practice-reorganisation")]
        public IActionResult SelectRecipientForPracticeReorganisation(
            string internalOrgId,
            CallOffId callOffId,
            string recipientIds,
            RecipientForPracticeReorganisationModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var selectedRecipientId = model.SelectedOdsCode;

            return RedirectToAction(
                nameof(ConfirmChanges),
                typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
                new { internalOrgId, callOffId, recipientIds, selectedRecipientId });
        }

        [HttpGet("select-recipients")]
        public async Task<IActionResult> SelectServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            SelectionMode? selectionMode = null)
        {
            Organisation organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            OrderWrapper wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            OrderType orderType = wrapper.Order.OrderType;

            if (!orderType.MergerOrSplit)
            {
                return BadRequest($"Expected {callOffId} to be a merger or a split");
            }

            List<ServiceRecipientModel> possibleServiceRecipients = MapToModel(
                await odsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId),
                true);

            List<string> preSelectedRecipients =
                wrapper.Order.FlattenedRecipients.Select(x => x.RecipientOdsCode).ToList();

            if (preSelectedRecipients.Count > 0
                && wrapper.Order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode is not null)
            {
                preSelectedRecipients.Add(wrapper.Order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode);
            }

            var model =
                new SelectMergerOrSplitRecipientsModel(
                    organisation,
                    callOffId,
                    wrapper.Order.OrderType,
                    possibleServiceRecipients,
                    preSelectedRecipients,
                    selectionMode,
                    wrapper.IsAmendment)
                {
                    BackLink = Url.Action(
                        nameof(OrderController.Order),
                        typeof(OrderController).ControllerName(),
                        new { internalOrgId, callOffId }),
                };

            return View(model);
        }

        [HttpPost("select-recipients")]
        public async Task<IActionResult> SelectServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            SelectMergerOrSplitRecipientsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var recipientIds = model.GetServiceRecipients()
                .Where(x => x.Selected)
                .Select(x => x.OdsCode)
                .ToRecipientsString();

            OrderWrapper wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            var selectedRecipientId = wrapper.Order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode;

            return RedirectToAction(
                nameof(SelectRecipientForPracticeReorganisation),
                typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
                new { internalOrgId, callOffId, recipientIds, selectedRecipientId });
        }

        [HttpGet("confirm-recipients")]
        public async Task<IActionResult> ConfirmChanges(
            string internalOrgId,
            CallOffId callOffId,
            string recipientIds,
            string selectedRecipientId)
        {
            OrderWrapper wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            OrderType orderType = wrapper.Order.OrderType;
            List<ServiceRecipientModel> selectedRecipients = MapToModel(
                await odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                    internalOrgId,
                    UrlStringToValues(recipientIds)),
                false);

            ServiceRecipientModel practiceReorganisation =
                selectedRecipients.FirstOrDefault(r => r.OdsCode == selectedRecipientId);

            if (practiceReorganisation is null)
            {
                return BadRequest(
                    $"The selected merger or split recipient {selectedRecipientId} isn't in the list of recipients");
            }

            selectedRecipients.Remove(practiceReorganisation);

            var model = new ConfirmChangesModel(
                callOffId,
                orderType,
                selectedRecipients,
                practiceReorganisation)
            {
                BackLink = Url.Action(
                    nameof(SelectRecipientForPracticeReorganisation),
                    new { internalOrgId, callOffId, recipientIds, selectedRecipientId }),
                AddRemoveRecipientsLink = string.Empty,
                Advice = "Review the organisations involved in the merger.",
            };

            return View(model);
        }

        [HttpPost("confirm-recipients")]
        public async Task<IActionResult> ConfirmChanges(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmChangesModel model)
        {
            OrderWrapper wrapper = await orderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId);

            Organisation organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

            await orderService.SetOrderPracticeReorganisationRecipient(
                internalOrgId,
                callOffId,
                model.PracticeReorganisationRecipient.OdsCode);

            IReadOnlyList<ServiceRecipient> selectedAsServiceRecipient =
                await odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                    internalOrgId,
                    model.Selected.Select(x => x.OdsCode));

            List<SublocationModel> recipientsAsSublocationModel = selectedAsServiceRecipient
                .GroupBy(x => x.LocationOrgId)
                .Select(x => new SublocationModel
                {
                    OdsCode = x.Key,
                    ServiceRecipients = x.Select(y => new ServiceRecipientModel(y))
                        .ToList(),
                })
                .ToList();

            List<OrderSublocation> sublocationsAsEntityModel =
                recipientsAsSublocationModel.Select(sl => new OrderSublocation
                {
                    OrderId = wrapper.Order.Id,
                    OwnerOdsCode = organisation.ExternalIdentifier,
                    SublocationOdsCode = sl.OdsCode,
                    SublocationRecipients = sl.ServiceRecipients
                            .Select(sr =>
                                wrapper.CreateRecipientWithExistingOrderContext(sr.OdsCode, sl.OdsCode))
                            .ToList(),
                })
                    .ToList();

            await orderService.SetSublocationsAndRecipients(callOffId, internalOrgId, sublocationsAsEntityModel);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("confirm-sublocations")]
        public async Task<IActionResult> ConfirmSublocations(string internalOrgId, CallOffId callOffId)
        {
            var backLink = Url.Action(
                nameof(SelectSublocations),
                typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });

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
                typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
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

        private static string[] UrlStringToValues(string recipientIds)
        {
            return recipientIds?.Split(
                RecipientsConstants.Delimiter,
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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
                typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });

            var model = new SelectSublocationsOverviewModel(
                isConfirm,
                wrapper.Order,
                sublocations,
                addOrChangeSublocationsLink,
                backLink)
            {
                CustomTitle = "Add organisations",
                CustomAdvice = "Select a sublocation to add organisations to this merger.",
            };

            return View("ServiceRecipients/SelectSublocationsOverview", model);

            async Task MapSublocationToSublocationModel(OrderSublocation orderSublocation)
            {
                var recipientLink = Url.Action(
                    nameof(SelectSublocationRecipients),
                    typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
                    new { callOffId, internalOrgId, sublocationOdsCode = orderSublocation.SublocationOdsCode });

                var serviceRecipientCount =
                    await orderSublocationService.GetCountForOrderSublocationRecipients(
                        wrapper.Order.OrderingParty.ExternalIdentifier,
                        wrapper.Order.Id,
                        orderSublocation.SublocationOdsCode);

                var previousRecipientCount = 0;

                if (wrapper.IsAmendment)
                {
                    // Get precise recipient counts for accurate status
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

        private async Task<IActionResult> SelectSublocationsOverviewDynamicRedirect(
            SelectSublocationsOverviewModel model,
            string internalOrgId,
            CallOffId callOffId)
        {
            var hasIncomplete = model.Sublocations.Any(x => x.ServiceRecipientCount == 0);
            if (hasIncomplete)
            {
                return RedirectToAction(
                    nameof(AddSublocations),
                    typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
                    new { callOffId, internalOrgId });
            }

            var wrapper = await orderService.GetOrderWithSublocationsAndSublocationRecipients(callOffId, internalOrgId);

            var allRecipientOds = wrapper.Order.OrderSublocations
                .SelectMany(s => s.SublocationRecipients)
                .Select(r => r.RecipientOdsCode)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            var recipientIds = allRecipientOds.ToRecipientsString();

            var selectedRecipientId =
                (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId))
                    .Order
                    .AssociatedServicesOnlyDetails
                    ?.PracticeReorganisationOdsCode;

            return RedirectToAction(
                nameof(SelectRecipientForPracticeReorganisation),
                typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
                new { internalOrgId, callOffId, recipientIds, selectedRecipientId });
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
