using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.TaskList
{
    [Flags]
    public enum TaskListOrderSections
    {
        Description = 0,
        OrderingParty = 1,
        Supplier = 2,
        CommencementDate = 4,
        CatalogueSolutions = 8,
        AdditionalServices = 16,
        AssociatedServices = 32,
        FundingSource = 64,
        DescriptionComplete = Description,
        OrderingPartyComplete = OrderingParty | DescriptionComplete,
        SupplierComplete = Supplier | OrderingPartyComplete,
        CommencementDateComplete = CommencementDate | SupplierComplete,
        CatalogueSolutionsComplete = CatalogueSolutions | CommencementDateComplete,
        AdditionalServicesComplete = AdditionalServices | CatalogueSolutionsComplete,
        AssociatedServicesComplete = AssociatedServices | CommencementDateComplete,
        FundingSourceComplete = FundingSource,
    }
}
