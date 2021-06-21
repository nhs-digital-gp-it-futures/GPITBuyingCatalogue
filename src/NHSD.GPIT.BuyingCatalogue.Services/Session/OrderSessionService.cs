using System;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Session;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Session
{
    public class OrderSessionService : IOrderSessionService
    {
        private readonly ISessionService sessionService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderService orderService;
        private readonly ISolutionsService solutionsService;
        private readonly IOdsService odsService;

        public OrderSessionService(
            ISessionService sessionService,
            IOrderItemService orderItemService,
            IOrderService orderService,
            ISolutionsService solutionsService,
            IOdsService odsService)
        {
            this.sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
        }

        public CreateOrderItemModel GetOrderStateFromSession()
        {
            return sessionService.GetObject<CreateOrderItemModel>("CatalogueItemState");
        }

        public void SetOrderStateToSession(CreateOrderItemModel model)
        {
            sessionService.SetObject("CatalogueItemState", model);
        }

        public async Task<bool> InitialiseStateForEdit(string odsCode, string callOffId, string catalogueSolutionId)
        {
            var state = GetOrderStateFromSession();

            var orderItem = await orderItemService.GetOrderItem(callOffId, catalogueSolutionId);

            if (state is not null && state.CatalogueSolutionId is not null)
            {
                if (state.IsNewOrder)
                    return true;

                foreach (var recipient in state.ServiceRecipients.Where(x => !x.DeliveryDate.HasValue))
                    recipient.DeliveryDate = orderItem.DefaultDeliveryDate;

                SetOrderStateToSession(state);

                return false;
            }

            var order = await orderService.GetOrder(callOffId);

            var solution = await solutionsService.GetSolution(orderItem.CatalogueItemId.ToString());

            state = new CreateOrderItemModel
            {
                IsNewOrder = false,
                CommencementDate = order.CommencementDate,
                SupplierId = order.SupplierId,
                CatalogueItemType = CatalogueItemType.Solution,
                CatalogueSolutionId = orderItem.CatalogueItemId.ToString(),
                CatalogueItemName = solution.Name,
                Price = orderItem.Price,
                ItemUnit = new ItemUnitModel { Name = orderItem.PricingUnit.Name, Description = orderItem.PricingUnit.Description },
                TimeUnit = orderItem.PriceTimeUnit,
                ProvisioningType = orderItem.ProvisioningType,
                CurrencyCode = orderItem.CurrencyCode,
                EstimationPeriod = orderItem.EstimationPeriod,
                PriceId = orderItem.PriceId,
                Type = orderItem.CataloguePriceType,
            };

            var recipients = await odsService.GetServiceRecipientsByParentOdsCode(odsCode);
            state.ServiceRecipients = recipients.Select(x => new OrderItemRecipientModel(x)).ToList();

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

        public void SetPrice(EntityFramework.Models.GPITBuyingCatalogue.CataloguePrice cataloguePrice)
        {
            var state = GetOrderStateFromSession();

            state.ProvisioningType = ProvisioningType.Parse(cataloguePrice.ProvisioningType.Name);
            state.CurrencyCode = cataloguePrice.CurrencyCode;
            state.Type = CataloguePriceType.Parse(cataloguePrice.CataloguePriceType.Name);
            state.ItemUnit = new ItemUnitModel { Name = cataloguePrice.PricingUnit.Name, Description = cataloguePrice.PricingUnit.Description };
            if (cataloguePrice.TimeUnit != null)
                state.TimeUnit = TimeUnit.Parse(cataloguePrice.TimeUnit.Name);
            state.Price = cataloguePrice.Price;
            state.PriceId = cataloguePrice.CataloguePriceId;
            state.EstimationPeriod = TimeUnit.PerYear;

            SetOrderStateToSession(state);
        }
    }
}
