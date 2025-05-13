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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.ServiceRecipients;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.ServiceRecipientModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/merger-or-split-service-recipients")]
    public class MergerOrSplitServiceRecipientsController(
        IOdsService odsService,
        IOrderService orderService,
        IOrderSublocationService orderSublocationService,
        IOrganisationsService organisationsService,
        IOrderItemService orderItemService)
        : Controller
    {
        private const string ConfirmRecipientTitle = "Confirm Service Recipients";

        private readonly IOdsService odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));

        private readonly IOrderService orderService =
            orderService ?? throw new ArgumentNullException(nameof(orderService));

        private readonly IOrderSublocationService orderSublocationService =
            orderSublocationService ?? throw new ArgumentNullException(nameof(orderSublocationService));

        private readonly IOrganisationsService organisationsService =
            organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));

        private readonly IOrderItemService orderItemService =
            orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));

        [HttpGet("select-recipients")]
        public async Task<IActionResult> SelectServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            SelectionMode? selectionMode = null,
            string recipientIds = null,
            string importedRecipients = null)
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
            var importedRecipientCodes = UrlStringToValues(
                string.Join(RecipientsConstants.Delimiter, recipientIds, importedRecipients));

            PageTitleModel title = GetSelectMergeOrSplitServiceRecipientsTitle(wrapper.Order.OrderType);

            IEnumerable<ServiceRecipient> previousRecipients =
                await odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                    internalOrgId,
                    wrapper.Previous.FlattenedRecipients.Select(x => x.RecipientOdsCode));
            List<ServiceRecipientModel> previousRecipientsModel = MapToModel(previousRecipients, false);

            IEnumerable<string> addedRecipientOdsCodes = wrapper.Order.AddedOrderRecipients(wrapper.Previous)
                .Select(r => r.RecipientOdsCode);

            if (wrapper.Order.AssociatedServicesOnlyDetails?.PracticeReorganisationOdsCode is not null)
            {
                addedRecipientOdsCodes = addedRecipientOdsCodes.Append(
                    wrapper.Order.AssociatedServicesOnlyDetails!.PracticeReorganisationOdsCode);
            }

            var model =
                new SelectRecipientsModel(
                    organisation,
                    possibleServiceRecipients,
                    addedRecipientOdsCodes,
                    previousRecipientsModel,
                    importedRecipientCodes,
                    selectionMode,
                    wrapper.IsAmendment)
                {
                    Title = title.Title,
                    Caption = $"Order {callOffId}",
                    Advice = title.Advice,
                    BackLink =
                        wrapper.Order.OrderType.MergerOrSplit
                            ? Url.Action(
                                nameof(OrderController.Order),
                                typeof(OrderController).ControllerName(),
                                new { internalOrgId, callOffId })
                            : Url.Action(
                                nameof(ServiceRecipientsController.UploadOrSelectServiceRecipients),
                                typeof(ServiceRecipientsController).ControllerName(),
                                new { internalOrgId, callOffId }),
                    HasImportedRecipients = !string.IsNullOrWhiteSpace(importedRecipients),
                    SelectAtLeast = wrapper.Order.OrderType.MergerOrSplit
                        ? 2
                        : null,
                };

            return View(model);
        }

        [HttpPost("select-recipients")]
        public async Task<IActionResult> SelectServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            SelectRecipientsModel model)
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
                typeof(ServiceRecipientsController).ControllerName(),
                new { internalOrgId, callOffId, recipientIds, selectedRecipientId });
        }

        [HttpGet("select-recipient-for-practice-reorganisation")]
        public async Task<IActionResult> SelectRecipientForPracticeReorganisation(
            string internalOrgId,
            CallOffId callOffId,
            string recipientIds,
            string selectedRecipientId)
        {
            Organisation organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            OrderType orderType = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId))
                .Order
                .OrderType;

            var selectedRecipientOdsCodes = UrlStringToValues(recipientIds);
            List<ServiceRecipientModel> serviceRecipients = MapToModel(
                await odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                    internalOrgId,
                    selectedRecipientOdsCodes),
                false);
            PageTitleModel title = GetSelectRecipientForPracticeReorganisationTitle(orderType);

            var model = new RecipientForPracticeReorganisationModel(
                organisation,
                serviceRecipients)
            {
                Title = title.Title,
                Caption = $"Order {callOffId}",
                Advice = title.Advice,
                BackLink = "",
                SelectedOdsCode = selectedRecipientId,
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
                typeof(ServiceRecipientsController).ControllerName(),
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
            ServiceRecipientModel practiceReorganisation = null;

            if (orderType.MergerOrSplit)
            {
                practiceReorganisation = selectedRecipients.FirstOrDefault(r => r.OdsCode == selectedRecipientId);
                if (practiceReorganisation == null)
                {
                    throw new InvalidOperationException(
                        $"The selected merger or split recipient {selectedRecipientId} isn't in the list of recipients");
                }

                selectedRecipients.Remove(practiceReorganisation);
            }

            IEnumerable<ServiceRecipient> previousRecipients =
                await odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                    internalOrgId,
                    wrapper.Previous.FlattenedRecipients.Select(x => x.RecipientOdsCode));

            PageTitleModel title = GetConfirmRecipientsTitle(orderType);
            var model = new ConfirmChangesModel
            {
                Title = title.Title,
                Caption = $"Order {callOffId}",
                Advice = title.Advice,
                OrderType = orderType,
                BackLink = Url.Action(
                    nameof(SelectRecipientForPracticeReorganisation),
                    new { internalOrgId, callOffId, recipientIds, selectedRecipientId }),
                AddRemoveRecipientsLink = "",
                Selected = selectedRecipients,
                PracticeReorganisationRecipient = practiceReorganisation,
                PreviouslySelected = MapToModel(previousRecipients, false),
            };

            return View(model);
        }

        [HttpPost("confirm-recipients")]
        public async Task<IActionResult> ConfirmChanges(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmChangesModel model)
        {
            if (model.OrderType.MergerOrSplit)
            {
                await orderService.SetOrderPracticeReorganisationRecipient(
                    internalOrgId,
                    callOffId,
                    model.PracticeReorganisationRecipient.OdsCode);
            }

            // await orderRecipientService.SetOrderRecipients(internalOrgId, callOffId, model.Selected.Select(x => x.OdsCode));

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        private static PageTitleModel GetSelectMergeOrSplitServiceRecipientsTitle(OrderType orderType)
        {
            return orderType.Value switch
            {
                OrderTypeEnum.AssociatedServiceSplit => new PageTitleModel
                {
                    Title = "Service Recipients splitting",
                    Advice =
                        "Select all the practices that will be involved in the split you’re ordering. They must all be using the same Catalogue Solution.",
                },
                OrderTypeEnum.AssociatedServiceMerger => new PageTitleModel
                {
                    Title = "Service Recipients merging",
                    Advice =
                        "Select all the practices that will be involved in the merger you’re ordering. They must all be using the same Catalogue Solution.",
                },
                _ => throw new ArgumentOutOfRangeException(nameof(orderType)),
            };
        }

        private static PageTitleModel GetSelectRecipientForPracticeReorganisationTitle(OrderType orderType)
        {
            return new PageTitleModel
            {
                Title = orderType.GetPracticeReorganisationRecipientTitle(),
                Advice = orderType.Value switch
                {
                    OrderTypeEnum.AssociatedServiceSplit =>
                        "Select the Service Recipient that will be losing patients as part of the split.",
                    OrderTypeEnum.AssociatedServiceMerger =>
                        "Select the Service Recipient that will still exist after the merger.",
                    _ => throw new InvalidOperationException(
                        $"Unsupported orderType {orderType.Value} in {nameof(GetSelectRecipientForPracticeReorganisationTitle)}"),
                },
            };
        }

        private static PageTitleModel GetConfirmRecipientsTitle(OrderType orderType)
        {
            return orderType.Value switch
            {
                OrderTypeEnum.AssociatedServiceSplit => new PageTitleModel
                {
                    Title = ConfirmRecipientTitle,
                    Advice = "Review the practices involved in the split you’re ordering.",
                },
                OrderTypeEnum.AssociatedServiceMerger => new PageTitleModel
                {
                    Title = ConfirmRecipientTitle,
                    Advice = "Review the practices involved in the merger you’re ordering.",
                },
                _ => throw new ArgumentOutOfRangeException(nameof(orderType)),
            };
        }

        private static string[] UrlStringToValues(string recipientIds)
        {
            return recipientIds.Split(
                RecipientsConstants.Delimiter,
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        private List<ServiceRecipientModel> MapToModel(IEnumerable<ServiceRecipient> recipients, bool orderByName)
        {
            if (orderByName)
            {
                recipients = recipients.OrderBy(x => x.Name);
            }

            return recipients
                .Select(x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OrgId, Location = x.Location })
                .ToList();
        }
    }
}
