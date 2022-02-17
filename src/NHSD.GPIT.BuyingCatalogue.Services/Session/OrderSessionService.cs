using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Session
{
    public sealed class OrderSessionService : IOrderSessionService
    {
        private readonly ISessionService sessionService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderService orderService;
        private readonly ISolutionsService solutionsService;
        private readonly IOdsService odsService;
        private readonly IOrganisationsService organisationService;

        public OrderSessionService(
            ISessionService sessionService,
            IOrderItemService orderItemService,
            IOrderService orderService,
            ISolutionsService solutionsService,
            IOdsService odsService,
            IOrganisationsService organisationService)
        {
            this.sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.organisationService = organisationService ?? throw new ArgumentNullException(nameof(organisationService));
        }

        public CreateOrderItemModel GetOrderStateFromSession(CallOffId callOffId)
        {
            return sessionService.GetObject<CreateOrderItemModel>(callOffId.ToString());
        }

        public void SetOrderStateToSession(CreateOrderItemModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            sessionService.SetObject(model.CallOffId.ToString(), model);
        }

        public CreateOrderItemModel InitialiseStateForCreate(
            Order order,
            CatalogueItemType catalogueItemType,
            IEnumerable<CatalogueItemId> solutionIds,
            OrderItemRecipientModel associatedOrderRecipient)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var model = GetOrderStateFromSession(order.CallOffId);

            if (model is not null)
                return model;

            model = new CreateOrderItemModel
            {
                IsNewSolution = true,
                CallOffId = order.CallOffId,
                CommencementDate = order.CommencementDate,
                SupplierId = order.SupplierId,
                CatalogueItemType = catalogueItemType,
                SolutionIds = solutionIds,
            };

            if (associatedOrderRecipient is not null)
                model.ServiceRecipients = new List<OrderItemRecipientModel> { associatedOrderRecipient };

            SetOrderStateToSession(model);

            return model;
        }

        public async Task<CreateOrderItemModel> InitialiseStateForEdit(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var state = GetOrderStateFromSession(callOffId);

            if (state is not null && state.IsNewSolution)
            {
                state.HasHitEditSolution = true;
                SetOrderStateToSession(state);
                return state;
            }

            var orderItem = await orderItemService.GetOrderItem(callOffId, odsCode, catalogueItemId);

            if (state is null)
            {
                var order = await orderService.GetOrderThin(callOffId, odsCode);
                var solution = await solutionsService.GetSolutionListPrices(orderItem.CatalogueItemId);

                state = new CreateOrderItemModel
                {
                    CallOffId = callOffId,
                    IsNewSolution = false,
                    CommencementDate = order.CommencementDate,
                    SupplierId = order.SupplierId,
                    CatalogueItemType = solution.CatalogueItemType,
                    CatalogueItemId = orderItem.CatalogueItemId,
                    CatalogueItemName = solution.Name,
                    AgreedPrice = orderItem.Price,
                    CataloguePrice = orderItem.CataloguePrice,
                    CurrencySymbol = CurrencyCodeSigns.Code[orderItem.CataloguePrice.CurrencyCode],
                    EstimationPeriod = orderItem.EstimationPeriod,
                };

                if (state.CatalogueItemType == CatalogueItemType.AssociatedService)
                {
                    var organisation = await organisationService.GetOrganisationByInternalIdentifier(odsCode);
                    state.ServiceRecipients = new List<OrderItemRecipientModel> { new() { OdsCode = odsCode, Name = organisation.Name } };
                }
                else
                {
                    var recipients = await odsService.GetServiceRecipientsByParentInternalIdentifier(odsCode);
                    state.ServiceRecipients = recipients.Select(r => new OrderItemRecipientModel(r)).ToList();
                }

                foreach (var serviceRecipient in state.ServiceRecipients)
                {
                    var orderRecipient = orderItem.OrderItemRecipients.FirstOrDefault(r => r.OdsCode.EqualsIgnoreCase(serviceRecipient.OdsCode));

                    if (orderRecipient is not null)
                    {
                        serviceRecipient.Selected = true;
                    }
                }
            }

            foreach (var serviceRecipient in state.ServiceRecipients.Where(r => r.Selected))
            {
                var orderRecipient = orderItem.OrderItemRecipients.FirstOrDefault(r => r.OdsCode.EqualsIgnoreCase(serviceRecipient.OdsCode));

                if (orderRecipient is not null)
                {
                    serviceRecipient.Quantity = orderRecipient.Quantity;
                    serviceRecipient.DeliveryDate = orderRecipient.DeliveryDate;
                }

                serviceRecipient.DeliveryDate ??= orderItem.DefaultDeliveryDate;
                serviceRecipient.Quantity ??= state.Quantity;
            }

            state.HasHitEditSolution = true;

            SetOrderStateToSession(state);

            return state;
        }

        public CreateOrderItemModel SetPrice(CallOffId callOffId, CataloguePrice cataloguePrice)
        {
            if (cataloguePrice is null)
                throw new ArgumentNullException(nameof(cataloguePrice));

            var state = GetOrderStateFromSession(callOffId);

            state.AgreedPrice = cataloguePrice.Price;
            state.CataloguePrice = cataloguePrice;
            state.CurrencySymbol = CurrencyCodeSigns.Code[cataloguePrice.CurrencyCode];

            SetOrderStateToSession(state);

            return state;
        }

        public void ClearSession(CallOffId callOffId)
        {
            sessionService.ClearSession(callOffId.ToString());
        }
    }
}
