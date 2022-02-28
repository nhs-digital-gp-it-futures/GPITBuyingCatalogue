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
        CatalogueSolutions = 16,
        AdditionalServices = 32,
        AssociatedServices = 64,
        FundingSource = 128,
        DescriptionComplete = Description,
        OrderingPartyComplete = OrderingParty,
        SupplierComplete = Supplier | OrderingPartyComplete,
        SupplierContactComplete = SupplierContact | SupplierComplete,
        CommencementDateComplete = CommencementDate | SupplierContactComplete,
        CatalogueSolutionsComplete = CatalogueSolutions | CommencementDateComplete,
        AdditionalServicesComplete = AdditionalServices | CatalogueSolutionsComplete,
        AssociatedServicesComplete = AssociatedServices | CommencementDateComplete,
        FundingSourceComplete = FundingSource,
    }
}
