using System;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList
{
    public class OrderTaskProgressProviderService : IOrderTaskProgressProviderService
    {
        public ITaskProgressProvider ProviderFor(OrderTaskListStatus status)
        {
            return status switch
            {
                OrderTaskListStatus.Description => new DescriptionStatusProvider(),
                OrderTaskListStatus.OrderingParty => new OrderingPartyStatusProvider(),
                OrderTaskListStatus.Supplier => new SupplierStatusProvider(),
                OrderTaskListStatus.CommencementDate => new CommencementDateStatusProvider(),

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
