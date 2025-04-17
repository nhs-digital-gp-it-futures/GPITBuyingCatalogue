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
        private const string UploadOrSelectViewName = "ServiceRecipients/UploadOrSelectServiceRecipient";

        private readonly IOdsService odsService;
        private readonly IOrderService orderService;
        private readonly IOrderRecipientService orderRecipientService;
        private readonly IOrderSublocationService orderSublocationService;
        private readonly IOrganisationsService organisationsService;
        private readonly IOrderItemService orderItemService;

        public ServiceRecipientsController(
            IOdsService odsService,
            IOrderService orderService,
            IOrderRecipientService orderRecipientService,
            IOrderSublocationService orderSublocationService,
            IOrganisationsService organisationsService,
            IOrderItemService orderItemService)
        {
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderRecipientService =
                orderRecipientService ?? throw new ArgumentNullException(nameof(orderRecipientService));
            this.orderSublocationService =
                orderSublocationService ?? throw new ArgumentNullException(nameof(orderSublocationService));
            this.organisationsService =
                organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.orderItemService =
                orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
        }

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
            return View(UploadOrSelectViewName, model);
        }

        [HttpPost("upload-or-select-service-recipients")]
        public IActionResult UploadOrSelectServiceRecipients(
            UploadOrSelectServiceRecipientModel model,
            string internalOrgId,
            CallOffId callOffId)
        {
            if (!ModelState.IsValid)
                return View(UploadOrSelectViewName, model);

            if (model.ShouldUploadRecipients.GetValueOrDefault())
            {
                return RedirectToAction(
                    nameof(Index),
                    typeof(ImportServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            return RedirectToAction(
                nameof(SelectServiceRecipients),
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

            var backLinkHref = Url.Action(
                nameof(UploadOrSelectServiceRecipients),
                typeof(ServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });

            var model = new SelectSublocationsModel(
                wrapper.Order,
                possibleSublocations,
                backLinkHref);
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

            OrderWrapper order =
                await orderService.GetOrderWithSublocations(callOffId, internalOrgId);

            HashSet<string> orderSublocations =
                order.Order.OrderSublocations.Select(x => x.SublocationOdsCode).ToHashSet();

            HashSet<string> removes = [.. orderSublocations];
            removes.ExceptWith(sublocationOdsCodes);

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

        [HttpGet("add-sublocations")]
        public async Task<IActionResult> AddSublocations(string internalOrgId, CallOffId callOffId)
        {
            var backLink = Url.Action(
                nameof(SelectSublocations),
                typeof(ServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });

            return await SelectSublocationsOverview(callOffId, internalOrgId, false, backLink);
        }

        [HttpPost("add-sublocations")]
        public IActionResult AddSublocations(
            SelectSublocationsOverviewModel model,
            string internalOrgId,
            CallOffId callOffId)
        {
            return SelectSublocationsOverviewDynamicRedirect(model, internalOrgId, callOffId);
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

            var backLinkHref = Url.Action(
                nameof(ConfirmSublocations),
                typeof(ServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });

            var model = new RemoveSublocationsModel(
                wrapper.Order,
                parsedSublocations,
                parsedRemoves,
                backLinkHref);

            return View("ServiceRecipients/RemoveSublocations", model);
        }

        [HttpPost("remove-sublocations")]
        public async Task<IActionResult> RemoveSublocations(
            RemoveSublocationsModel removeSublocationsModel,
            string internalOrgId,
            CallOffId callOffId)
        {
            if (removeSublocationsModel.ConfirmRemove is not true || removeSublocationsModel.SublocationOdsCodes is not
                    { Count: > 0 })
            {
                return BadRequest();
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

            OrderSublocation orderSublocation =
                await orderSublocationService.GetOrderSublocationWithRecipients(
                    externalOrganisationId,
                    callOffId,
                    sublocationOdsCode);

            if (orderSublocation is null)
            {
                return NotFound();
            }

            var sublocationAsSublocationModel = new SublocationModel(orderSublocation, true);

            List<ServiceRecipientModel> possibleRecipients =
                await GetServiceRecipientModelsBySublocation(sublocationOdsCode);

            var backLinkHref = Url.Action(
                nameof(ConfirmSublocations),
                typeof(ServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });

            var model = new SelectSublocationRecipientsModel(
                orderSublocation.Order,
                sublocationAsSublocationModel,
                possibleRecipients,
                backLinkHref,
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

            OrderSublocation sublocation =
                await orderSublocationService.GetOrderSublocationWithRecipients(
                    externalOrganisationId,
                    callOffId,
                    sublocationOdsCode);

            if (sublocation is null)
            {
                return BadRequest();
            }

            HashSet<string> pageSelections = selectSublocationRecipientsModel.RenderedServiceRecipients
                .Where(x => x.Selected)
                .Select(y => y.OdsCode)
                .ToHashSet();

            await orderSublocationService.SetSublocationRecipients(
                externalOrganisationId,
                callOffId,
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
                nameof(TaskListController.TaskList),
                typeof(TaskListController).ControllerName(),
                new { callOffId, internalOrgId });

            return await SelectSublocationsOverview(callOffId, internalOrgId, true, backLink);
        }

        [HttpPost("confirm-sublocations")]
        public IActionResult ConfirmSublocations(
            SelectSublocationsOverviewModel model,
            string internalOrgId,
            CallOffId callOffId)
        {
            return SelectSublocationsOverviewDynamicRedirect(model, internalOrgId, callOffId);
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
                nameof(TaskListController.TaskList),
                typeof(TaskListController).ControllerName(),
                new { callOffId, internalOrgId });

            var model = new ConfirmSublocationRecipientsModel(
                wrapper.Order,
                backLinkUrl,
                continueLinkUrl);

            return View("ServiceRecipients/ConfirmSublocationRecipients", model);
        }

        [HttpPost("confirm-sublocation-recipients")]
        public IActionResult ConfirmSublocationRecipientsPost(
            string internalOrgId,
            CallOffId callOffId)
        {
            return RedirectToAction(
                nameof(TaskListController.TaskList),
                typeof(TaskListController).ControllerName(),
                new { callOffId, internalOrgId });
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

            IEnumerable<ServiceRecipient> previousRecipients =
                await odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                    internalOrgId,
                    wrapper.PreviousRecipientsOdsCodes());
            var previousRecipientsModel = MapToModel(previousRecipients, false);

            var model =
                new SelectRecipientsModel(
                    organisation,
                    possibleServiceRecipients,
                    wrapper.AddedRecipientsOdsCodes(),
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
                                nameof(UploadOrSelectServiceRecipients),
                                typeof(ServiceRecipientsController).ControllerName(),
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
            List<ServiceRecipientModel> serviceRecipients = MapToModel(
                await odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                    internalOrgId,
                    selectedRecipientOdsCodes),
                false);
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
            string selectedRecipientId,
            bool? hasImported = null)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var orderType = wrapper.Order.OrderType;
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
                    throw new InvalidOperationException($"The selected merger or split recipient {selectedRecipientId} isn't in the list of recipients");
                }

                selectedRecipients.Remove(practiceReorganisation);
            }

            IEnumerable<ServiceRecipient> previousRecipients =
                await odsService.GetServiceRecipientsByParentInternalIdentifierAndOdsCodes(
                    internalOrgId,
                    wrapper.PreviousRecipientsOdsCodes());

            var title = GetConfirmRecipientsTitle(orderType, callOffId.IsAmendment);
            var model = new ConfirmChangesModel()
            {
                Title = title.Title,
                Caption = $"Order {callOffId}",
                Advice = title.Advice,
                OrderType = orderType,
                BackLink = orderType.MergerOrSplit
                    ? Url.Action(
                        nameof(SelectRecipientForPracticeReorganisation),
                        new { internalOrgId, callOffId, recipientIds, selectedRecipientId })
                    : hasImported.GetValueOrDefault()
                        ? Url.Action(
                            nameof(ImportServiceRecipientsController.Index),
                            typeof(ImportServiceRecipientsController).ControllerName(),
                            new { internalOrgId, callOffId })
                        : Url.Action(nameof(SelectServiceRecipients), new { internalOrgId, callOffId, recipientIds }),
                AddRemoveRecipientsLink =
                    Url.Action(nameof(SelectServiceRecipients), new { internalOrgId, callOffId, recipientIds }),
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

        private async Task<IActionResult> SelectSublocationsOverview(
            CallOffId callOffId,
            string internalOrgId,
            bool isConfirm,
            string backLinkHref)
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

            var addOrChangeSublocationsHref = Url.Action(
                nameof(SelectSublocations),
                typeof(ServiceRecipientsController).ControllerName(),
                new { callOffId, internalOrgId });

            var model = new SelectSublocationsOverviewModel(
                isConfirm,
                wrapper.Order,
                sublocations,
                addOrChangeSublocationsHref,
                backLinkHref);

            return View("ServiceRecipients/SelectSublocationsOverview", model);

            async Task MapSublocationToSublocationModel(OrderSublocation competitionSublocation)
            {
                var recipientHref = Url.Action(
                    nameof(SelectSublocationRecipients),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { callOffId, internalOrgId, sublocationOdsCode = competitionSublocation.SublocationOdsCode });

                var serviceRecipientCount =
                    await orderSublocationService.GetCountForOrderSublocationRecipients(
                        wrapper.Order.OrderingParty.ExternalIdentifier,
                        callOffId,
                        competitionSublocation.SublocationOdsCode);

                var sublocationModel = new SublocationModel(
                    competitionSublocation,
                    recipientHref,
                    serviceRecipientCount);
                sublocations.Add(sublocationModel);
            }
        }

        private RedirectToActionResult SelectSublocationsOverviewDynamicRedirect(
            SelectSublocationsOverviewModel model,
            string internalOrgId,
            CallOffId callOffId)
        {
            var sublocationToComplete = model.Sublocations.Any(x => x.ServiceRecipientCount == 0);

            if (sublocationToComplete)
            {
                return RedirectToAction(
                    nameof(TaskListController.TaskList),
                    typeof(TaskListController).ControllerName(),
                    new { callOffId, internalOrgId });
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
