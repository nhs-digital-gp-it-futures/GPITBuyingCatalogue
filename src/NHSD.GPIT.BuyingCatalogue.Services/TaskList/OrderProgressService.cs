using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList
{
    public class OrderProgressService : IOrderProgressService
    {
        private readonly IOrderService orderService;
        private readonly IOrderTaskProgressProviderService providerService;

        public OrderProgressService(IOrderService orderService, IOrderTaskProgressProviderService providerService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.providerService = providerService ?? throw new ArgumentNullException(nameof(providerService));
        }

        public async Task<OrderProgress> GetOrderProgress(string internalOrgId, CallOffId callOffId)
        {
            var wrapper = await orderService.GetOrderForTaskListStatuses(callOffId, internalOrgId);
            var output = new OrderProgress();

            output.DescriptionStatus = providerService.ProviderFor(OrderTaskListStatus.Description).Get(wrapper, output);
            output.OrderingPartyStatus = providerService.ProviderFor(OrderTaskListStatus.OrderingParty).Get(wrapper, output);
            output.SupplierStatus = providerService.ProviderFor(OrderTaskListStatus.Supplier).Get(wrapper, output);
            output.CommencementDateStatus = providerService.ProviderFor(OrderTaskListStatus.CommencementDate).Get(wrapper, output);

            output.ServiceRecipients = providerService.ProviderFor(OrderTaskListStatus.ServiceRecipients).Get(wrapper, output);
            output.SolutionOrService = providerService.ProviderFor(OrderTaskListStatus.SolutionsOrServices).Get(wrapper, output);
            output.DeliveryDates = providerService.ProviderFor(OrderTaskListStatus.DeliveryDates).Get(wrapper, output);
            output.FundingSource = providerService.ProviderFor(OrderTaskListStatus.FundingSources).Get(wrapper, output);

            output.ImplementationPlan = providerService.ProviderFor(OrderTaskListStatus.ImplementationPlan).Get(wrapper, output);
            output.AssociatedServiceBilling = providerService.ProviderFor(OrderTaskListStatus.AssociatedServicesBilling).Get(wrapper, output);
            output.DataProcessingInformation = providerService.ProviderFor(OrderTaskListStatus.DataProcessing).Get(wrapper, output);

            output.ReviewAndCompleteStatus = providerService.ProviderFor(OrderTaskListStatus.ReviewAndComplete).Get(wrapper, output);

            return output;
        }
    }
}
