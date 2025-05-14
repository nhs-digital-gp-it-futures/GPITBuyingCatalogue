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

            List<string> preSelectedRecipients = wrapper.FlattenedRecipients.Select(x => x.Id).ToList();

            if (preSelectedRecipients.Count > 0
                && wrapper.Order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode is not null)
            {
                preSelectedRecipients.Add(wrapper.Order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode);
            }

            var model =
                new SelectRecipientsModel(
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
                typeof(MergerOrSplitServiceRecipientsController).ControllerName(),
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

            var model = new RecipientForPracticeReorganisationModel(
                organisation,
                callOffId,
                orderType,
                serviceRecipients) { SelectedOdsCode = selectedRecipientId };
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

            practiceReorganisation = selectedRecipients.FirstOrDefault(r => r.OdsCode == selectedRecipientId);

            if (practiceReorganisation == null)
            {
                throw new InvalidOperationException(
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
                AddRemoveRecipientsLink = "",
            };

            return View(model);
        }

        [HttpPost("confirm-recipients")]
        public async Task<IActionResult> ConfirmChanges(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmChangesModel model)
        {
            var orderId = await orderService.GetOrderId(internalOrgId, callOffId);

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
                recipientsAsSublocationModel.Select(x => new OrderSublocation
                    {
                        OrderId = orderId,
                        OwnerOdsCode = organisation.ExternalIdentifier,
                        SublocationOdsCode = x.OdsCode,
                        SublocationRecipients = x.ServiceRecipients.Select(y => new OrderSublocationRecipient
                            {
                                OrderId = orderId,
                                ParentSublocationOdsCode = x.OdsCode,
                                RecipientOdsCode = y.OdsCode,
                            })
                            .ToList(),
                    })
                    .ToList();

            await orderService.SetSublocationsAndRecipients(callOffId, internalOrgId, sublocationsAsEntityModel);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
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
