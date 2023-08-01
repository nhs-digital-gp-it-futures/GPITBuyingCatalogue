using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList
{
    public class OrderTaskProgressProviderService : IOrderTaskProgressProviderService
    {
        private readonly Dictionary<OrderTaskListStatus, ITaskProgressProvider> providers = new();

        public ITaskProgressProvider ProviderFor(OrderTaskListStatus status)
        {
            if (!providers.ContainsKey(status))
            {
                providers.Add(status, GetProvider(status));
            }

            return providers[status];
        }

        private static ITaskProgressProvider GetProvider(OrderTaskListStatus status)
        {
            return status switch
            {
                OrderTaskListStatus.Description => new DescriptionStatusProvider(),
                OrderTaskListStatus.OrderingParty => new OrderingPartyStatusProvider(),
                OrderTaskListStatus.Supplier => new SupplierStatusProvider(),
                OrderTaskListStatus.CommencementDate => new CommencementDateStatusProvider(),

                OrderTaskListStatus.ServiceRecipients => new ServiceRecipientsStatusProvider(),
                OrderTaskListStatus.SolutionsOrServices => new SolutionOrServiceStatusProvider(),
                OrderTaskListStatus.DeliveryDates => new DeliveryDatesStatusProvider(),
                OrderTaskListStatus.FundingSources => new FundingSourceStatusProvider(),

                OrderTaskListStatus.ImplementationPlan => new ImplementationPlanStatusProvider(),
                OrderTaskListStatus.AssociatedServicesBilling => new AssociatedServicesBillingStatusProvider(),
                OrderTaskListStatus.DataProcessing => new DataProcessingStatusProvider(),

                OrderTaskListStatus.ReviewAndComplete => new ReviewAndCompleteStatusProvider(),

                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
            };
        }
    }
}
