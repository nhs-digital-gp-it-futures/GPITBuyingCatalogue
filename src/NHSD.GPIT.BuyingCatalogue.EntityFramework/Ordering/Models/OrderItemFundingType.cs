using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [Flags]
    public enum OrderItemFundingType
    {
        None = 0,
        CentralAllocation = 1,
        LocalAllocation = 2,
        MixedAllocation = CentralAllocation | LocalAllocation,
    }
}
