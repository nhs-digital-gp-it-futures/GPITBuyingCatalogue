using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
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
            sessionService.SetObject(model.CallOffId.ToString(), model);
        }

        public CreateOrderItemModel InitialiseStateForCreate(CallOffId callOffId, DateTime? commencementDate, string supplierId, CatalogueItemType catalogueItemType, IEnumerable<CatalogueItemId> solutionIds, OrderItemRecipientModel associatedOrderRecipient)
        {
            var model = GetOrderStateFromSession(callOffId);

            if (model is null)
            {
                model = new CreateOrderItemModel
                {
                    IsNewSolution = true,
                    CallOffId = callOffId,
                    CommencementDate = commencementDate,
                    SupplierId = supplierId,
                    CatalogueItemType = catalogueItemType,
                    SolutionIds = solutionIds,
                };

                if (associatedOrderRecipient is not null)
                    model.ServiceRecipients = new List<OrderItemRecipientModel> { associatedOrderRecipient };

                SetOrderStateToSession(model);
            }

            return model;
        }

        public async Task<CreateOrderItemModel> InitialiseStateForEdit(string odsCode, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var state = GetOrderStateFromSession(callOffId);

            if (state is not null && state.IsNewSolution)
                return state;

            var orderItem = await orderItemService.GetOrderItem(callOffId, catalogueItemId);

            if (state is null)
            {
                var order = await orderService.GetOrder(callOffId);
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
                    TimeUnit = orderItem.CataloguePrice.TimeUnit,
                    PriceId = orderItem.PriceId,
                    CurrencyCode = orderItem.CataloguePrice.CurrencyCode,
                    CurrencySymbol = CurrencyCodeSigns.Code[orderItem.CataloguePrice.CurrencyCode],
                    ProvisioningType = orderItem.CataloguePrice.ProvisioningType,
                    PricingUnit = orderItem.CataloguePrice.PricingUnit,
                };

                if (state.ProvisioningType == ProvisioningType.OnDemand)
                    state.TimeUnit = orderItem.EstimationPeriod;

                if (state.CatalogueItemType == CatalogueItemType.AssociatedService)
                {
                    var organisation = await organisationService.GetOrganisationByOdsCode(odsCode);
                    state.ServiceRecipients = new List<OrderItemRecipientModel> { new() { OdsCode = odsCode, Name = organisation.Name } };
                }
                else
                {
                    var recipients = await odsService.GetServiceRecipientsByParentOdsCode(odsCode);
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

                if (serviceRecipient.DeliveryDate is null)
                    serviceRecipient.DeliveryDate = orderItem.DefaultDeliveryDate;
            }

            SetOrderStateToSession(state);

            return state;
        }

        public CreateOrderItemModel SetPrice(CallOffId callOffId, CataloguePrice cataloguePrice)
        {
            var state = GetOrderStateFromSession(callOffId);

            state.AgreedPrice = cataloguePrice.Price;
            state.PriceId = cataloguePrice.CataloguePriceId;
            state.ProvisioningType = cataloguePrice.ProvisioningType;
            state.CataloguePrice = cataloguePrice.Price;
            state.CurrencyCode = cataloguePrice.CurrencyCode;
            state.CurrencySymbol = CurrencyCodeSigns.Code[cataloguePrice.CurrencyCode];
            state.PricingUnit = cataloguePrice.PricingUnit;
            state.TimeUnit = cataloguePrice.TimeUnit;

            SetOrderStateToSession(state);

            return state;
        }

        public void ClearSession(CallOffId callOffId)
        {
            sessionService.ClearSession(callOffId.ToString());
        }
    }
}
