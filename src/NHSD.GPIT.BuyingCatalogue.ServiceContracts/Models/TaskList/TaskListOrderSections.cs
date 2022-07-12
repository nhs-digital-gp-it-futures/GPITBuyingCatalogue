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
        SolutionOrServiceInProgress = 16,
        SolutionOrService = 32,
        FundingSourceInProgress = 64,
        FundingSource = 128,
        DescriptionComplete = Description,
        OrderingPartyComplete = OrderingParty,
        SupplierComplete = Supplier | OrderingPartyComplete,
        SupplierContactComplete = SupplierContact | SupplierComplete,
        CommencementDateComplete = CommencementDate | SupplierContactComplete,
        SolutionOrServiceComplete = SolutionOrService | SolutionOrServiceInProgress | CommencementDateComplete,
        FundingSourceComplete = FundingSource | FundingSourceInProgress | SolutionOrServiceComplete,
    }
}
