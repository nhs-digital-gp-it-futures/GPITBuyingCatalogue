using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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

        public CreateOrderItemModel GetOrderStateFromSession()
        {
            return sessionService.GetObject<CreateOrderItemModel>("CatalogueItemState");
        }

        public void SetOrderStateToSession(CreateOrderItemModel model)
        {
            sessionService.SetObject("CatalogueItemState", model);
        }

        public async Task<bool> InitialiseStateForEdit(string odsCode, CallOffId callOffId, CatalogueItemId catalogueSolutionId)
        {
            var state = GetOrderStateFromSession();

            var orderItem = await orderItemService.GetOrderItem(callOffId, catalogueSolutionId);

            if (orderItem is null)
            {
                foreach (var serviceRecipient in state.ServiceRecipients.Where(sr => sr.Selected))
                {
                    serviceRecipient.Quantity ??= state.Quantity;
                    serviceRecipient.DeliveryDate ??= state.PlannedDeliveryDate;
                }

                SetOrderStateToSession(state);

                return true;
            }

            if (state?.CatalogueItemId is not null)
            {
                foreach (var recipient in state.ServiceRecipients.Where(x => !x.DeliveryDate.HasValue))
                    recipient.DeliveryDate = orderItem.DefaultDeliveryDate;

                foreach (var serviceRecipient in state.ServiceRecipients)
                {
                    var orderRecipient = orderItem.OrderItemRecipients.FirstOrDefault(r => r.OdsCode.EqualsIgnoreCase(serviceRecipient.OdsCode));

                    if (orderRecipient is not null)
                    {
                        serviceRecipient.Selected = true;
                        serviceRecipient.Quantity = orderRecipient.Quantity;
                        serviceRecipient.DeliveryDate = orderRecipient.DeliveryDate;
                    }
                }

                SetOrderStateToSession(state);

                return false;
            }

            var order = await orderService.GetOrder(callOffId);

            var solution = await solutionsService.GetSolution(orderItem.CatalogueItemId);

            state = new CreateOrderItemModel
            {
                IsNewOrder = false,
                CommencementDate = order.CommencementDate,
                SupplierId = order.SupplierId,
                CatalogueItemType = solution.CatalogueItemType,
                CatalogueItemId = orderItem.CatalogueItemId,
                CatalogueItemName = solution.Name,
                Price = orderItem.Price,
                ItemUnit = new ItemUnitModel
                {
                    Name = orderItem.CataloguePrice.PricingUnit.Name,
                    Description = orderItem.CataloguePrice.PricingUnit.Description,
                },
                EstimationPeriod = orderItem.EstimationPeriod,
                PriceId = orderItem.PriceId,
                ProvisioningType = orderItem.CataloguePrice.ProvisioningType,
                Type = orderItem.CataloguePrice.CataloguePriceType,

                // TODO: this isn't right – only additional services have a parent catalogue item
                // CatalogueSolutionId = orderItem.CatalogueItem.ParentCatalogueItemId?.ToString(),
            };

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
                var orderRecipient = orderItem.OrderItemRecipients.FirstOrDefault(x => x.OdsCode.EqualsIgnoreCase(serviceRecipient.OdsCode));

                if (orderRecipient != null)
                {
                    serviceRecipient.Selected = true;
                    serviceRecipient.Quantity = orderRecipient.Quantity;
                    serviceRecipient.DeliveryDate = orderRecipient.DeliveryDate;
                }
            }

            SetOrderStateToSession(state);

            return false;
        }

        public void SetPrice(CataloguePrice cataloguePrice)
        {
            var state = GetOrderStateFromSession();

            state.ItemUnit = new ItemUnitModel { Name = cataloguePrice.PricingUnit.Name, Description = cataloguePrice.PricingUnit.Description };
            state.Price = cataloguePrice.Price;
            state.PriceId = cataloguePrice.CataloguePriceId;
            state.ProvisioningType = cataloguePrice.ProvisioningType;

            SetOrderStateToSession(state);
        }
    }
}
