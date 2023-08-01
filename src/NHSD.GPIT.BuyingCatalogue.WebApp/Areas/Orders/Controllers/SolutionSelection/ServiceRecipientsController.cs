using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
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
        public const char Separator = ',';

        private readonly IOdsService odsService;
        private readonly IOrderService orderService;
        private readonly IOrganisationsService organisationsService;

        public ServiceRecipientsController(
            IOdsService odsService,
            IOrderService orderService,
            IOrganisationsService organisationsService)
        {
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.organisationsService =
                organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
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
            var serviceRecipients = await GetServiceRecipients(internalOrgId);
            var splitImportedRecipients = string.Join(Separator, recipientIds, importedRecipients).Split(Separator, StringSplitOptions.RemoveEmptyEntries);

            var model =
                new SelectRecipientsModel(
                    organisation,
                    serviceRecipients,
                    wrapper.Order.OrderRecipients.Select(x => x.OdsCode),
                    splitImportedRecipients,
                    selectionMode)
                {
                    BackLink =
                        Url.Action(
                            nameof(TaskListController.TaskList),
                            typeof(TaskListController).ControllerName(),
                            new { internalOrgId, callOffId }),
                    Caption = callOffId.ToString(),
                    Advice = "Select the organisations you want to receive the items you’re ordering.",
                    ImportRecipientsLink = Url.Action(
                        nameof(ImportServiceRecipientsController.Index),
                        typeof(ImportServiceRecipientsController).ControllerName(),
                        new { internalOrgId, callOffId }),
                    HasImportedRecipients = !string.IsNullOrWhiteSpace(importedRecipients),
                };

            return View(SelectViewName, model);
        }

        [HttpPost("select-recipients")]
        public IActionResult SelectServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            SelectRecipientsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(SelectViewName, model);
            }

            var recipientIds = string.Join(
                Separator,
                model.GetServiceRecipients().Where(x => x.Selected).Select(x => x.OdsCode));

            return RedirectToAction(
                nameof(ConfirmChanges),
                typeof(ServiceRecipientsController).ControllerName(),
                new { internalOrgId, callOffId, recipientIds, journey = JourneyType.Add });
        }

        [HttpGet("confirm-recipients")]
        public async Task<IActionResult> ConfirmChanges(
            string internalOrgId,
            CallOffId callOffId,
            string recipientIds)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.RolledUp;
            var recipientOdsCodes = recipientIds.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var recipients = await odsService.GetServiceRecipientsById(internalOrgId, recipientOdsCodes);

            var model = new ConfirmChangesModel(order.OrderingParty)
            {
                BackLink = Url.Action(nameof(SelectServiceRecipients), new { internalOrgId, callOffId, recipientIds }),
                Caption = callOffId.ToString(),
                Selected = recipients.Select(x => new ServiceRecipientModel { Name = x.Name, OdsCode = x.OrgId, Location = x.Location }).ToList(),
                Advice = callOffId.IsAmendment
                    ? ConfirmChangesModel.AdditionalAdviceText
                    : ConfirmChangesModel.AdviceText,
            };

            return View(ConfirmViewName, model);
        }

        [HttpPost("confirm-recipients")]
        public async Task<IActionResult> ConfirmChanges(
            string internalOrgId,
            CallOffId callOffId,
            ConfirmChangesModel model)
        {
            // Update Order Recipients
            await Task.Yield();

            return RedirectToAction(
                nameof(TaskListController.TaskList),
                typeof(TaskListController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        private async Task<List<ServiceRecipientModel>> GetServiceRecipients(string internalOrgId)
        {
            var recipients = await odsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId);

            return recipients
                .OrderBy(x => x.Name)
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
