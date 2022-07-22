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
        ImplementationPlanInProgress = 256,
        ImplementationPlan = 512,
        AssociatedServiceBillingInProgress = 1024,
        AssociatedServiceBilling = 2048,
        AssociatedServiceBillingNotApplicable = 4096,
        DataProcessingInformationInProgress = 8192,
        DataProcessingInformation = 16384,

        OrderingPartyComplete = OrderingParty,
        SupplierComplete = Supplier | OrderingPartyComplete,
        SupplierContactComplete = SupplierContact | SupplierComplete,
        CommencementDateComplete = CommencementDate | SupplierContactComplete,
        SolutionOrServiceComplete = SolutionOrService | SolutionOrServiceInProgress | CommencementDateComplete,
        FundingSourceComplete = FundingSource | FundingSourceInProgress | SolutionOrServiceComplete,
        ImplementationPlanComplete = ImplementationPlan | ImplementationPlanInProgress | FundingSourceComplete,
        AssociatedServiceBillingComplete = AssociatedServiceBilling | AssociatedServiceBillingInProgress | ImplementationPlanComplete,
        DataProcessingInformationCompleted = DataProcessingInformation | DataProcessingInformationInProgress | ImplementationPlanComplete,
    }
}
