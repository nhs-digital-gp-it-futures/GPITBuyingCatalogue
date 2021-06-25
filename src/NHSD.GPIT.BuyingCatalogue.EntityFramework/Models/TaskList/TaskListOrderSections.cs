using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.TaskList
{
    [Flags]
    public enum TaskListOrderSections
    {
        Description = 0,
        OrderingParty = 1 | Description,
        Supplier = 2 | OrderingParty,
        CommencementDate = 4 | Supplier,
        CatalogueSolutions = 8 | CommencementDate,
        AdditionalServices = 16 | CatalogueSolutions,
        AssociatedServices = 32 | CommencementDate,
        FundingSource = 64,
    }
}
