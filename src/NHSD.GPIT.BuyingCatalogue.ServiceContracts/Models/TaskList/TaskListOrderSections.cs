using System;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList
{
    [Flags]
    public enum TaskListOrderSections
    {
        Description = 0,
        OrderingParty = 1,
        Supplier = 2,
        SupplierContact = 4,
        CommencementDate = 8,
        SolutionOrService = 16,
        FundingSourceInProgress = 32,
        FundingSource = 64,
        DescriptionComplete = Description,
        OrderingPartyComplete = OrderingParty,
        SupplierComplete = Supplier | OrderingPartyComplete,
        SupplierContactComplete = SupplierContact | SupplierComplete,
        CommencementDateComplete = CommencementDate | SupplierContactComplete,
        SolutionOrServiceComplete = SolutionOrService | CommencementDateComplete,
        FundingSourceComplete = FundingSource | SolutionOrServiceComplete,
    }
}
