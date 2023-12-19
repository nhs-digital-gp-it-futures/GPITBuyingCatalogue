using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
    [Route("order/organisation/{internalOrgId}/order/{callOffId}")]
    public class ServiceRecipientsController : Controller
    {
        private const string SelectViewName = "ServiceRecipients/SelectRecipients";
        private const string ConfirmViewName = "ServiceRecipients/ConfirmChanges";
        private const string ConfirmRecipientTitle = "Confirm Service Recipients";
        private const string AdviceText = "Review the organisations you’ve selected to receive the items you’re ordering. ";
        private const string AdditionalAdviceText = "Review the new organisations you’ve selected to receive the items you’re ordering.";

        private readonly IOdsService odsService;
        private readonly IOrderService orderService;
        private readonly IOrderRecipientService orderRecipientService;
        private readonly IOrganisationsService organisationsService;
        private readonly IOrderItemService orderItemService;

        public ServiceRecipientsController(
            IOdsService odsService,
            IOrderService orderService,
            IOrderRecipientService orderRecipientService,
            IOrganisationsService organisationsService,
            IOrderItemService orderItemService)
        {
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderRecipientService =
                orderRecipientService ?? throw new ArgumentNullException(nameof(orderRecipientService));
            this.organisationsService =
                organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.orderItemService =
                orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
        }

        [HttpGet("select-recipients")]
        public async Task<IActionResult> SelectServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            SelectionMode? selectionMode = null,
            string recipientIds = null,
            string importedRecipients = null)
        {
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var possibleServiceRecipients = MapToModel(await odsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId), true);
            var importedRecipientCodes = UrlStringToValues(string.Join(RecipientsConstants.Delimiter, recipientIds, importedRecipients));

            PageTitleModel title = GetSelectServiceRecipientsTitle(wrapper.Order.OrderType);

            var model =
                new SelectRecipientsModel(
                    organisation,
                    possibleServiceRecipients,
                    wrapper.AddedRecipientsOdsCodes(),
                    wrapper.PreviousRecipientsOdsCodes(),
                    importedRecipientCodes,
                    selectionMode)
                {
                    Title = title.Title,
                    Caption = $"Order {callOffId}",
                    Advice = title.Advice,
                    BackLink =
                        Url.Action(
                            nameof(OrderController.Order),
                            typeof(OrderController).ControllerName(),
                            new { internalOrgId, callOffId }),
                    ImportRecipientsLink = Url.Action(
                        nameof(ImportServiceRecipientsController.Index),
                        typeof(ImportServiceRecipientsController).ControllerName(),
                        new { internalOrgId, callOffId }),
                    HasImportedRecipients = !string.IsNullOrWhiteSpace(importedRecipients),
                    SelectAtLeast = wrapper.Order.OrderType.MergerOrSplit
                        ? 2
                        : null,
                };

            return View(SelectViewName, model);
        }

        [HttpPost("select-recipients")]
        public async Task<IActionResult> SelectServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            SelectRecipientsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(SelectViewName, model);
            }

            var recipientIds = model.GetServiceRecipients()
                .Where(x => x.Selected)
                .Select(x => x.OdsCode)
                .ToRecipientsString();

            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            if (wrapper.Order.OrderType.MergerOrSplit)
            {
                var selectedRecipientId = wrapper.Order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode;

                return RedirectToAction(
                    nameof(SelectRecipientForPracticeReorganisation),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId, recipientIds, selectedRecipientId });
            }
            else
            {
                return RedirectToAction(
                    nameof(ConfirmChanges),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId, recipientIds });
            }
        }

        [HttpGet("select-recipient-for-practice-reorganisation")]
        public async Task<IActionResult> SelectRecipientForPracticeReorganisation(
            string internalOrgId,
            CallOffId callOffId,
            string recipientIds,
            string selectedRecipientId)
        {
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            var orderType = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId))
                .Order
                .OrderType;

            if (!orderType.MergerOrSplit)
            {
                return BadRequest($"Expected {callOffId} to be a merger or a split");
            }

            var selectedRecipientOdsCodes = UrlStringToValues(recipientIds);
            var serviceRecipients = MapToModel(await odsService.GetServiceRecipientsById(internalOrgId, selectedRecipientOdsCodes), false);
            var title = GetSelectRecipientForPracticeReorganisationTitle(orderType);

            var model = new RecipientForPracticeReorganisationModel(
                organisation,
                serviceRecipients)
            {
                Title = title.Title,
                Caption = $"Order {callOffId}",
                Advice = title.Advice,
                BackLink = Url.Action(nameof(SelectServiceRecipients), new { internalOrgId, callOffId, recipientIds }),
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
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var orderType = wrapper.Order.OrderType;
            var selectedRecipients = MapToModel(
                await odsService.GetServiceRecipientsById(internalOrgId, UrlStringToValues(recipientIds)),
                false);
            ServiceRecipientModel practiceReorganisation = null;

            if (orderType.MergerOrSplit)
            {
                practiceReorganisation = selectedRecipients.FirstOrDefault(r => r.OdsCode == selectedRecipientId);
                if (practiceReorganisation == null)
                {
                    throw new InvalidOperationException($"The selected merger or split recipient {selectedRecipientId} isn't in the list of recipients");
                }

                selectedRecipients.Remove(practiceReorganisation);
            }

            var previouslySelectedIds = wrapper.Previous?.OrderRecipients
                ?.Select(x => x.OdsCode)
                .ToList() ?? Enumerable.Empty<string>();

            var previousRecipients = await odsService.GetServiceRecipientsById(internalOrgId, previouslySelectedIds);

            var title = GetConfirmRecipientsTitle(orderType, callOffId.IsAmendment);
            var model = new ConfirmChangesModel()
            {
                Title = title.Title,
                Caption = $"Order {callOffId}",
                Advice = title.Advice,
                OrderType = orderType,
                BackLink = orderType.MergerOrSplit
                    ? Url.Action(nameof(SelectRecipientForPracticeReorganisation), new { internalOrgId, callOffId, recipientIds, selectedRecipientId })
                    : Url.Action(nameof(SelectServiceRecipients), new { internalOrgId, callOffId, recipientIds }),
                AddRemoveRecipientsLink = Url.Action(nameof(SelectServiceRecipients), new { internalOrgId, callOffId, recipientIds }),
                Selected = selectedRecipients,
                PracticeReorganisationRecipient = practiceReorganisation,
                PreviouslySelected = MapToModel(previousRecipients, false),
            };

            return View(ConfirmViewName, model);
        }

        [HttpPost("confirm-recipients")]
        public async Task<IActionResult> ConfirmChanges(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmChangesModel model)
        {
            if (model.OrderType.MergerOrSplit)
            {
                await orderService.SetOrderPracticeReorganisationRecipient(internalOrgId, callOffId, model.PracticeReorganisationRecipient.OdsCode);
            }

            await orderRecipientService.SetOrderRecipients(internalOrgId, callOffId, model.Selected.Select(x => x.OdsCode));

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        private static PageTitleModel GetSelectServiceRecipientsTitle(OrderType orderType)
        {
            return orderType.Value switch
            {
                OrderTypeEnum.AssociatedServiceSplit => new()
                {
                    Title = "Service Recipients splitting",
                    Advice = "Select all the practices that will be involved in the split you’re ordering. They must all be using the same Catalogue Solution.",
                },
                OrderTypeEnum.AssociatedServiceMerger => new()
                {
                    Title = "Service Recipients merging",
                    Advice = "Select all the practices that will be involved in the merger you’re ordering. They must all be using the same Catalogue Solution.",
                },
                _ => new()
                {
                    Title = "Service Recipients for this order",
                    Advice = "Select the organisations you want to receive the items you’re ordering.",
                },
            };
        }

        private static PageTitleModel GetSelectRecipientForPracticeReorganisationTitle(OrderType orderType)
        {
            return new()
            {
                Title = orderType.GetPracticeReorganisationRecipientTitle(),
                Advice = orderType.Value switch
                {
                    OrderTypeEnum.AssociatedServiceSplit => "Select the Service Recipient that will be losing patients as part of the split.",
                    OrderTypeEnum.AssociatedServiceMerger => "Select the Service Recipient that will still exist after the merger.",
                    _ => throw new InvalidOperationException($"Unsupported orderType {orderType.Value} in {nameof(GetSelectRecipientForPracticeReorganisationTitle)}"),
                },
            };
        }

        private static PageTitleModel GetConfirmRecipientsTitle(OrderType orderType, bool isAmendment)
        {
            return orderType.Value switch
            {
                OrderTypeEnum.AssociatedServiceSplit => new()
                {
                    Title = ConfirmRecipientTitle,
                    Advice = "Review the practices involved in the split you’re ordering.",
                },
                OrderTypeEnum.AssociatedServiceMerger => new()
                {
                    Title = ConfirmRecipientTitle,
                    Advice = "Review the practices involved in the merger you’re ordering.",
                },
                OrderTypeEnum.Solution or OrderTypeEnum.AssociatedServiceOther => new()
                {
                    Title = ConfirmRecipientTitle,
                    Advice = isAmendment
                        ? AdditionalAdviceText
                        : AdviceText,
                },
                _ => throw new InvalidOperationException($"Unsupported orderType {orderType.Value} in {nameof(GetConfirmRecipientsTitle)}"),
            };
        }

        private static string[] UrlStringToValues(string recipientIds)
        {
            return recipientIds.Split(RecipientsConstants.Delimiter, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        private List<ServiceRecipientModel> MapToModel(IEnumerable<ServiceRecipient> recipients, bool orderByName)
        {
            if (orderByName)
            {
                recipients = recipients.OrderBy(x => x.Name);
            }

            return recipients
                .Select(x => new ServiceRecipientModel
                {
                    Name = x.Name,
                    OdsCode = x.OrgId,
                    Location = x.Location,
                })
                .ToList();
        }
    }
}
